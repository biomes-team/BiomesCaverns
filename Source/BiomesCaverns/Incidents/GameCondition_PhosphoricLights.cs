using RimWorld;
using UnityEngine;
using Verse;

namespace BiomesCaverns.Incidents
{
	/// <summary>
	/// Version of GameCondition_Aurora which does not get interrupted by daylight and does not use it to calculate light
	/// intensity.
	/// </summary>
	public class GameCondition_PhosphoricLights : GameCondition
	{
		private int _prevColorIndex;
		private bool _prevIsDark;
		private int _curColorIndex;
		private float _curColorTransition;
		private const float Glow = 1.0F;
		private const float SkyColorStrength = 0.2F;

		private const float OverlayColorStrength = 0.05F;

		// Must be less than DarknessCombatUtility.SkyGlowDarkThreshold, which is 0.35.
		private const float BaseBrightness = 1.0f;
		private const int TransitionDurationTicksPermanent = GenDate.TicksPerHour - 3;
		private const int TransitionDurationTicksNotPermanent = TransitionDurationTicksPermanent / 5;

		// Both color arrays should have the same size.
		private static readonly Color[] PhosphoricColorsDark =
		{
			new Color(0.2F, 0.7F, 0.2F), // Soft green
			new Color(0.0F, 0.5F, 1.0F), // Deep blue
			new Color(0.5F, 0.0F, 0.5F), // Violet
			new Color(0.1F, 0.1F, 0.3F), // Indigo
			new Color(0.4F, 0.0F, 0.4F), // Purple
			new Color(0.3F, 0.3F, 0.0F), // Chartreuse
			new Color(0.2F, 0.2F, 0.2F), // Dim gray
			new Color(0.1F, 0.1F, 0.1F) // Dark gray
		};

		private static readonly Color[] PhosphoricColorsBright =
		{
			new Color(0.2F, 1.0F, 0.2F), // Bright green
			new Color(0.2F, 0.7F, 1.0F), // Light blue
			new Color(1.0F, 0.4F, 1.0F), // Lavender
			new Color(0.3F, 0.2F, 1.0F), // Periwinkle
			new Color(0.7F, 0.2F, 1.0F), // Orchid
			new Color(0.6F, 1.0F, 0.2F), // Lime
			new Color(0.5F, 0.5F, 0.5F), // Silver
			new Color(0.8F, 0.8F, 0.8F) // Light gray
		};

		private Color CurrentColor()
		{
			Color prev = _prevIsDark ? PhosphoricColorsDark[_prevColorIndex] : PhosphoricColorsBright[_prevColorIndex];
			Color cur = !_prevIsDark ? PhosphoricColorsDark[_curColorIndex] : PhosphoricColorsBright[_curColorIndex];
			return Color.Lerp(prev, cur, _curColorTransition);
		}

		private int TransitionDurationTicks()
		{
			return !Permanent ? TransitionDurationTicksNotPermanent : TransitionDurationTicksPermanent;
		}

		public override int TransitionTicks => 200;

		private void InitColorData()
		{
			_prevIsDark = true;
			_prevColorIndex = Rand.Range(0, PhosphoricColorsDark.Length);
			_curColorIndex = Rand.Range(0, PhosphoricColorsBright.Length);
			_curColorTransition = 0.0F;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				// Set the phosphoric lights to a random state on load instead of persisting it.
				InitColorData();
			}
		}

		public override void Init()
		{
			base.Init();
			InitColorData();
		}

		// In case we decide to allow skygazing on caverns.
		/*
		public override float SkyGazeChanceFactor(Map map)
		{
			return 8.0F;
		}

		public override float SkyGazeJoyGainFactor(Map map)
		{
			return 5.0F;
		}
		*/

		public override float SkyTargetLerpFactor(Map map)
		{
			return GameConditionUtility.LerpInOutValue(this, TransitionTicks);
		}

		public override SkyTarget? SkyTarget(Map map)
		{
			Color currentColor = CurrentColor();
			SkyColorSet colorSet = new SkyColorSet(
				Color.Lerp(Color.white, currentColor, SkyColorStrength) * BaseBrightness,
				new Color(0.92F, 0.92F, 0.92F),
				Color.Lerp(Color.white, currentColor, OverlayColorStrength) * BaseBrightness, 1.0F);

			return new SkyTarget(Glow, colorSet, 1.0F, 1.0F);
		}

		public override void GameConditionTick()
		{
			_curColorTransition += 1.0F / TransitionDurationTicks();
			if (_curColorTransition >= 1.0F)
			{
				_prevIsDark = !_prevIsDark;
				_prevColorIndex = _curColorIndex;
				_curColorIndex = Rand.Range(0, PhosphoricColorsDark.Length);
				_curColorTransition = 0.0f;
			}
		}
	}
}