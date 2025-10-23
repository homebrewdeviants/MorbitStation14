using Content.Shared.Humanoid.Markings;
using Content.Shared.Localizations;
using Content.Shared.Morbit.Names;

namespace Content.Shared.IoC
{
    public static class SharedContentIoC
    {
        public static void Register(IDependencyCollection deps)
        {
            deps.Register<MarkingManager, MarkingManager>();
            deps.Register<ContentLocalizationManager, ContentLocalizationManager>();

            // MORBIT
            deps.Register<NameSchemeManager, NameSchemeManager>();
            // END MORBIT
        }
    }
}
