using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Botany.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FuPlantMutationComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<ProtoId<FuPlantMutationPrototype>> Mutations = [];
}
