using Content.Shared._Funkystation.Botany.Components;
using Content.Shared._Funkystation.Botany.Systems;
using Content.Shared.Botany.Systems;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects.Botany.PlantAttributes;

namespace Content.Shared._Funkystation.EntityEffects.Effects.Botany.PlantAttributes;

/// <summary>
/// Entity effect that adjusts the instability of a plant.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class PlantAdjustInstabilityEntityEffectSystem : EntityEffectSystem<PlantInstabilityComponent, PlantAdjustInstability>
{
    [Dependency] private SharedPlantInstabilitySystem _plantInstability = default!;
    [Dependency] private PlantHolderSystem _plantHolder = default!;

    protected override void Effect(Entity<PlantInstabilityComponent> entity, ref EntityEffectEvent<PlantAdjustInstability> args)
    {
        if (_plantHolder.IsDead(entity.Owner))
            return;

        _plantInstability.AdjustPlantInstability(entity.AsNullable(), args.Effect.Amount);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class PlantAdjustInstability : BasePlantAdjustAttribute<PlantAdjustInstability>
{
    public override string GuidebookAttributeName { get; set; } = "plant-attribute-instability";
    public override bool GuidebookIsAttributePositive { get; protected set; } = false;
}
