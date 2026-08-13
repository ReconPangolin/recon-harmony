using Content.Shared._Funkystation.Botany.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Botany.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true)]
[Access(typeof(SharedPlantInstabilitySystem))]
public sealed partial class PlantInstabilityComponent : Component
{
    /// <summary>
    /// Maximum toxin level the plant can tolerate before taking damage.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Instability = 0f;
}

