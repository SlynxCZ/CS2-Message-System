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
using System.Text.Json.Serialization;

namespace Message;

public class Helper : BasePluginConfig
{
    public static List<CCSPlayerController> GetValidPlayers()
    {
        return Utilities.GetPlayers().FindAll(p => p is
        { IsBot: false, IsHLTV: false });
    }

    [JsonPropertyName("Prefix")] public string prefix { get; set; } = $" {ChatColors.Yellow}[{ChatColors.Green}MSG LOG{ChatColors.Yellow}]";
}