

using Content.Shared._Funkystation.Botany.Components;

namespace Content.Shared._Funkystation.Botany.Systems;

/// <summary>
/// This handles...
/// </summary>
public abstract partial class SharedPlantInstabilitySystem : EntitySystem
{
    /// <summary>
    /// Adjusts the instability of a plant
    /// </summary>
    public void AdjustPlantInstability(Entity<PlantInstabilityComponent?> ent, float amount)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, false))
            return;

        ent.Comp.Instability = MathHelper.Clamp(ent.Comp.Instability + amount, 0, 100);

        DirtyField(ent, nameof(ent.Comp.Instability));
    }

}
