using Content.Server.Administration;
using Content.Server.Morbit.Scraps.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Morbit.Scraps.Components;
using Robust.Shared.Console;
using Robust.Shared.Utility;

namespace Content.Server.Morbit.Administration.Commands;

[AdminCommand(AdminFlags.VarEdit)]
public sealed class SetScrapSensitivityCommand : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entities = default!;

    public string Command => "setscrapsensitivity";
    public string Description => Loc.GetString("command-admin-set-scrap-sensitivity-description");
    public string Help => Loc.GetString("command-admin-set-scrap-sensitivity-help", ("command", Command));

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
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

        var sensitivity = ScrapSensitivity.All;

        if (args.TryGetValue(1, out var arg))
        {
            if (!Enum.TryParse(typeof(ScrapSensitivity), arg, ignoreCase: true, out var parsed))
            {
                shell.WriteError(Loc.GetString("command-admin-set-scrap-sensitivity-error-invalid-sensitivity"));
                return;
            }

            sensitivity = (ScrapSensitivity)parsed!;
        }

        var scrapSystem = _entities.System<ScrapSystem>();
        scrapSystem.SetScrapSensitivity(target.Value, sensitivity);
        shell.WriteLine(Loc.GetString("command-admin-set-scrap-sensitivity-success",
            ("entity", _entities.ToPrettyString(target.Value)),
            ("sensitivity", sensitivity.ToString())));
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
            return CompletionResult.FromOptions(CompletionHelper.NetEntities(args[0], entManager: _entities));

        if (args.Length == 2)
            return CompletionResult.FromOptions(Enum.GetNames<ScrapSensitivity>());

        return CompletionResult.Empty;
    }
}
