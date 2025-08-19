using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Content.Client.Morbit.Scraps.Components;
using Content.Shared.Morbit.Motifs;
using Content.Shared.Morbit.Scraps;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Morbit.Scraps.EntitySystems;

public sealed class ScrapVisualizerSystem : VisualizerSystem<ScrapVisualsComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

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
            || !AppearanceSystem.TryGetData<string>(ent.Owner, ScrapVisuals.MotifId, out var motifId)
            || component.LastMotifId == motifId
            || !_prototype.TryIndex<MotifPrototype>(motifId, out var motif))
            return;

        var layerCount = ent.Comp.AllLayers.Count();
        for (var i = 0; i < layerCount; i++)
            SpriteSystem.RemoveLayer(ent, 0, false);

        var index = 0;
        foreach (var layer in motif.ScrapLayers)
        {
            var data = new PrototypeLayerData()
            {
                RsiPath = layer.Sprite,
                State = $"{layer.State}-{layer.Key}",
                MapKeys = [layer.Key],
                Shader = layer.Shader,
            };

            SpriteSystem.AddBlankLayer(ent!, index);
            SpriteSystem.LayerSetData(ent, index, data);
            index++;
        }

        component.LastMotifId = motifId;
    }

    private void UpdateColors(Entity<SpriteComponent?> ent, ScrapVisualsComponent component)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, false))
            return;

        if (AppearanceSystem.TryGetData<Dictionary<string, string>>(ent.Owner,
            ScrapVisuals.Colors,
            out var colors))
        {
            var motifColors = colors
                .ToDictionary(p => p.Key, p => p.Value
                    .Split("|")
                    .Select(c => Color.FromHex(c)));

            foreach (var pair in motifColors)
            {
                var color = GetScrapColor(motifColors: pair.Value,
                    interpolationAmount: component.ColorInterpolationAmount);
                SpriteSystem.LayerSetColor(ent, key: pair.Key, color);
            }
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
