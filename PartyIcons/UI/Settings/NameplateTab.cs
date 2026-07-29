using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using PartyIcons.Configuration;
using PartyIcons.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace PartyIcons.UI.Settings;

public sealed class NameplateTab
{
    private readonly Dictionary<NameplateMode, IDalamudTextureWrap> _nameplateExamples = new();

    public NameplateTab()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var examplesImageNames = new Dictionary<NameplateMode, string>
        {
            { NameplateMode.SmallJobIcon, "PartyIcons.Resources.1.png" },
            { NameplateMode.BigJobIcon, "PartyIcons.Resources.2.png" },
            { NameplateMode.BigJobIconAndPartySlot, "PartyIcons.Resources.3.png" },
            { NameplateMode.RoleLetters, "PartyIcons.Resources.4.png" }
        };

        foreach (var kv in examplesImageNames) {
            using var fileStream = assembly.GetManifestResourceStream(kv.Value);

            if (fileStream == null) {
                Service.Log.Error($"Failed to get resource stream for {kv.Value}");

                continue;
            }

            _nameplateExamples[kv.Key] = Service.TextureProvider.CreateFromImageAsync(fileStream).Result;
        }
    }

    public void Draw()
    {
        ImGui.Dummy(new Vector2(0, 2f));

        ImGui.Text("圖示組：");
        ImGui.SameLine();
        ImGuiExt.SetComboWidth(Enum.GetValues<IconSetId>().Select(UiNames.GetName));

        ImGuiExt.DrawIconSetCombo("##icon_set", false, () => Plugin.Settings.IconSetId, iconSetId =>
        {
            Plugin.Settings.IconSetId = iconSetId;
            Plugin.Settings.Save();
        });

        var iconSizeMode = Plugin.Settings.SizeMode;
        ImGui.Text("名牌大小：");
        ImGui.SameLine();
        ImGuiExt.SetComboWidth(Enum.GetValues<NameplateSizeMode>().Select(UiNames.GetName));

        using (var combo = ImRaii.Combo("##icon_size", UiNames.GetName(iconSizeMode))) {
            if (combo) {
                foreach (var mode in Enum.GetValues<NameplateSizeMode>()) {
                    if (ImGui.Selectable(UiNames.GetName(mode) + "##icon_set_" + mode)) {
                        Plugin.Settings.SizeMode = mode;
                        Plugin.Settings.Save();
                    }
                }
            }
        }

        ImGuiComponents.HelpMarker("影響「遊戲預設」以外的所有預設。");

        if (Plugin.Settings.SizeMode == NameplateSizeMode.Custom) {
            var scale = Plugin.Settings.SizeModeCustom;
            if (ImGui.SliderFloat("自訂縮放", ref scale, 0.3f, 3f)) {
                Plugin.Settings.SizeModeCustom = Math.Clamp(scale, 0.1f, 10f);
                Plugin.Settings.Save();
            }

            ImGuiComponents.HelpMarker("按住 Ctrl 並點擊滑桿，可輸入精確數值。");
        }

        var hideLocalNameplate = Plugin.Settings.HideLocalPlayerNameplate;
        if (ImGui.Checkbox("##hidelocal", ref hideLocalNameplate)) {
            Plugin.Settings.HideLocalPlayerNameplate = hideLocalNameplate;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGui.Text("隱藏自己的名牌");
        ImGuiComponents.HelpMarker(
            "你可以在遊戲中開啟自己的名牌，並啟用此設定，讓名牌只顯示自己的團隊站位。\n若不希望顯示自己的站位，可直接在角色設定中關閉自己的名牌。");

        ImGuiExt.Spacer(6);

        ImGuiExt.SectionHeader("野外");
        using (ImRaii.PushIndent(15f)) {
            NameplateModeSection("##np_overworld", () => Plugin.Settings.DisplaySelectors.DisplayOverworld,
                sel => Plugin.Settings.DisplaySelectors.DisplayOverworld = sel,
                "隊伍：");

            NameplateModeSection("##np_others", () => Plugin.Settings.DisplaySelectors.DisplayOthers,
                sel => Plugin.Settings.DisplaySelectors.DisplayOthers = sel,
                "其他玩家：");
        }

        ImGuiExt.SectionHeader("副本");
        using (ImRaii.PushIndent(15f)) {
            NameplateModeSection("##np_dungeon", () => Plugin.Settings.DisplaySelectors.DisplayDungeon,
                (sel) => Plugin.Settings.DisplaySelectors.DisplayDungeon = sel,
                "迷宮：");

            NameplateModeSection("##np_raid", () => Plugin.Settings.DisplaySelectors.DisplayRaid,
                sel => Plugin.Settings.DisplaySelectors.DisplayRaid = sel,
                "大型任務：");

            NameplateModeSection("##np_alliance", () => Plugin.Settings.DisplaySelectors.DisplayAllianceRaid,
                sel => Plugin.Settings.DisplaySelectors.DisplayAllianceRaid = sel,
                "聯盟大型任務：");

            NameplateModeSection("##np_chaotic", () => Plugin.Settings.DisplaySelectors.DisplayChaoticRaid,
               sel => Plugin.Settings.DisplaySelectors.DisplayChaoticRaid = sel,
               "混沌大型任務：");
        }

        ImGuiExt.SectionHeader("特殊場域探索");

        using (ImRaii.PushIndent(15f)) {
            ImGui.TextDisabled("例如：禁地優雷卡、南方博茲雅戰線");

            NameplateModeSection("##np_field_party", () => Plugin.Settings.DisplaySelectors.DisplayFieldOperationParty,
                sel => Plugin.Settings.DisplaySelectors.DisplayFieldOperationParty = sel, "隊伍：");

            NameplateModeSection("##np_field_others",
                () => Plugin.Settings.DisplaySelectors.DisplayFieldOperationOthers,
                sel => Plugin.Settings.DisplaySelectors.DisplayFieldOperationOthers = sel, "其他玩家：");
        }

        ImGuiExt.SectionHeader("PvP");

        using (ImRaii.PushIndent(15f)) {
            ImGui.TextDisabled("本插件會在 PvP 對戰期間停用。");
        }

        ImGuiExt.Spacer(15);

        if (ImGui.CollapsingHeader("範例")) {
            foreach (var kv in _nameplateExamples) {
                CollapsibleExampleImage(kv.Key, kv.Value);
            }
        }
    }

    private static void CollapsibleExampleImage(NameplateMode mode, IDalamudTextureWrap tex)
    {
        if (ImGui.CollapsingHeader(UiNames.GetName(mode))) {
            ImGui.Image(tex.Handle, new Vector2(tex.Width, tex.Height));
        }
    }

    private static void NameplateModeSection(string label, Func<DisplaySelector> getter, Action<DisplaySelector> setter,
        string title = "Nameplate: ")
    {
        ImGui.SetCursorPosY(ImGui.GetCursorPos().Y + 3f);
        ImGui.Text(title);
        ImGui.SameLine(100f);
        ImGui.SetCursorPosY(ImGui.GetCursorPos().Y - 3f);
        ImGuiExt.SetComboWidth(Plugin.Settings.DisplayConfigs.Selectors.Select(UiNames.GetName));

        using var combo = ImRaii.Combo(label, UiNames.GetName(getter()));
        if (!combo) return;

        foreach (var selector in Plugin.Settings.DisplayConfigs.Selectors) {
            using var col = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.HealerGreen,
                selector.Preset == DisplayPreset.Custom);
            if (ImGui.Selectable(UiNames.GetName(selector), selector == getter())) {
                setter(selector);
                Plugin.Settings.Save();
            }
        }
    }
}
