using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities;
using Microsoft.Extensions.Logging;
using System.Data;
using CounterStrikeSharp.API.Modules.Listeners;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Events;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace Message;

public class Msg : BasePlugin, IPluginConfig<Helper>
{
    public override string ModuleName => "Message";
    public override string ModuleAuthor => "The Slynx";
    public override string ModuleDescription => "Message System with logging messages for ADMINS";
    public override string ModuleVersion => "1.1";
    public required Helper Config { get; set; }
    public static IStringLocalizer? _localizer;
    public void OnConfigParsed(Helper config)
    {
        Config = config;
    }

    //Public !msg usage
    [ConsoleCommand("css_msg")]
    [CommandHelper(2, "Usage, /msg <target> + <message>")]
    public void OnPublicMSG(CCSPlayerController? caller, CCSPlayerController? admin, CommandInfo command, CommandInfo command2)
    {
        var callerName = caller == null ? "Console" : caller.PlayerName;

        var targets = GetTarget(command);
        if (targets == null) return;
        var playersToTarget = targets.Players.Where(player => player.IsValid && player.SteamID.ToString().Length == 17 && !player.IsHLTV).ToList();

        var range = command.GetArg(0).Length + command.GetArg(1).Length + 2;
        var message = command.GetCommandString[range..];

        var utf8BytesString = Encoding.UTF8.GetBytes(message);
        var utf8String = Encoding.UTF8.GetString(utf8BytesString);

        playersToTarget.ForEach(player =>
        {
            player.PrintToChat($" {ChatColors.Green}[{ChatColors.White}{callerName} {ChatColors.Green}-> {ChatColors.White}me{ChatColors.Green}] {ChatColors.Yellow}{utf8String}".ReplaceColorTags());
        });

        command.ReplyToCommand($" {ChatColors.Green}[{ChatColors.White}me {ChatColors.Green}-> {ChatColors.White}{command.GetArg(1)}{ChatColors.Green}] {ChatColors.Yellow}{utf8String}".ReplaceColorTags());

        if (admin == null || !admin.IsValid || command2.GetCommandString[command2.GetCommandString.IndexOf(' ')..].Length == 0) return;

        foreach (var player in Helper.GetValidPlayers().Where(p => AdminManager.PlayerHasPermissions(p, "@css/generic")))
        {
            using (new WithTemporaryCulture(player.GetLanguage()))
            {
                player.PrintToChat($" {Config.prefix} {ChatColors.Red}{callerName} {ChatColors.Yellow}-> {ChatColors.Red}{command.GetArg(1)} {ChatColors.Yellow}: {ChatColors.White}{utf8String}".ReplaceColorTags());
            }
        }
    }

    private static TargetResult? GetTarget(CommandInfo command)
    {
        var matches = command.GetArgTargetResult(1);

        if (!matches.Any())
        {
            command.ReplyToCommand($"Player {command.GetArg(1)} doesn't exit.");
            return null;
        }

        if (command.GetArg(1).StartsWith('@'))
            return matches;

        if (matches.Count() == 1)
            return matches;

        command.ReplyToCommand($"More players found: \"{command.GetArg(1)}\".");
        return null;
    }
}
