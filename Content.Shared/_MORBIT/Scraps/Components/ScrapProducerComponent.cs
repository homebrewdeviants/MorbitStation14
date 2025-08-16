using Content.Shared.Morbit.Motifs;
using Content.Shared.Random;
using Robust.Shared.Prototypes;

namespace Content.Shared.Morbit.Scraps.Components;

[RegisterComponent]
public sealed partial class ScrapProducerComponent : Component
{
    /// <summary>
    ///     A list of motifs associated with this entity.
    /// </summary>
    [DataField, ViewVariables]
    public HashSet<ProtoId<MotifPrototype>> Motifs = new();

    /// <summary>
    ///     The maximum number of motifs this entity can have.
    /// </summary>
    [DataField("maxMotifs"), ViewVariables]
    public int MaximumMotifs = 1;

    /// <summary>
    ///     As entities generate scraps, it becomes more difficult to generate more scraps from that point on.
    ///     This field applies a penalty to scrap growth and generation the higher it gets.
    /// </summary>
    [DataField, ViewVariables]
    public float MemoryTolerance = 1.0f;

    /// <summary>
    ///     The base value for minimum "event severity" needed in order for a scrap to be generated.
    ///     "Low-severity events" will contribute to scrap growth, but will not lead to scrap generation.
    ///     More impactful events can cause a scrap to be released, scaling with <see cref="MemoryTolerance"/>.
    /// </summary>
    [DataField, ViewVariables]
    public float BaseScrapReleaseSeverity = 5.0f;

    /// <summary>
    ///     The minimum required <see cref="ScrapGrowth"/> needed before a scrap may have the possibility of being
    ///     released. The actual growth needed will scale with <see cref="MemoryTolerance"/>.
    /// </summary>
    [DataField]
    public float BaseScrapGrowthRequirement = 50.0f;

    /// <summary>
    ///     The combined potency of all memories in this component. Once this value accumulates enough, a
    ///     sufficiently high-severity event will cause a scrap to be shed.
    /// </summary>
    [ViewVariables]
    public float ScrapGrowth = 0.0f;

    /// <summary>
    ///     The minimum required event severity needed for a scrap to be released.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float ScrapReleaseSeverity = 0.0f;

    /// <summary>
    ///     The minimum required scrap growth needed for a scrap to be released.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public float ScrapGrowthRequirement = 0.0f;

    /// <summary>
    ///     A list of "memories" that will be included in the next scrap that generates.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public List<EntityUid> Memories = new();

    /// <summary>
    ///     A list of all scrap entities this entity has generated.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public List<EntityUid> GeneratedScraps = new();
}
