using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Dataset;
using Content.Shared.Random.Helpers;
using Robust.Shared.Random;
using Robust.Shared.Prototypes;
using Robust.Shared.Enums;
using Content.Shared.Morbit.Names;

namespace Content.Shared.Humanoid
{
    /// <summary>
    /// Figure out how to name a humanoid with these extensions.
    /// </summary>
    public sealed class NamingSystem : EntitySystem
    {
        private static readonly ProtoId<SpeciesPrototype> FallbackSpecies = "Human";

        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly NameSchemeManager _nameScheme = default!;

        public string GetName(string species, Gender? gender = null)
        {
            // if they have an old species or whatever just fall back to human I guess?
            // Some downstream is probably gonna have this eventually but then they can deal with fallbacks.
            if (!_prototypeManager.TryIndex(species, out SpeciesPrototype? speciesProto))
            {
                speciesProto = _prototypeManager.Index(FallbackSpecies);
                Log.Warning($"Unable to find species {species} for name, falling back to {FallbackSpecies}");
            }

            // MORBIT: NameScheme system
            return GenerateName(speciesProto, gender);
        }

        /// <summary>
        ///     MORBIT: Generate a name based on the species's nameScheme.
        /// </summary>
        /// <param name="species">The species prototype.</param>
        /// <param name="gender">The gender of the character.</param>
        /// <returns>A random character name.</returns>
        private string GenerateName(SpeciesPrototype species, Gender? gender = null)
        {
            var nameScheme = species.NameScheme;
            var datasets = species.NameDatasets;

            var context = new NameSchemeContext()
            {
                Datasets = datasets,
                Gender = gender,
            };

            return _nameScheme.GenerateName(nameScheme, context);
        }
    }
}
