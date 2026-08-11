using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Botany.PlantAnalyzer;

[Serializable, NetSerializable]
public sealed class PlantAnalyzerUserMessage : BoundUserInterfaceMessage
{
    public readonly NetEntity? TargetEntity;
    public int AnalyzerTier;
    public float Production;
    public float Maturation;
    public int Yield;
    public float Potency;
    public List<string>? ChemsBasic;
    public string PlantName;


    public float Lifespan;

    public float NutrientCons;

    public float WaterCons;

    public float IdealHeat;


    public PlantAnalyzerUserMessage(NetEntity? targetEntity, int analyzerTier, string plantName,
        float production, float maturation, int yield, float potency, List<ProtoId<ReagentPrototype>> chems,
        float lifespan, float nutrientCons, float waterCons, float idealHeat)
    {
        TargetEntity = targetEntity;
        AnalyzerTier = analyzerTier;

        //Tier 1 and above stats
        Production = production;
        Maturation = maturation;
        Yield = yield;
        PlantName = plantName;
        Potency = potency;

        if (analyzerTier > 1)
        {
            //TODO: Fix chems
            ChemsBasic = null;
            Lifespan = lifespan;
            NutrientCons = nutrientCons;
            WaterCons = waterCons;
            IdealHeat = idealHeat;
        }
    }
}
