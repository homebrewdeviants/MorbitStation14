using System.Linq;
using Content.Shared.Dataset;
using Content.Shared.Morbit.Names.Prototypes;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using JetBrains.Annotations;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;

namespace Content.Shared.Morbit.Names.NameSchemeParts;

[ImplicitDataDefinitionForInheritors]
[Serializable, NetSerializable]
[MeansImplicitUse]
public abstract partial class NameSchemePart
{
    public abstract string Generate(NameSchemeContext context,
        NameSchemeManager nameSchemeMan,
        IPrototypeManager protoMan,
        IRobustRandom random);
}

/// <summary>
///     Part consists of a bit of static, unlocalized text.
///     Using this is not recommended except (possibly) symbols and the like.
/// </summary>
public sealed partial class TextPart : NameSchemePart
{
    [DataField(required: true)]
    public string Text = string.Empty;

    public override string Generate(NameSchemeContext context,
        NameSchemeManager nameSchemeMan,
        IPrototypeManager protoMan,
        IRobustRandom random)
    {
        return Text;
    }
}

/// <summary>
///     Part consists of a localized text string.
///     Optional "Args" parameter allows you to use another NameSchemePart as parameters to Loc.GetString().
/// </summary>
public sealed partial class LocalizedTextPart : NameSchemePart
{
    [DataField(required: true)]
    public LocId Text = string.Empty;

    [DataField]
    public Dictionary<string, NameSchemePart> Args = new();

    public override string Generate(NameSchemeContext context,
        NameSchemeManager nameSchemeMan,
        IPrototypeManager protoMan,
        IRobustRandom random)
    {

        var args = Args
            .Select(a => (a.Key, (object)a.Value.Generate(context, nameSchemeMan, protoMan, random)))
            .ToArray();

        return Loc.GetString(Text, args);
    }
}

/// <summary>
///     Part consists of a contextually-provided dataset.
///     For example, species will have a list of datasets for their first and last names, which can then
///     be referenced by this NameSchemePart.
/// </summary>
public sealed partial class NameDatasetPart : NameSchemePart
{
    [DataField(required: true)]
    public string Dataset = string.Empty;

    public override string Generate(NameSchemeContext context,
        NameSchemeManager nameSchemeMan,
        IPrototypeManager protoMan,
        IRobustRandom random)
    {
        var datasets = context.Datasets;

        if (datasets.TryGetValue(Dataset, out var nameDatasets))
        {
            var datasetId = random.Pick(nameDatasets);
            var nameDataset = protoMan.Index(datasetId);
            return random.Pick(nameDataset);
        }

        throw new KeyNotFoundException($"Key {Dataset} does not exist in datasets: {string.Join(", ", datasets.Keys)}");
    }
}

/// <summary>
///     Part consists of a random localized string from a LocalizedDataset.
/// </summary>
public sealed partial class LocalizedDatasetPart : NameSchemePart
{
    [DataField(required: true)]
    public ProtoId<LocalizedDatasetPrototype> Id;

    public override string Generate(NameSchemeContext context,
        NameSchemeManager nameSchemeMan,
        IPrototypeManager protoMan,
        IRobustRandom random)
    {
        var dataset = protoMan.Index(Id);
        return random.Pick(dataset);
    }
}

/// <summary>
///     Part consists of a randomly-selected NameSchemePrototype from a list.
/// </summary>
public sealed partial class WeightedRandomSchemePart : NameSchemePart
{
    [DataField(required: true)]
    public ProtoId<WeightedRandomPrototype> Id;

    public override string Generate(NameSchemeContext context,
        NameSchemeManager nameSchemeMan,
        IPrototypeManager protoMan,
        IRobustRandom random)
    {
        var weightedRandom = protoMan.Index(Id);
        var schemeId = weightedRandom.Pick(random);
        return nameSchemeMan.GenerateName(schemeId, context);
    }
}

/// <summary>
///     Part consists of the result of another NameSchemePrototype.
/// </summary>
public sealed partial class NameSchemePrototypePart : NameSchemePart
{
    [DataField(required: true)]
    public ProtoId<NameSchemePrototype> Id;

    public override string Generate(NameSchemeContext context,
        NameSchemeManager nameSchemeMan,
        IPrototypeManager protoMan,
        IRobustRandom random)
    {
        var scheme = protoMan.Index(Id);
        return nameSchemeMan.GenerateName(scheme, context);
    }
}

/// <summary>
///     Evaluates a NameSchemePart based on the gender of the entity.
/// </summary>
public sealed partial class GenderDependentPart : NameSchemePart
{
    [DataField(required: true)]
    public Dictionary<Gender, NameSchemePart> Parts = new();

    public override string Generate(NameSchemeContext context,
        NameSchemeManager nameSchemeMan,
        IPrototypeManager protoMan,
        IRobustRandom random)
    {
        var part = Parts.GetValueOrDefault(Gender.Neuter);
        var gender = context.Gender ?? Gender.Neuter;

        if (Parts.TryGetValue(gender, out var newPart))
            part = newPart;

        if (part != null)
            return part.Generate(context, nameSchemeMan, protoMan, random);

        throw new KeyNotFoundException($"No valid name part result found for gender: {context.Gender}");
    }
}
