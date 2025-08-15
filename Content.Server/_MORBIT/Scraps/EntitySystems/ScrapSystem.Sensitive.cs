using Content.Shared.Eye;
using Content.Shared.Morbit.Scraps.Components;

namespace Content.Server.Morbit.Scraps.EntitySystems;

public sealed partial class ScrapSystem
{
    private void OnSensitiveGetVisMask(Entity<ScrapSensitiveComponent> ent, ref GetVisMaskEvent args)
    {
        if ((ent.Comp.Sensitivity & ScrapSensitivity.See) != 0)
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
}
