
using Content.Shared.Eye;
using Content.Shared.Morbit.Scraps.Components;

namespace Content.Server.Morbit.Scraps.EntitySystems;

public sealed partial class ScrapSystem
{
    // If you are a scrap, you can always see scraps. I guess
    private void OnScrapGetVisMask(Entity<ScrapComponent> ent, ref GetVisMaskEvent args)
    {
        if (ent.Comp.LifeStage <= ComponentLifeStage.Running)
            args.VisibilityMask |= (int)VisibilityFlags.Scrap;
    }

    private void OnScrapStartup(Entity<ScrapComponent> ent, ref ComponentStartup args)
    {
        AddToScrapLayer(ent);
    }

    private void OnScrapShutdown(Entity<ScrapComponent> ent, ref ComponentShutdown args)
    {
        RemoveFromScrapLayer(ent);
    }
}
