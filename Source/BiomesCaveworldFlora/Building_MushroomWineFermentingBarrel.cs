using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

[StaticConstructorOnStartup]
public class Building_MushroomFermentingBarrel : Building, IThingHolder
{
	private int wortCount;

	private ThingOwner ingredients;

	private float progressInt;

	private Material barFilledCachedMat;

	public const int MaxCapacity = 25;

	private const int BaseFermentationDuration = 360000;

	public const float MinIdealTemperature = 7f;

	private static readonly Vector2 BarSize = new Vector2(0.55f, 0.1f);

	private static readonly Color BarZeroProgressColor = new Color(0.4f, 0.27f, 0.22f);

	private static readonly Color BarFermentedColor = new Color(0.9f, 0.85f, 0.2f);

	private static readonly Material BarUnfilledMat =
		SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));

	public float Progress
	{
		get => progressInt;

		set
		{
			if (value != progressInt)
			{
				progressInt = value;
				barFilledCachedMat = null;
			}
		}
	}

	private Material BarFilledMat
	{
		get
		{
			if (barFilledCachedMat == null)
			{
				barFilledCachedMat =
					SolidColorMaterials.SimpleSolidColorMaterial(Color.Lerp(BarZeroProgressColor, BarFermentedColor, Progress));
			}

			return barFilledCachedMat;
		}
	}

	public int SpaceLeftForWort
	{
		get
		{
			if (Fermented)
			{
				return 0;
			}

			return 25 - wortCount;
		}
	}

	private bool Empty => wortCount <= 0;

	public bool Fermented => !Empty && Progress >= 1f;

	private float CurrentTempProgressSpeedFactor
	{
		get
		{
			var compProperties = def.GetCompProperties<CompProperties_TemperatureRuinable>();
			float ambientTemperature = AmbientTemperature;
			if (ambientTemperature < compProperties.minSafeTemperature)
			{
				return 0.1f;
			}

			if (ambientTemperature < 7f)
			{
				return GenMath.LerpDouble(compProperties.minSafeTemperature, 7f, 0.1f, 1f, ambientTemperature);
			}

			return 1f;
		}
	}

	private float ProgressPerTickAtCurrentTemp => 2.77777781E-06f * CurrentTempProgressSpeedFactor;

	private int EstimatedTicksLeft => Mathf.Max(Mathf.RoundToInt((1f - Progress) / ProgressPerTickAtCurrentTemp), 0);

	public Building_MushroomFermentingBarrel()
	{
		ingredients = new ThingOwner<Thing>(this, oneStackOnly: false);
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref wortCount, "wortCount");
		Scribe_Values.Look(ref progressInt, "progress");
		Scribe_Deep.Look(ref ingredients, "ingredients", this);
	}

	public override void TickRare()
	{
		base.TickRare();
		if (!Empty)
		{
			Progress = Mathf.Min(Progress + 250f * ProgressPerTickAtCurrentTemp, 1f);
		}
	}

	protected override void ReceiveCompSignal(string signal)
	{
		if (signal == "RuinedByTemperature")
		{
			Reset();
		}
	}

	private void Reset()
	{
		wortCount = 0;
		Progress = 0f;
		if (!ingredients.NullOrEmpty())
		{
			ingredients.ClearAndDestroyContents();
		}
	}

	public void AddWort(Thing wort)
	{
		int num = Mathf.Min(wort.stackCount, 25 - wortCount);
		if (num > 0)
		{
			ingredients ??= new ThingOwner<Thing>(this, oneStackOnly: false);

			wort.holdingOwner.TryTransferToContainer(wort, ingredients, num);
			GetComp<CompTemperatureRuinable>().Reset();
			if (Fermented)
			{
				Log.Warning(
					"Tried to add mushroom wine must to a barrel full of mushroom wine. Colonists should take out the finished mushroom wine first.");
				return;
			}

			Progress = GenMath.WeightedAverage(0f, num, Progress, wortCount);
			wortCount += num;
		}
	}

	public override string GetInspectString()
	{
		var stringBuilder = new StringBuilder();
		stringBuilder.Append(base.GetInspectString());
		if (stringBuilder.Length != 0)
		{
			stringBuilder.AppendLine();
		}

		var comp = GetComp<CompTemperatureRuinable>();
		if (!Empty && !comp.Ruined)
		{
			if (Fermented)
			{
				stringBuilder.AppendLine("BMT_ContainsMushroomWine".Translate(wortCount, 25));
			}
			else
			{
				stringBuilder.AppendLine("BMT_ContainsMushroomMust".Translate(wortCount, 25));
			}
		}

		if (!Empty)
		{
			if (Fermented)
			{
				stringBuilder.AppendLine("Fermented".Translate());
			}
			else
			{
				stringBuilder.AppendLine("FermentationProgress".Translate(Progress.ToStringPercent(),
					EstimatedTicksLeft.ToStringTicksToPeriod()));
				if (CurrentTempProgressSpeedFactor != 1f)
				{
					stringBuilder.AppendLine(
						"FermentationBarrelOutOfIdealTemperature".Translate(CurrentTempProgressSpeedFactor.ToStringPercent()));
				}
			}
		}

		stringBuilder.AppendLine("Temperature".Translate() + ": " + AmbientTemperature.ToStringTemperature("F0"));
		stringBuilder.AppendLine(string.Concat("IdealFermentingTemperature".Translate(), ": ", 7f.ToStringTemperature("F0"),
			" ~ ", comp.Props.maxSafeTemperature.ToStringTemperature("F0")));
		return stringBuilder.ToString().TrimEndNewlines();
	}

	public Thing TakeOutBeer()
	{
		if (!Fermented)
		{
			Log.Warning("Tried to get mushroom wine but it's not yet fermented.");
			return null;
		}

		var thing = ThingMaker.MakeThing(ThingDef.Named("BMT_MushroomWine"));
		var @object = thing.TryGetComp<CompIngredients>();
		if (!ingredients.NullOrEmpty())
		{
			foreach (var item in ingredients)
			{
				item.TryGetComp<CompIngredients>()?.ingredients.ForEach(@object.RegisterIngredient);
			}
		}

		thing.stackCount = wortCount;
		Reset();
		return thing;
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		base.DrawAt(drawLoc, flip);
		if (!Empty)
		{
			drawLoc.y += 3.0F / 64.0F;
			drawLoc.z += 0.25F;
			var r = default(GenDraw.FillableBarRequest);
			r.center = drawLoc;
			r.size = BarSize;
			r.fillPercent = wortCount / 25.0F;
			r.filledMat = BarFilledMat;
			r.unfilledMat = BarUnfilledMat;
			r.margin = 0.1F;
			r.rotation = Rot4.North;
			GenDraw.DrawFillableBar(r);
		}
	}

	[DebuggerHidden]
	public override IEnumerable<Gizmo> GetGizmos()
	{
		foreach (var gizmo in base.GetGizmos())
		{
			yield return gizmo;
		}

		if (Prefs.DevMode && !Empty)
		{
			yield return new Command_Action
			{
				defaultLabel = "Debug: Set progress to 1",
				action = delegate { Progress = 1f; }
			};
		}
	}

	public void GetChildHolders(List<IThingHolder> outChildren)
	{
		ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
	}

	public ThingOwner GetDirectlyHeldThings()
	{
		return ingredients;
	}
}