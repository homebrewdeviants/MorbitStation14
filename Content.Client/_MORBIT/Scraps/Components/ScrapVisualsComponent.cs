namespace Content.Client.Morbit.Scraps.Components;

[RegisterComponent]
public sealed partial class ScrapVisualsComponent : Component
{
    [DataField]
    public string BaseLayerSuffix = "-base";

    [DataField]
    public string HighlightLayerSuffix = "-highlight";

    [DataField]
    public string BaseKey = "base";

    [DataField]
    public string HighlightKey = "highlight";

    [DataField]
    public string OutlineKey = "outline";

    [DataField]
    public float ColorInterpolationAmount = 0.4f;
}
