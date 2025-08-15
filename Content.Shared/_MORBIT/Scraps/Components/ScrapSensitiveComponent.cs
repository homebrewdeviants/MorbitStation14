using Content.Shared.Morbit.Scraps.EntitySystems;

namespace Content.Shared.Morbit.Scraps.Components;

/// <summary>
///     Scrap-sensitive entities can see and/or interact with scraps without the need for tools.
/// </summary>
[RegisterComponent]
[Access(typeof(SharedScrapSystem))]
public sealed partial class ScrapSensitiveComponent : Component
{
    [DataField]
    public ScrapSensitivity Sensitivity = ScrapSensitivity.None;
}

[Flags]
public enum ScrapSensitivity : byte
{
    /// <summary>
    /// This entity cannot detect scraps.
    /// </summary>
    None = 0,

    /// <summary>
    /// This entity can detect when there is high scrap activity nearby, as a flavor text message.
    /// </summary>
    Feel = 1 << 1,

    /// <summary>
    /// This entity can see scrap entities.
    /// </summary>
    See = 1 << 2,

    /// <summary>
    /// This entity can pick up scrap entities.
    /// </summary>
    Touch = 1 << 3,

    /// <summary>
    /// This entity can detect high scrap activity and see scrap entities.
    /// </summary>
    FeelAndSee = Feel | See,

    /// <summary>
    /// This entity can see scrap entities and pick them up.
    /// </summary>
    SeeAndTouch = See | Touch,

    /// <summary>
    /// This entity can detect high scrap activity, see scrap entities, and pick them up.
    /// </summary>
    All = Feel | See | Touch,
}
