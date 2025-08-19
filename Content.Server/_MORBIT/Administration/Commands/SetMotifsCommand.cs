using System.Linq;
using Content.Server.Administration;
using Content.Server.Morbit.Scraps.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Morbit.Motifs;
using Content.Shared.Morbit.Scraps.Components;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Morbit.Administration.Commands;

[AdminCommand(AdminFlags.VarEdit)]
public sealed class SetMotifsCommand : LocalizedEntityCommands
{
    [Dependency] private readonly IEntityManager _entities = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly ScrapSystem _scrap = default!;

    public override string Command => "setmotifs";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 1)
        {
            shell.WriteLine(Help);
            return;
        }

        if (!int.TryParse(args[0], out var targetId))
        {
            shell.WriteError(Loc.GetString("shell-argument-must-be-number"));
            return;
        }

        var targetNet = new NetEntity(targetId);

        if (!_entities.TryGetEntity(targetNet, out var target))
        {
            shell.WriteError(Loc.GetString("shell-invalid-entity-id"));
            return;
        }

        if (!_entities.TryGetComponent<ScrapProducerComponent>(target, out var producer))
        {
            shell.WriteError(Loc.GetString("cmd-setmotifs-error-not-producer",
                ("entity", _entities.ToPrettyString(target.Value))));
            return;
        }

        var motifs = args.Skip(1).Select(m => (ProtoId<MotifPrototype>)m);
        if (!_scrap.TryAddMotifs((target.Value, producer), motifs, replace: true, force: true))
        {
            shell.WriteError(Loc.GetString("cmd-setmotifs-error-invalid-motif"));
            return;
        }

        shell.WriteLine(Loc.GetString("cmd-setmotifs-success",
            ("entity", _entities.ToPrettyString(target.Value)),
            ("motifs", string.Join(", ", motifs))));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
            return CompletionResult.FromOptions(CompletionHelper.NetEntities(args[0], entManager: _entities));

        return CompletionResult.FromOptions(_scrap.GetMotifCompletions());
    }
}
