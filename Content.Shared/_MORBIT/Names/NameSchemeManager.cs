using Content.Shared.Dataset;
using Content.Shared.Humanoid;
using Content.Shared.Morbit.Names.Prototypes;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Morbit.Names;

public sealed partial class NameSchemeManager
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public string GenerateName(NameSchemePrototype prototype, NameSchemeContext context)
    {
        var name = "";

        foreach (var part in prototype.Parts)
            name += part.Generate(context, this, _prototype, _random);

        return name;
    }

    public string GenerateName(ProtoId<NameSchemePrototype> protoId, NameSchemeContext context)
    {
        var prototype = _prototype.Index(protoId);
        return GenerateName(prototype, context);
    }
}

public sealed partial class NameSchemeContext
{
    public Dictionary<string, List<ProtoId<LocalizedDatasetPrototype>>> Datasets = new();
    public Gender? Gender = null;
}
