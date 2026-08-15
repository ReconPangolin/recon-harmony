using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Botany;

/// <summary>
/// Data that specifies the odds and effects of possible random plant mutations.
/// </summary>
[Prototype]
public sealed partial class FuPlantMutationPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// List of RandomFills that can be picked from.
    /// </summary>
    [DataField(required: true)]
    public List<FuPlantEffect> PlantEffects = [];

    /// <summary>
    /// This mutation stays on the plant and its produce. If false while AppliesToPlant is true, the effect will run when triggered.
    /// </summary>
    [DataField]
    public bool Inheritable = true;

    /// <summary>
    /// This mutation stays on the plant and its produce. If false while AppliesToPlant is true, the effect will run when triggered.
    /// </summary>
    public bool Innate;
}
