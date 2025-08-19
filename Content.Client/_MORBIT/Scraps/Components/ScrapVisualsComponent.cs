namespace Content.Client.Morbit.Scraps.Components;

[RegisterComponent]
public sealed partial class ScrapVisualsComponent : Component
{
    [DataField]
    public float ColorInterpolationAmount = 0.4f;

    public string LastMotifId = string.Empty;
}
