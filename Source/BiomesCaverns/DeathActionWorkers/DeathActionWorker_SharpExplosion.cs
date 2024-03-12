using RimWorld;
using Verse;

namespace BiomesCaverns.DeathActionWorkers
{
	public class DeathActionWorker_SharpExplosion : DeathActionWorker
	{
		public override RulePackDef DeathRules => RulePackDefOf.Transition_DiedExplosive;

		public override bool DangerousInMelee => true;

		public override void PawnDied(Corpse corpse)
		{
			float radius = corpse.InnerPawn.ageTracker.CurLifeStageIndex == 0
				? 1.9F
				: (corpse.InnerPawn.ageTracker.CurLifeStageIndex != 1 ? 4.9F : 2.9F);
			GenExplosion.DoExplosion(corpse.Position, corpse.Map, radius, BC_DefOf.BMT_Crystalope_SharpBomb,
				corpse.InnerPawn);
		}
	}
}