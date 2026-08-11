using System.Linq;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.PowerCell;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Timing;

using Content.Shared._Funkystation.Botany.PlantAnalyzer;
using Content.Shared.Botany.Components;
using Content.Shared.Botany.Systems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;


namespace Content.Server._Funkystation.Botany.PlantAnalyzer;

public sealed partial class PlantAnalyzerSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private PowerCellSystem _cell = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private ItemToggleSystem _toggle = default!;
    [Dependency] private UserInterfaceSystem _uiSystem = default!;
    [Dependency] private TransformSystem _transformSystem = default!;
    [Dependency] private BotanySystem _botany = default!;


    public override void Initialize()
    {
        SubscribeLocalEvent<PlantAnalyzerComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<PlantAnalyzerComponent, PlantAnalyzerDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<PlantAnalyzerComponent, EntGotInsertedIntoContainerMessage>(OnInsertedIntoContainer);
        SubscribeLocalEvent<PlantAnalyzerComponent, ItemToggledEvent>(OnToggled);
        SubscribeLocalEvent<PlantAnalyzerComponent, DroppedEvent>(OnDropped);
    }


    public override void Update(float frameTime)
    {

        var analyzerQuery = EntityQueryEnumerator<PlantAnalyzerComponent, TransformComponent>();
        while (analyzerQuery.MoveNext(out var uid, out var component, out var transform))
        {
            //Update rate limited to 1 second
            if (component.NextUpdate > _timing.CurTime)
                continue;

            if (component.ScannedEntity is not {} plant)
                continue;

            if (Deleted(plant))
            {
                StopAnalyzingEntity((uid, component), plant);
                continue;
            }

            component.NextUpdate = _timing.CurTime + component.UpdateInterval;

            //Get distance between plant analyzer and the scanned entity
            //null is infinite range
            var plantCoords = Transform(plant).Coordinates;
            if (component.MaxScanRange != null && !_transformSystem.InRange(plantCoords, transform.Coordinates, component.MaxScanRange.Value))
            {
                //Range too far, disable updates until they are back in range
                PauseAnalyzingEntity((uid, component), plant);
                continue;
            }

            component.IsAnalyzerActive = true;
            UpdateScannedPlant((uid, component), plant, true);
        }
    }



    /// <summary>
    /// Trigger the doafter for scanning
    /// </summary>
    private void OnAfterInteract(Entity<PlantAnalyzerComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Target == null || !args.CanReach || (!HasComp<PlantTrayComponent>(args.Target) && !HasComp<PlantComponent>(args.Target)) || !_cell.HasDrawCharge(ent.Owner, user: args.User))
            return;

        if (!ent.Comp.Silent)
            _audio.PlayPvs(ent.Comp.ScanningBeginSound, ent);

        var doAfterCancelled = !_doAfterSystem.TryStartDoAfter(new DoAfterArgs(EntityManager, args.User, ent.Comp.ScanDelay, new PlantAnalyzerDoAfterEvent(), ent, target: args.Target, used: ent)
        {
            NeedHand = true,
            BreakOnMove = true,
        });
    }


    /// <summary>
    /// Analyze an entity after a doafter
    /// </summary>
    private void OnDoAfter(Entity<PlantAnalyzerComponent> ent, ref PlantAnalyzerDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Target == null || !_cell.HasDrawCharge(ent.Owner, user: args.User))
            return;

        if (!ent.Comp.Silent)
            _audio.PlayPvs(ent.Comp.ScanningEndSound, ent);

        OpenUserInterface(args.User, ent);
        BeginAnalyzingEntity(ent, args.Target.Value);
        args.Handled = true;
    }



    /// <summary>
    /// Turn off when placed into a storage item or moved between slots/hands
    /// </summary>
    private void OnInsertedIntoContainer(Entity<PlantAnalyzerComponent> ent, ref EntGotInsertedIntoContainerMessage args)
    {
        if (ent.Comp.ScannedEntity is { } plant)
            _toggle.TryDeactivate(ent.Owner);
    }

    /// <summary>
    /// Disable continuous updates once turned off
    /// </summary>
    private void OnToggled(Entity<PlantAnalyzerComponent> ent, ref ItemToggledEvent args)
    {
        if (!args.Activated && ent.Comp.ScannedEntity is { } plant)
            StopAnalyzingEntity(ent, plant);
    }

    /// <summary>
    /// Turn off the analyzer when dropped
    /// </summary>
    private void OnDropped(Entity<PlantAnalyzerComponent> ent, ref DroppedEvent args)
    {
        if (ent.Comp.ScannedEntity is { } plant)
            _toggle.TryDeactivate(ent.Owner);
    }


    /// <summary>
    /// Turn open the analyzer UI
    /// </summary>
    private void OpenUserInterface(EntityUid user, EntityUid analyzer)
    {
        if (!_uiSystem.HasUi(analyzer, PlantAnalyzerUiKey.Key))
            return;

        _uiSystem.OpenUi(analyzer, PlantAnalyzerUiKey.Key, user);
    }


    /// <summary>
    /// Mark the entity as having its health analyzed, and link the analyzer to it
    /// </summary>
    /// <param name="ent">The plant analyzer</param>
    /// <param name="target">The entity being scanned</param>
    private void BeginAnalyzingEntity(Entity<PlantAnalyzerComponent> ent, EntityUid target)
    {
        //Link the health analyzer to the scanned entity
        ent.Comp.ScannedEntity = target;

        _toggle.TryActivate(ent.Owner);

        UpdateScannedPlant(ent, target, true);
    }

    /// <summary>
    /// Remove the analyzer from the active list, and remove the component if it has no active analyzers
    /// </summary>
    /// <param name="ent">The plant analyzer</param>
    /// <param name="target">The entity being scanned</param>
    private void StopAnalyzingEntity(Entity<PlantAnalyzerComponent> ent, EntityUid target)
    {
        //Unlink the analyzer
        ent.Comp.ScannedEntity = null;
        _toggle.TryDeactivate(ent.Owner);

        UpdateScannedPlant(ent, target, false);
    }


    /// <summary>
    /// If the scanner is active, sends one last update and sets it to inactive.
    /// </summary>
    /// <param name="ent">The plant analyzer</param>
    /// <param name="target">The entity being scanned</param>
    private void PauseAnalyzingEntity(Entity<PlantAnalyzerComponent> ent, EntityUid target)
    {
        if (!ent.Comp.IsAnalyzerActive)
            return;

        UpdateScannedPlant(ent, target, false);
        ent.Comp.IsAnalyzerActive = false;
    }


    /// <summary>
    /// Send an update for the target to the healthAnalyzer
    /// </summary>
    /// <param name="ent">The plant analyzer</param>
    /// <param name="target">The entity being scanned</param>
    /// <param name="scanMode">True makes the UI show ACTIVE, False makes the UI show INACTIVE</param>
    public void UpdateScannedPlant(Entity<PlantAnalyzerComponent> ent, EntityUid target, bool scanMode)
    {
        if (!_uiSystem.HasUi(ent, PlantAnalyzerUiKey.Key))
            return;

        var analyzerMessage = GetPlantAnalyzerUiState(ent, target);

        _uiSystem.ServerSendUiMessage(
            ent.Owner,
            PlantAnalyzerUiKey.Key,
            analyzerMessage
        );
    }

    /// <summary>
    /// Creates a HealthAnalyzerState based on the current state of an entity.
    /// </summary>
    /// <param name="ent">The plant analyzer</param>
    /// <param name="target">The entity being scanned</param>
    /// <returns></returns>
    public PlantAnalyzerUserMessage GetPlantAnalyzerUiState(Entity<PlantAnalyzerComponent> ent, EntityUid? target)
    {

        var plantUid = target;
        if (TryComp<PlantTrayComponent>(target, out var plantTray))
        {
            plantUid = plantTray.PlantEntity;
        }

        if (TryComp<PlantComponent>(plantUid, out var plant)
            && TryComp<PlantChemicalsComponent>(plantUid, out var chems)
            && TryComp<PlantDataComponent>(plantUid, out var plantData)
            && TryComp<PlantGrowthComponent>(plantUid, out var plantTolerance)
            && TryComp<PlantAtmosphericComponent>(plantUid, out var plantHeat))
        {
            return new PlantAnalyzerUserMessage(
                GetNetEntity(plantUid),
                ent.Comp.Version,
                plantData.Name,
                plant.Production,
                plant.Maturation,
                plant.Yield,
                plant.Potency,
                chems.Chemicals.Keys.ToList(),
                plant.Lifespan,
                plantTolerance.NutrientConsumption,
                plantTolerance.WaterConsumption,
                (plantHeat.HighHeatTolerance + plantHeat.LowHeatTolerance) / 2);
        }

        return new PlantAnalyzerUserMessage(
            GetNetEntity(target),
            1,
            "No plant",
            1,
            1,
            1,
            1,
            new List<ProtoId<ReagentPrototype>>(),
            1,
            1,
            1,
            1);

    }
}
