using Content.Shared.Humanoid.Markings;
using Content.Shared.Localizations;
using Content.Shared.Morbit.Names;

namespace Content.Shared.IoC
{
    public static class SharedContentIoC
    {
        public static void Register(IDependencyCollection deps)
        {
            IoCManager.Register<MarkingManager, MarkingManager>();
            IoCManager.Register<ContentLocalizationManager, ContentLocalizationManager>();

            // MORBIT
            IoCManager.Register<NameSchemeManager, NameSchemeManager>();
            // END MORBIT
            deps.Register<MarkingManager, MarkingManager>();
            deps.Register<ContentLocalizationManager, ContentLocalizationManager>();
        }
    }
}
