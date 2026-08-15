using Content.Shared.EntityEffects;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Botany;

[Serializable, NetSerializable]
[DataDefinition]
public sealed partial class FuPlantEffect
{
    /// <summary>
    /// The actual EntityEffect to apply to the target
    /// </summary>
    [DataField(required: true)]
    public List<EntityEffect> Effects = default!;

    /// <summary>
    /// This mutation will target the harvested produce
    /// </summary>
    [DataField]
    public bool AppliesToProduce;

    /// <summary>
    /// This mutation will target the growing plant as soon as this mutation is applied.
    /// </summary>
    [DataField]
    public bool AppliesToPlant;

    /// <summary>
    /// This mutation will target the growing plant as soon as this mutation is applied.
    /// </summary>
    [DataField]
    public bool AppliesToHarvester;

    /// <summary>
    /// This mutation will target the growing plant as soon as this mutation is applied.
    /// </summary>
    [DataField]
    public bool AppliesWhenHarvested;
}
