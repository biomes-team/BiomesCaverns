using System;
using RimWorld;
using Verse;
using Verse.AI;

public class WorkGiver_FillMushroomFermentingBarrel : WorkGiver_Scanner
{
    private static string TemperatureTrans;

    private static string NoWortTrans;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDef.Named("BMT_MushroomFermentingBarrel"));

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public static void ResetStaticData()
    {
        TemperatureTrans = "BadTemperature".Translate().ToLower();
        NoWortTrans = "NoMushroomWineMust".Translate();
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var building_MushroomWineFermentingBarrel = t as Building_MushroomFermentingBarrel;
        if (building_MushroomWineFermentingBarrel == null || building_MushroomWineFermentingBarrel.Fermented || building_MushroomWineFermentingBarrel.SpaceLeftForWort <= 0)
        {
            return false;
        }

        float ambientTemperature = building_MushroomWineFermentingBarrel.AmbientTemperature;
        var compProperties = building_MushroomWineFermentingBarrel.def.GetCompProperties<CompProperties_TemperatureRuinable>();
        if (ambientTemperature < compProperties.minSafeTemperature + 2f || ambientTemperature > compProperties.maxSafeTemperature - 2f)
        {
            JobFailReason.Is(TemperatureTrans);
            return false;
        }

        if (!t.IsForbidden(pawn))
        {
            LocalTargetInfo target = t;
            if (pawn.CanReserve(target, 1, -1, null, forced))
            {
                if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
                {
                    return false;
                }

                if (FindWort(pawn, building_MushroomWineFermentingBarrel) == null)
                {
                    JobFailReason.Is(NoWortTrans);
                    return false;
                }

                return !t.IsBurning();
            }
        }

        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var barrel = (Building_MushroomFermentingBarrel) t;
        var t2 = FindWort(pawn, barrel);
        return new Job(DefDatabase<JobDef>.GetNamed("BMT_FillMushroomFermentingBarrelJob"), t, t2);
    }

    private Thing FindWort(Pawn pawn, Building_MushroomFermentingBarrel barrel)
    {
        bool Predicate(Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x);
        var position = pawn.Position;
        var map = pawn.Map;
        var thingReq = ThingRequest.ForDef(ThingDef.Named("BMT_MushroomMust"));
        var peMode = PathEndMode.ClosestTouch;
        var traverseParams = TraverseParms.For(pawn);
        var validator = (Predicate<Thing>) Predicate;
        return GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams, 9999f, validator);
    }
}
