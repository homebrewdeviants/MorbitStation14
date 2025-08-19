using Content.Shared.Eye;
using Content.Shared.Morbit.Scraps.Components;
using JetBrains.Annotations;

namespace Content.Server.Morbit.Scraps.EntitySystems;

public sealed partial class ScrapSystem
{
    private void OnSensitiveGetVisMask(Entity<ScrapSensitiveComponent> ent, ref GetVisMaskEvent args)
    {
        if (HasScrapSensitivity(ent!, ScrapSensitivity.See))
            args.VisibilityMask |= (int)VisibilityFlags.Scrap;
    }

    private void OnSensitiveStartup(Entity<ScrapSensitiveComponent> ent, ref ComponentStartup args)
    {
        _eye.RefreshVisibilityMask(ent.Owner);
    }

    private void OnSensitiveShutdown(Entity<ScrapSensitiveComponent> ent, ref ComponentShutdown args)
    {
        _eye.RefreshVisibilityMask(ent.Owner);
    }

    /// <summary>
    ///     Sets the scrap sensitivity level of an entity. This gives them ScrapSensitiveComponent if they
    ///     do not already have it. If the sensitivity is None, it will remove ScrapSensitiveComponent instead.
    /// </summary>
    /// <param name="uid">The entity to set scrap sensitivity level of.</param>
    /// <param name="sensitivity">The sensitivity level of the entity.</param>
    public void SetScrapSensitivity(EntityUid uid, ScrapSensitivity sensitivity)
    {
        if (sensitivity == ScrapSensitivity.None)
            RemComp<ScrapSensitiveComponent>(uid);
        else
        {
            var sensitive = EnsureComp<ScrapSensitiveComponent>(uid);
            sensitive.Sensitivity = sensitivity;
        }

        _eye.RefreshVisibilityMask(uid);
    }

    /// <summary>
    ///     Check if a given entity has a certain scrap sensitivity level.
    /// </summary>
    /// <param name="ent">The entity to check.</param>
    /// <param name="flags">The sensitivity level to check.</param>
    /// <returns>Whether or not the entity has this level of scrap sensitivity.</returns>
    [PublicAPI]
    public bool HasScrapSensitivity(Entity<ScrapSensitiveComponent?> ent, ScrapSensitivity flags)
        => Resolve(ent, ref ent.Comp, false) && (ent.Comp.Sensitivity & flags) != 0;
}
