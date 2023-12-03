using RimWorld;
using Verse;

namespace BiomesCaverns.Incidents
{
	public class ThoughtWorker_PhosphoricLights : ThoughtWorker_GameCondition
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return base.CurrentStateInternal(p).Active && p.SpawnedOrAnyParentSpawned &&
			       !PawnUtility.IsBiologicallyOrArtificiallyBlind(p);
		}
	}
}