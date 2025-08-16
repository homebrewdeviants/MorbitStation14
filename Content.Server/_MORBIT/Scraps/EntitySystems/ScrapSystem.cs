using Content.Server.Administration.Logs;
using Content.Shared.Eye;
using Content.Shared.Morbit.Scraps.Components;
using Content.Shared.Morbit.Scraps.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Server.Morbit.Scraps.EntitySystems;

public sealed partial class ScrapSystem : SharedScrapSystem
{
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly SharedEyeSystem _eye = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly VisibilitySystem _visibility = default!;

    private readonly ISawmill _sawmill = Logger.GetSawmill("ScrapSystem");

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ScrapComponent, GetVisMaskEvent>(OnScrapGetVisMask);
        SubscribeLocalEvent<ScrapComponent, ComponentStartup>(OnScrapStartup);
        SubscribeLocalEvent<ScrapComponent, ComponentShutdown>(OnScrapShutdown);

        SubscribeLocalEvent<ScrapProducerComponent, ComponentStartup>(OnProducerStartup);

        SubscribeLocalEvent<ScrapSensitiveComponent, GetVisMaskEvent>(OnSensitiveGetVisMask);
        SubscribeLocalEvent<ScrapSensitiveComponent, ComponentStartup>(OnSensitiveStartup);
        SubscribeLocalEvent<ScrapSensitiveComponent, ComponentShutdown>(OnSensitiveShutdown);
    }

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

    private void AddToScrapLayer(EntityUid uid)
    {
        if (Terminating(uid))
            return;

        var visibility = EnsureComp<VisibilityComponent>(uid);
        var visEnt = (uid, visibility);
        _visibility.RemoveLayer(visEnt, (int)VisibilityFlags.Normal, refresh: false);
        _visibility.AddLayer(visEnt, (int)VisibilityFlags.Scrap, refresh: false);
        _visibility.RefreshVisibility(uid, visibilityComponent: visibility);

        _eye.RefreshVisibilityMask(uid);
    }

    private void RemoveFromScrapLayer(EntityUid uid)
    {
        if (TryComp<VisibilityComponent>(uid, out var visibility))
        {
            var visEnt = (uid, visibility);
            _visibility.RemoveLayer(visEnt, (int)VisibilityFlags.Scrap, refresh: false);
            _visibility.AddLayer(visEnt, (int)VisibilityFlags.Normal, refresh: false);
            _visibility.RefreshVisibility(uid, visibilityComponent: visibility);
        }

        _eye.RefreshVisibilityMask(uid);
    }
}
