using System.Linq;
using Content.Shared.Database;
using Content.Shared.Morbit.Motifs;
using Content.Shared.Morbit.Scraps.Components;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Morbit.Scraps.EntitySystems;

public sealed partial class ScrapSystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    private static readonly ProtoId<WeightedRandomPrototype> DefaultPool = "AnyRandomMotif";
    private IEnumerable<string>? _motifs = null;

    private void OnProducerStartup(Entity<ScrapProducerComponent> ent, ref ComponentStartup args)
    {
        if (ent.Comp.Motifs.Count == 0)
            GiveRandomMotifs(ent);
    }

    /// <summary>
    ///     Give a scrap producer entity a number of random motifs. The amount of motifs they actually receive will
    ///     always be hard-capped by ScrapProducerComponent.MaximumMotifs. For example, an entity with 0 motifs and 2
    ///     maximum motifs will be able to receive at most two new random motifs.
    /// </summary>
    /// <param name="ent">The entity to receive new motifs.</param>
    /// <param name="replace">Whether or not the old motif list should be cleared.</param>
    /// <param name="count">How many new motifs to give. Default will fill motif count to maximum.</param>
    /// <param name="pool">The weighted random list of motifs to choose from. Default is AnyRandomMotif.</param>
    [PublicAPI]
    public void GiveRandomMotifs(Entity<ScrapProducerComponent> ent,
        bool replace = true,
        int? count = null,
        ProtoId<WeightedRandomPrototype> pool = default)
    {
        var producer = ent.Comp;

        // Not adding any motifs, or we're not replacing them and we have enough
        if (count != null && count <= 0
            || !replace && producer.MaximumMotifs <= producer.Motifs.Count)
            return;

        if (replace)
        {
            producer.Motifs.Clear();
            if (producer.MaximumMotifs <= 0)
                return;
        }

        var motifsToAdd = producer.MaximumMotifs - producer.Motifs.Count;
        if (motifsToAdd <= 0)
            return;

        if (count != null)
            motifsToAdd = Math.Clamp(count.Value, 0, motifsToAdd);

        if (pool == default)
            pool = DefaultPool;

        if (!_prototype.TryIndex(pool, out var motifs))
            return;

        AddRandomMotifs(ent, motifs, motifsToAdd);
    }

    private void AddRandomMotifs(Entity<ScrapProducerComponent> ent, WeightedRandomPrototype motifs, int count = 1)
    {
        var weights = motifs.Weights.ShallowClone()
            .Where(w => !ent.Comp.Motifs.Contains(w.Key))
            .ToDictionary();

        for (var i = 0; i < count; i++)
        {
            // No motifs left to take
            if (!_random.TryPickAndTake(weights, out var motif))
                return;

            TryAddMotif(ent, motif);
        }
    }

    /// <summary>
    ///     Attempt to add several motifs to the entity. If any of the motifs cannot be added, none of them will
    ///     be added.
    /// </summary>
    /// <param name="ent">The entity to add motifs to.</param>
    /// <param name="motifs">The motifs to add.</param>
    /// <param name="replace">Whether or not the current motif list of the entity should be cleared first.</param>
    /// <param name="force">Whether the maximum motif count of the entity should be increased first.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryAddMotifs(Entity<ScrapProducerComponent> ent,
        IEnumerable<ProtoId<MotifPrototype>> motifs,
        bool replace = true,
        bool force = false)
    {
        var producer = ent.Comp;
        if (replace)
            ent.Comp.Motifs.Clear();

        var slotsLeft = producer.MaximumMotifs - producer.Motifs.Count;
        var motifsToAdd = motifs.Count();

        if (!force && slotsLeft < motifsToAdd)
            return false;

        var oldMax = ent.Comp.MaximumMotifs;
        if (force)
            ent.Comp.MaximumMotifs = producer.Motifs.Count + motifsToAdd;

        foreach (var motif in motifs)
            if (!CanAddMotif(ent, motif))
            {
                ent.Comp.MaximumMotifs = oldMax;
                return false;
            }

        foreach (var motif in motifs)
            AddMotif(ent, motif);

        return true;
    }

    private bool CanAddMotif(Entity<ScrapProducerComponent> ent, ProtoId<MotifPrototype> motif)
        => !ent.Comp.Motifs.Contains(motif)
            && ent.Comp.MaximumMotifs - ent.Comp.Motifs.Count >= 1
            && _prototype.TryIndex(motif, out var _);

    /// <summary>
    ///     Attempt to add a motif to an entity.
    /// </summary>
    /// <param name="ent">The entity to add a motif to.</param>
    /// <param name="motif">The motif to add.</param>
    /// <returns>Whether or not this operation was successful.</returns>
    public bool TryAddMotif(Entity<ScrapProducerComponent> ent, ProtoId<MotifPrototype> motif)
    {
        if (!CanAddMotif(ent, motif))
            return false;

        AddMotif(ent, motif);
        return true;
    }

    private void AddMotif(Entity<ScrapProducerComponent> ent, ProtoId<MotifPrototype> motif)
    {
        ent.Comp.Motifs.Add(motif);
        var message = $"\"{motif}\" motif was added to {ToPrettyString(ent)}.";
        _adminLogger.Add(LogType.Scrap, LogImpact.Low, $"{message}");
        _sawmill.Debug(message);
    }

    /// <summary>
    ///     Gets an enumerable list of Motif prototypes in string form, for command completions.
    /// </summary>
    /// <returns>An enumerable container of Motif prototype IDs.</returns>
    public IEnumerable<string> GetMotifCompletions()
    {
        if (_motifs == null)
            UpdateMotifs();

        return _motifs!;
    }

    private void UpdateMotifs()
    {
        _motifs = _prototype.EnumeratePrototypes<MotifPrototype>()
            .Select(m => m.ID);
    }
}
