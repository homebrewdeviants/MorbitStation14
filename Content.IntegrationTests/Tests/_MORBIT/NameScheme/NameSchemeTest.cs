using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Morbit.Names.Prototypes;
using Robust.Shared.Enums;

namespace Content.IntegrationTests.Tests.Morbit.NameScheme;

[TestFixture]
[TestOf(typeof(NameSchemePrototype))]
public sealed class NameSchemeTest
{
    const int NamesPerSpeciesAndGender = 3;

    /// <summary>
    ///     Tests if all species can generate names without error.
    /// </summary>
    [Test]
    public async Task AllSpeciesHaveValidNameScheme()
    {
        await using var pair = await PoolManager.GetServerClient();
        var server = pair.Server;
        var entManager = server.EntMan;
        var protoMan = server.ProtoMan;
        var compFactory = server.EntMan.ComponentFactory;
        var namingSystem = entManager.System<NamingSystem>();

        await server.WaitAssertion(() =>
        {
            Assert.Multiple(() =>
            {
                var protos = protoMan.EnumeratePrototypes<SpeciesPrototype>();
                foreach (var species in protos)
                    GenerateNames(species, namingSystem);
            });
        });

        await pair.CleanReturnAsync();
    }

    private static void GenerateNames(SpeciesPrototype species, NamingSystem namingSystem)
    {
        for (var i = 0; i < NamesPerSpeciesAndGender; i++)
        {
            namingSystem.GenerateName(species, null);
            foreach (var gender in Enum.GetValues<Gender>())
                namingSystem.GenerateName(species, gender);
        }
    }
}
