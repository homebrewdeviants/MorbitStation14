using Robust.Shared.Prototypes;

namespace Content.Shared.Morbit.Motifs;

/// <summary>
///     All Morbitians have at least one motif (gods generally have two).
///
///     Motifs are represented by a physical substance (such as smoke, crystal, meat, chains, or light). A morbitian's
///     motif is linked to certain themes and feelings that feel especially relevant to them, either literally or
///     abstractly. For example, someone may have a rain motif if they find rainy days important or comforting to them,
///     or another person may have a rain motif because they are moody.
///
///     Motifs are related to scrap affinity, and scrap producers will produce scraps of the type associated with their
///     motif. Conversely, scraps will also have associated motifs.
/// </summary>
[Prototype]
[Access(Other = AccessPermissions.ReadExecute)]
public sealed partial class MotifPrototype : IPrototype
{
    /// <summary>
    /// Identifier for this prototype.
    /// </summary>
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// The name of the motif.
    /// </summary>
    [DataField]
    public LocId Name = default!;

    /// <summary>
    /// A color associated with the motif.
    /// </summary>
    [DataField("color")]
    public Color AssociatedColor = Color.White;

    [DataField]
    public ScrapVisualProperties ScrapProperties = new();
}

[DataDefinition]
public sealed partial class ScrapVisualProperties
{
    /// <summary>
    /// The outline color of the motif in scrap form.
    /// </summary>
    [DataField]
    public Color OutlineColor = Color.White;

    /// <summary>
    /// The base color of the motif in scrap form.
    /// </summary>
    [DataField]
    public Color BaseColor = Color.White;

    /// <summary>
    /// The path of the RSI file containing scrap layer states for this motif.
    /// </summary>
    [DataField]
    public string Sprite = "_MORBIT/Objects/Scrapmancy/scrap.rsi";

    /// <summary>
    /// The state name of this motif's scrap form.
    /// </summary>
    [DataField]
    public string State = "default";
}
