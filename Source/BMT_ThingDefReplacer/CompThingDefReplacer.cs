using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace BMT
{
    public class CompThingDefReplacer : ThingComp
    {
		private int ticksUntilSpawn;
		
        public CompProperties_ThingDefReplacer Props => (CompProperties_ThingDefReplacer) props;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
			{
				ResetCountdown();
			}
		}
        
		public override void CompTick()
		{
			TickInterval(1);
		}

		// public bool Active => parent.GetComp<CompPowerTrader>()?.PowerOn ?? false;

		public override void CompTickRare()
		{
			TickInterval(250);
		}

		private void TickInterval(int interval)
		{
			if (!parent.Spawned)
			{
				return;
			}
			CompCanBeDormant comp = parent.GetComp<CompCanBeDormant>();
			if (comp != null)
			{
				if (!comp.Awake)
				{
					return;
				}
			}
			else if (parent.Position.Fogged(parent.Map))
			{
				return;
			}
			else
			{
				ticksUntilSpawn -= interval;
				CheckShouldSpawn();
			}
		}

		private void CheckShouldSpawn()
		{
			if (ticksUntilSpawn <= 0)
			{
				ResetCountdown();
				SpawnThing();
				// GenPlace.TryPlaceThing(Props.evolveIntoThingDef, parent.Position, parent.Map, ThingPlaceMode.Near);
				// ThingMaker.MakeThing(Props.evolveIntoThingDef);
            }
		}
		private void ResetCountdown()
		{
			ticksUntilSpawn = Props.spawnIntervalRange.RandomInRange;
		}
		private void SpawnThing()
		{
			var newThing = CreateNewThing();
			if (newThing != null) GenSpawn.Spawn(newThing, parent.Position, parent.Map);
		}
        // public override void CompTickRare()
        // {
            // base.CompTickRare();

            // if (parent.Map == null || !(parent is Pawn oldPawn)) return;
            
            // if (oldPawn.ageTracker.AgeBiologicalTicks >= Props.ageInDays * 60000L)
            // {
                // var newThing = CreateNewThing(oldPawn);

                // Props.evolveSound?.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                
                // if (Props.filthDef != null) for (int i = 0; i < Props.filthAmount; i++)
                // {
                    // var parms = TraverseParms.For(TraverseMode.NoPassClosedDoors);
                    // if (CellFinder.TryFindRandomReachableCellNear(parent.Position, parent.Map, 2, parms, null, null, out var c))
                    // {
                        // FilthMaker.TryMakeFilth(c, parent.Map, Props.filthDef);
                    // }
                // }
                
                // if (newThing != null) GenSpawn.Spawn(newThing, parent.Position, parent.Map);
                // if (!parent.Destroyed) parent.Destroy();
            // }
        // }

        private Thing CreateNewThing()
        {
            // if (Props.evolveIntoPawnKindDef != null)
            // {
                // var request = new PawnGenerationRequest(Props.evolveIntoPawnKindDef)
                // {
                    // Faction = oldPawn.Faction,
                    // FixedGender = oldPawn.gender,
                    // FixedBiologicalAge = Props.carryOverAge ? oldPawn.ageTracker.AgeBiologicalYearsFloat : 0f,
                    // FixedChronologicalAge = Props.carryOverAge ? oldPawn.ageTracker.AgeChronologicalYearsFloat : 0f
                // };

                // var newPawn = PawnGenerator.GeneratePawn(request);
                // return newPawn;
            // }
            // ThingMaker.MakeThing(Props.evolveIntoThingDef);
            if (Props.evolveIntoThingDef != null)
            {
                return ThingMaker.MakeThing(Props.evolveIntoThingDef);
            }
            if (!parent.Destroyed) parent.Destroy();

            return null;
        }

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Prefs.DevMode)
			{
				Command_Action command_Action = new Command_Action();
				command_Action.defaultLabel = "DEV: Evolve";
				command_Action.action = delegate
				{
					ResetCountdown();
					SpawnThing();
				};
				yield return (Gizmo)command_Action;
			}
		}

        // public override string CompInspectStringExtra()
        // {
            // var key = Props.inspectionStringKey;
            // if (key.NullOrEmpty() || !(parent is Pawn oldPawn)) return null;
            // int remainingTicks = Math.Max(0, (int)(ticksUntilSpawn));
            // return key.Translate(remainingTicks.ToStringTicksToPeriod(false, false, false));
        // }
    }
    
    public class CompProperties_ThingDefReplacer : CompProperties
    {
        // public int ageInDays;
		public IntRange spawnIntervalRange = new IntRange(100, 100);
        public ThingDef evolveIntoThingDef;
		// public bool requiresPower;
        // public PawnKindDef evolveIntoPawnKindDef;
        // public SoundDef evolveSound;
        // public int filthAmount;
        // public ThingDef filthDef;
        // public bool carryOverAge;
        // public string inspectionStringKey;
        
        public CompProperties_ThingDefReplacer() => compClass = typeof(CompThingDefReplacer);
    }
}