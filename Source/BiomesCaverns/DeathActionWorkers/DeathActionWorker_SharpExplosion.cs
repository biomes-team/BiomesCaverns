using RimWorld;
using Verse;
using Verse.AI.Group;

namespace BiomesCaverns.DeathActionWorkers
{
	public class DeathActionWorker_SharpExplosion : DeathActionWorker
	{
		public override RulePackDef DeathRules => RulePackDefOf.Transition_DiedExplosive;

		public override bool DangerousInMelee => true;

		public override void PawnDied(Corpse corpse, Lord prevLord)
		{
			float radius = corpse.InnerPawn.ageTracker.CurLifeStageIndex != 0
				? (corpse.InnerPawn.ageTracker.CurLifeStageIndex != 1 ? 4.9F : 2.9F)
				: 1.9F;
			GenExplosion.DoExplosion(corpse.Position, corpse.Map, radius, BC_DefOf.BMT_Crystalope_SharpBomb,
				corpse.InnerPawn);
		}
	}
}