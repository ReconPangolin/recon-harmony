using Content.Shared._Funkystation.Botany.Components;
using Content.Shared._Funkystation.Botany.Systems;
using Content.Shared.Botany.Events;
using Content.Shared.Random.Helpers;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.Botany.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class PlantInstabilitySystem : SharedPlantInstabilitySystem
{
    [Dependency] private ILogManager _logManager = default!;
    [Dependency] private IGameTiming _timing = default!;

    [SubscribeLocalEvent]
    private void OnPlantGrow(Entity<PlantInstabilityComponent> ent, ref PlantGrowEvent args)
    {
        var traitChance = ent.Comp.Instability / 100;
        if (SharedRandomExtensions.PredictedProb(_timing, traitChance, GetNetEntity(ent.Owner)))
        {
            var sawmill = _logManager.GetSawmill("InstabilitySystem");
            sawmill.Debug("Funky plant mutation");
        }
    }
}
