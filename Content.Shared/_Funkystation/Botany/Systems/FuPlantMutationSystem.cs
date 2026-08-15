using Content.Shared._Funkystation.Botany.Components;
using Content.Shared.Botany.Events;
using Content.Shared.EntityEffects;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class FuPlantMutationSystem : EntitySystem
{

    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private SharedEntityEffectsSystem _entityEffects = default!;

    [SubscribeLocalEvent]
    private void OnAfterDoHarvest(Entity<FuPlantMutationComponent> ent, ref AfterDoHarvestEvent args)
    {
        //TODO: Optimise
        foreach (var mutationId in ent.Comp.Mutations)
        {
            var plantMutation = _prototypeManager.Index(mutationId);

            foreach (var effects in plantMutation.PlantEffects)
            {
                if (effects.AppliesToHarvester)
                {
                    foreach (var effect in effects.Effects)
                    {
                        _entityEffects.TryApplyEffect(args.User, effect);
                    }
                }

                if (effects.AppliesWhenHarvested)
                {
                    foreach (var effect in effects.Effects)
                    {
                        _entityEffects.TryApplyEffect(args.Target, effect);
                    }
                }
            }
        }
    }
}
