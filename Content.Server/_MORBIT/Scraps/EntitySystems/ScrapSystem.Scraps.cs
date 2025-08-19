
using System.Linq;
using Content.Shared.Examine;
using Content.Shared.Eye;
using Content.Shared.Localizations;
using Content.Shared.Morbit.Scraps;
using Content.Shared.Morbit.Scraps.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Utility;

namespace Content.Server.Morbit.Scraps.EntitySystems;

public sealed partial class ScrapSystem
{
    [Dependency] private readonly AppearanceSystem _appearance = default!;

    // If you are a scrap, you can always see scraps. I guess
    private void OnScrapGetVisMask(Entity<ScrapComponent> ent, ref GetVisMaskEvent args)
    {
        if (ent.Comp.LifeStage <= ComponentLifeStage.Running)
            args.VisibilityMask |= (int)VisibilityFlags.Scrap;
    }

    private void OnScrapStartup(Entity<ScrapComponent> ent, ref ComponentStartup args)
    {
        AddToScrapLayer(ent);
        UpdateScrapAppearance(ent);
    }

    private void OnScrapShutdown(Entity<ScrapComponent> ent, ref ComponentShutdown args)
    {
        RemoveFromScrapLayer(ent);
    }

    private void OnScrapExamined(Entity<ScrapComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.Motifs.Count == 0)
            return;

        var motifs = new List<string>();
        foreach (var id in ent.Comp.Motifs)
        {
            if (!_prototype.TryIndex(id, out var motif))
                continue;

            var motifString = Loc.GetString("scrap-component-motif",
                ("motif", Loc.GetString(motif.Name)),
                ("color", motif.AssociatedColor.ToHex()));
            motifs.Add(motifString);
        }

        var motifList = ContentLocalizationManager.FormatList(motifs);
        args.PushMarkup(Loc.GetString("scrap-component-examine-text", ("motifs", motifList)));
    }

    private void UpdateScrapAppearance(Entity<ScrapComponent> ent)
    {
        UpdateScrapSprites(ent);
        UpdateScrapColors(ent);
    }

    private void UpdateScrapSprites(Entity<ScrapComponent> ent)
    {
        var motifs = ent.Comp.Motifs;
        if (motifs.Count != 0)
        {
            var firstMotif = motifs.First();
            _appearance.SetData(ent.Owner, ScrapVisuals.MotifId, (string)firstMotif);
        }
    }

    private void UpdateScrapColors(Entity<ScrapComponent> ent)
    {
        var colors = new Dictionary<string, List<string>>();
        foreach (var id in ent.Comp.Motifs)
        {
            if (!_prototype.TryIndex(id, out var motif))
                continue;

            foreach (var layer in motif.ScrapLayers)
                colors.GetOrNew(layer.Key).Add(layer.Color.ToHex());
        }

        // sure whatever
        var motifColors = colors.ToDictionary(p => p.Key, p => string.Join("|", p.Value));
        _appearance.SetData(ent.Owner, ScrapVisuals.Colors, motifColors);
    }
}
