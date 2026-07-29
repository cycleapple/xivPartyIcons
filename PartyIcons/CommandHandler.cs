using System;
using Dalamud.Game.Command;
using PartyIcons.Runtime;

namespace PartyIcons;

public class CommandHandler : IDisposable
{
    private const string commandName = "/ppi";
    
    public CommandHandler()
    {
        Service.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage =
                "開啟設定視窗；輸入「reset」或「r」可重設所有站位指派"
        });
    }
    
    public void Dispose()
    {
        Service.CommandManager.RemoveHandler(commandName);
    }

    private void OnCommand(string command, string arguments)
    {
        arguments = arguments.Trim().ToLower();

        if (arguments == "" || arguments == "config")
        {
            Plugin.WindowManager.ToggleSettings();
        }
        else if (arguments == "reset" || arguments == "r")
        {
            Plugin.RoleTracker.ResetOccupations();
            Plugin.RoleTracker.ResetAssignments();
            Plugin.RoleTracker.CalculateUnassignedPartyRoles();
            Service.ChatGui.Print("已重設已佔用站位，並自動重新指派站位。", Service.PluginInterface.InternalName, 45);
        }
        else if (arguments == "dbg r")
        {
            Plugin.RoleTracker.ResetOccupations();
            Plugin.RoleTracker.ResetAssignments();
            Service.ChatGui.Print("已重設已佔用站位與站位指派。", Service.PluginInterface.InternalName, 45);
        }
        else if (arguments == "dbg state")
        {
            Service.Log.Info($"Current mode is {Plugin.NameplateView.PartyDisplay.Mode}, party count {Service.PartyList.Length}", Service.PluginInterface.InternalName, 45);
            Service.Log.Info(Plugin.RoleTracker.DebugDescription(), Service.PluginInterface.InternalName, 45);
        }
        else if (arguments == "dbg party")
        {
            PartyListHUDUpdater.DebugPartyData();
        }
    }
}
