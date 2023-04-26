using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

public class JobDriver_FillMushroomFermentingBarrel : JobDriver
{
    private const TargetIndex BarrelInd = TargetIndex.A;

    private const TargetIndex WortInd = TargetIndex.B;

    private const int Duration = 200;

    protected Building_MushroomFermentingBarrel Barrel => (Building_MushroomFermentingBarrel) job.GetTarget(TargetIndex.A).Thing;

    protected Thing Wort => job.GetTarget(TargetIndex.B).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var pawn = this.pawn;
        LocalTargetInfo target = Barrel;
        var job = this.job;
        if (pawn.Reserve(target, job, 1, -1, null, errorOnFailed))
        {
            pawn = this.pawn;
            target = Wort;
            job = this.job;
            return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
        }

        return false;
    }

    [DebuggerHidden]
    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        AddEndCondition(() => Barrel.SpaceLeftForWort > 0 ? JobCondition.Ongoing : JobCondition.Succeeded);
        yield return Toils_General.DoAtomic(delegate
        {
            job.count = Barrel.SpaceLeftForWort;
        });
        var reserveWort = Toils_Reserve.Reserve(TargetIndex.B);
        yield return reserveWort;
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
        yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(TargetIndex.B);
        yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveWort, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return Toils_General.Wait(200).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
            .WithProgressBarToilDelay(TargetIndex.A);
        yield return new Toil
        {
            initAction = delegate
            {
                Barrel.AddWort(Wort);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}
