using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server._Funkystation.Botany.PlantAnalyzer;

/// <summary>
/// After scanning, retrieves the target uid to use with its related UI.
/// </summary>
/// <remarks>
/// Requires <c>ItemToggleComponent</c>.
/// </remarks>
[RegisterComponent, AutoGenerateComponentPause]
[Access(typeof(PlantAnalyzerSystem))]
public sealed partial class PlantAnalyzerComponent : Component
{
    /// <summary>
    /// When should the next update be sent for the patient
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan NextUpdate = TimeSpan.Zero;

    /// <summary>
    /// The delay between patient health updates
    /// </summary>
    [DataField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

    [DataField]
    public int Version = 1;

    /// <summary>
    /// If the last state of the health analyzer was active (e.g. they are in range of the patient).
    /// </summary>
    [DataField]
    public bool IsAnalyzerActive = false;

    /// <summary>
    /// How long it takes to scan someone.
    /// </summary>
    [DataField]
    public TimeSpan ScanDelay = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Which entity has been scanned, for continuous updates
    /// </summary>
    [DataField]
    public EntityUid? ScannedEntity;

    /// <summary>
    /// The maximum range in tiles at which the analyzer can receive continuous updates, a value of null will be infinite range
    /// </summary>
    [DataField]
    public float? MaxScanRange = 2.5f;

    /// <summary>
    /// Sound played on scanning begin
    /// </summary>
    [DataField]
    public SoundSpecifier? ScanningBeginSound;

    /// <summary>
    /// Sound played on scanning end
    /// </summary>
    [DataField]
    public SoundSpecifier ScanningEndSound = new SoundPathSpecifier("/Audio/Items/Medical/healthscanner.ogg");

    /// <summary>
    /// Whether to show up the popup
    /// </summary>
    [DataField]
    public bool Silent;
}
