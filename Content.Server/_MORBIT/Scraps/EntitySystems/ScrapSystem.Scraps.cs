
using System.Linq;
using Content.Shared.Eye;
using Content.Shared.Morbit.Scraps;
using Content.Shared.Morbit.Scraps.Components;
using Robust.Server.GameObjects;

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

    private void UpdateScrapAppearance(Entity<ScrapComponent> ent)
    {
        var motifs = ent.Comp.Motifs;

        if (motifs.Any())
        {
            var firstMotif = motifs.First();
            if (_prototype.TryIndex(firstMotif, out var motif))
            {
                _appearance.SetData(ent.Owner, ScrapVisuals.Rsi, motif.ScrapProperties.Sprite);
                _appearance.SetData(ent.Owner, ScrapVisuals.State, motif.ScrapProperties.State);
            }
        }

        var colors = new List<Color>();
        var outlineColors = new List<Color>();
        foreach (var id in motifs)
        {
            if (!_prototype.TryIndex(id, out var motif))
                continue;

            colors.Add(motif.ScrapProperties.BaseColor);
            outlineColors.Add(motif.ScrapProperties.OutlineColor);
        }

        _appearance.SetData(ent.Owner, ScrapVisuals.Colors, colors);
        _appearance.SetData(ent.Owner, ScrapVisuals.OutlineColors, outlineColors);
    }
}
