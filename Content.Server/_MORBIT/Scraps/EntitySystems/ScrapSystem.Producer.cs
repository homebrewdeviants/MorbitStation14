using System.Linq;
using Content.Shared.Database;
using Content.Shared.Morbit.Motifs;
using Content.Shared.Morbit.Scraps.Components;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Morbit.Scraps.EntitySystems;

public sealed partial class ScrapSystem
{
    private void OnProducerStartup(Entity<ScrapProducerComponent> ent, ref ComponentStartup args)
    {
        if (ent.Comp.Motifs.Count == 0)
            GiveRandomMotifs(ent);
    }
}
