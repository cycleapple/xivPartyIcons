using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using PartyIcons.Entities;
using PartyIcons.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PartyIcons.UI.Settings;

public sealed class StaticAssignmentsTab
{
    private string _occupationNewName = "角色名稱@伺服器";
    private RoleId _occupationNewRole = RoleId.Undefined;

    public void Draw()
    {
        ImGui.Dummy(new Vector2(0, 2f));
        
        var easternNamingConvention = Plugin.Settings.EasternNamingConvention;

        if (ImGui.Checkbox("##easteannaming", ref easternNamingConvention))
        {
            Plugin.Settings.EasternNamingConvention = easternNamingConvention;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGui.Text("使用日式站位命名");
        ImGuiComponents.HelpMarker("使用日本資料中心常見的站位命名（MT、ST、D1～D4、H1～H2）。");

        var displayRoleInPartyList = Plugin.Settings.DisplayRoleInPartyList;

        if (ImGui.Checkbox("##displayrolesinpartylist", ref displayRoleInPartyList))
        {
            Plugin.Settings.DisplayRoleInPartyList = displayRoleInPartyList;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGui.Text("在隊伍列表中以站位取代隊伍編號");
        ImGuiExt.ImGuiHelpTooltip(
            "僅在名牌設為「站位代號」或「小型職業圖示、站位與名稱」時生效。");

        var useContextMenu = Plugin.Settings.UseContextMenu;
        
        if (ImGui.Checkbox("##useContextMenu", ref useContextMenu))
        {
            Plugin.Settings.UseContextMenu = useContextMenu;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGui.Text("在右鍵選單中加入站位指派指令");
        ImGuiComponents.HelpMarker("在玩家的右鍵選單中加入站位指派指令；適用時，也會加入交換站位與採用建議站位的指令。");

        var useContextMenuStatic = Plugin.Settings.UseContextMenuStatic;

        if (ImGui.Checkbox("##useContextMenuStatic", ref useContextMenuStatic))
        {
            Plugin.Settings.UseContextMenuStatic = useContextMenuStatic;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGui.Text("在右鍵選單中加入固定站位儲存指令");

        var useContextMenuSubmenu = Plugin.Settings.UseContextMenuSubmenu;

        if (ImGui.Checkbox("##useContextMenuSubmenu", ref useContextMenuSubmenu))
        {
            Plugin.Settings.UseContextMenuSubmenu = useContextMenuSubmenu;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGui.Text("將已啟用的右鍵選單指令放入專用子選單");

        var assignFromChat = Plugin.Settings.AssignFromChat;

        if (ImGui.Checkbox("##assignFromChat", ref assignFromChat))
        {
            Plugin.Settings.AssignFromChat = assignFromChat;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGui.Text("允許隊員透過隊伍頻道自行指派站位");
        ImGuiComponents.HelpMarker("允許隊員自行指派站位，例如在隊伍頻道輸入「h1」，便會將該玩家指派為 H1。");
        
        ImGui.Dummy(new Vector2(0, 2f));
        ImGui.PushStyleColor(0, ImGuiHelpers.DefaultColorPalette()[0]);
        ImGui.Text("固定站位");
        ImGui.PopStyleColor();
        ImGui.Separator();
        ImGui.Dummy(new Vector2(0, 2f));
        {
            ImGui.PushStyleColor(0, ImGuiColors.ParsedGrey);
            {
                ImGui.TextWrapped(
                    "名稱須包含伺服器名稱，並以 @ 分隔。若玩家職業不符合指定站位，該指派會被忽略！");
                ImGui.Dummy(new Vector2(0f, 25f));
            }
            ImGui.PopStyleColor();
        }
        
        ImGui.SetCursorPosY(ImGui.GetCursorPos().Y - 22f);
        foreach (var kv in new Dictionary<string, RoleId>(Plugin.Settings.StaticAssignments))
        {
            if (ImGui.Button("x##remove_occupation_" + kv.Key))
            {
                Plugin.Settings.StaticAssignments.Remove(kv.Key);
                Plugin.Settings.Save();

                continue;
            }

            ImGui.SameLine();
            ImGuiExt.SetComboWidth(Enum.GetValues<RoleId>().Select(x => Plugin.PlayerStylesheet.GetRoleName(x)));

            if (ImGui.BeginCombo("##role_combo_" + kv.Key,
                    Plugin.PlayerStylesheet.GetRoleName(Plugin.Settings.StaticAssignments[kv.Key])))
            {
                foreach (var roleId in Enum.GetValues<RoleId>())
                {
                    if (ImGui.Selectable(Plugin.PlayerStylesheet.GetRoleName(roleId) + "##role_combo_option_" + kv.Key + "_" +
                                         roleId))
                    {
                        Plugin.Settings.StaticAssignments[kv.Key] = roleId;
                        Plugin.Settings.Save();
                    }
                }

                ImGui.EndCombo();
            }

            ImGui.SameLine();
            ImGui.Text(kv.Key);
        }

        if (ImGui.Button("+##add_occupation"))
        {
            Plugin.Settings.StaticAssignments[_occupationNewName] = _occupationNewRole;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGuiExt.SetComboWidth(Enum.GetValues<RoleId>().Select(x => Plugin.PlayerStylesheet.GetRoleName(x)));

        if (ImGui.BeginCombo("##new_role_combo", Plugin.PlayerStylesheet.GetRoleName(_occupationNewRole)))
        {
            foreach (var roleId in Enum.GetValues<RoleId>())
            {
                if (ImGui.Selectable(Plugin.PlayerStylesheet.GetRoleName(roleId) + "##new_role_combo_option_" + "_" + roleId))
                {
                    _occupationNewRole = roleId;
                }
            }

            ImGui.EndCombo();
        }

        ImGui.SameLine();
        ImGui.InputText("##new_role_name", ref _occupationNewName, 64);
        
        ImGui.SetCursorPosY(ImGui.GetCursorPos().Y + 22f);

    }
}
