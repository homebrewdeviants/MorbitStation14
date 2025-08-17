using System.Linq;
using Content.Client.Morbit.Scraps.Components;
using Content.Shared.Morbit.Scraps;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client.Morbit.Scraps.EntitySystems;

public sealed class ScrapVisualizerSystem : VisualizerSystem<ScrapVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid,
        ScrapVisualsComponent component,
        ref AppearanceChangeEvent args)
    {
        UpdateAppearance(uid, component);
    }

    private void UpdateAppearance(EntityUid uid, ScrapVisualsComponent component)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite))
            return;

        var spriteEnt = (uid, sprite);
        UpdateSprites(spriteEnt, component);
        UpdateColors(spriteEnt, component);
    }

    private void UpdateSprites(Entity<SpriteComponent?> ent, ScrapVisualsComponent component)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, false)
            || !AppearanceSystem.TryGetData<string>(ent.Owner, ScrapVisuals.Rsi, out var rsi)
            || !AppearanceSystem.TryGetData<string>(ent.Owner, ScrapVisuals.State, out var state))
            return;

        var rsiPath = new ResPath(rsi);

        SpriteSystem.LayerSetRsi(ent,
            key: component.BaseKey,
            rsi: rsiPath,
            state: state + component.BaseLayerSuffix);

        SpriteSystem.LayerSetRsi(ent,
            key: component.HighlightKey,
            rsi: rsiPath,
            state: state + component.HighlightLayerSuffix);
    }

    private void UpdateColors(Entity<SpriteComponent?> ent, ScrapVisualsComponent component)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, false))
            return;

        if (AppearanceSystem.TryGetData<List<Color>>(ent.Owner, ScrapVisuals.Colors, out var colors))
        {
            var color = GetScrapColor(colors, component.ColorInterpolationAmount);
            SpriteSystem.LayerSetColor(ent, key: component.BaseKey, color);
        }

        if (AppearanceSystem.TryGetData<List<Color>>(ent.Owner, ScrapVisuals.OutlineColors, out var outlineColors))
        {
            var outlineColor = GetScrapColor(outlineColors, component.ColorInterpolationAmount);
            SpriteSystem.LayerSetColor(ent, key: component.OutlineKey, outlineColor);
        }

    }

    private static Color GetScrapColor(IEnumerable<Color> motifColors, float interpolationAmount = 0.4f)
    {
        if (!motifColors.Any())
            return Color.White;

        Color color = motifColors.First();
        foreach (var motifColor in motifColors.Skip(1))
            color = Color.InterpolateBetween(color, motifColor, interpolationAmount);

        return color;
    }
}
