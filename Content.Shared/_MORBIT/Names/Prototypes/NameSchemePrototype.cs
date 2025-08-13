using Content.Shared.Morbit.Names.NameSchemeParts;
using Robust.Shared.Prototypes;

namespace Content.Shared.Morbit.Names.Prototypes;

[Prototype]
public sealed partial class NameSchemePrototype : IPrototype
{
    /// <summary>
    /// Identifier for this prototype.
    /// </summary>
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public List<NameSchemePart> Parts = new();
}
