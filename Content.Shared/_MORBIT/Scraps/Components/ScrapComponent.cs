using Content.Shared.Morbit.Motifs;
using Content.Shared.Morbit.Scraps.EntitySystems;
using Robust.Shared.Prototypes;

namespace Content.Shared.Morbit.Scraps.Components;

/// <summary>
///     Scraps are pieces of a soul, produced by all living beings - plants, animals, sometimes robots and the like.
///     Scraps contain emotions and memories. They act as a power source, an energy source for scrap users (wizards),
///     can be used to resurrect people (imperfectly), and are often used to power the abilities of certain Morbitian
///     species.
/// </summary>
[RegisterComponent]
[Access(typeof(SharedScrapSystem))]
public sealed partial class ScrapComponent : Component
{
    /// <summary>
    ///     Power determines the potency of a scrap - how much energy it provides, how strong its effects are.
    ///     Scrap power is generally determined upon creation by the complexity/importance of the memories within them.
    /// </summary>
    [DataField]
    public float Power = 1.0f;

    /// <summary>
    ///     A list of motifs associated with this scrap.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<MotifPrototype>> Motifs = new();

    // TODO: Store memories.
}
