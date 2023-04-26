using RimWorld;
using Verse;
using Verse.AI;

public class WorkGiver_TakeMushroomWineOutOfFermentingBarrel : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDef.Named("BMT_MushroomFermentingBarrel"));

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!(t is Building_MushroomFermentingBarrel { Fermented: true }))
        {
            return false;
        }

        if (t.IsBurning())
        {
            return false;
        }

        if (!t.IsForbidden(pawn))
        {
            LocalTargetInfo target = t;
            if (pawn.CanReserve(target, 1, -1, null, forced))
            {
                return true;
            }
        }

        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(DefDatabase<JobDef>.GetNamed("BMT_TakeMushroomWineOutOfFermentingBarrelJob"), t);
    }
}
