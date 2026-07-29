using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using PartyIcons.Configuration;
using PartyIcons.Runtime;
using PartyIcons.UI.Utils;
using System.Linq;
using System.Numerics;

namespace PartyIcons.UI.Settings;

public static class UpgradeGuideTab
{
    public static bool ForceRedisplay { get; set; }

    public static void Draw()
    {
        var buttonSize = new Vector2(100f, 80f) * ImGuiHelpers.GlobalScale;

        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.ParsedGreen)) {
            ImGui.TextWrapped("Party Icons 升級指南（適用於 1.2 版）");
        }

        ImGui.TextWrapped("Party Icons 的《黃金的遺產》更新調整了狀態圖示的顯示方式。");
        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey2)) {
            ImGui.TextWrapped("（狀態圖示是指斷線、觀看過場動畫、等待任務、指導者、新手冒險者等線上狀態的圖示。）");
        }
        ImGui.TextWrapped("從此版本開始，可以同時顯示職業圖示與狀態圖示，並能為各種名牌顯示類型分別自訂。");

        ImGuiExt.Spacer(8);
        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudYellow)) {
            ImGui.TextWrapped("你希望如何顯示狀態圖示？");
        }
        ImGuiExt.Spacer(8);

        if (ImGui.Button("使用新的\n預設值", buttonSize)) {
            UseNewDefaults();
        }
        ImGui.SameLine();
        ImGui.TextWrapped("在任務中，部分重要狀態會與職業圖示交換位置，其餘狀態大多隱藏；在野外，大多數狀態會顯示於各自的狀態圖示欄位。");
        ImGui.Separator();

        if (ImGui.Button("沿用舊版\n優先系統", buttonSize)) {
            ReplicatePriority();
        }
        ImGui.SameLine();
        ImGui.TextWrapped("適用時，部分重要狀態圖示（例如斷線、離開鍵盤或觀看過場動畫）會完全取代職業圖示，其餘狀態圖示則隱藏。");
        ImGui.Separator();

        if (ImGui.Button("完全不顯示\n狀態圖示", buttonSize)) {
            NoIcons();
        }
        ImGui.SameLine();
        ImGui.TextWrapped("只顯示職業圖示。");
        ImGui.Separator();

        if (ForceRedisplay) {
            if (ImGui.Button("取消", buttonSize)) {
                Cancel();
            }
            ImGui.SameLine();
            ImGui.TextWrapped("不進行任何變更。");
            ImGui.Separator();
        }

        ImGui.TextWrapped("請選擇上述其中一個選項以繼續。之後可從「一般」分頁再次開啟此視窗，或在「外觀」與「狀態圖示」分頁中進一步調整狀態圖示的顯示方式。");
    }

    private static void UseNewDefaults()
    {
        foreach (var config in Plugin.Settings.DisplayConfigs.Configs.Where(c => c.Preset != DisplayPreset.Custom)) {
            config.StatusIcon.Show = true;
            config.SwapStyle = StatusSwapStyle.Swap;
            config.StatusSelectors[ZoneType.Overworld] = new StatusSelector(StatusPreset.Overworld);
        }
        Plugin.Settings.SelectorsDialogComplete = true;
        Plugin.Settings.Save();
        ForceRedisplay = false;

    }

    private static void ReplicatePriority()
    {
        foreach (var config in Plugin.Settings.DisplayConfigs.Configs.Where(c => c.Preset != DisplayPreset.Custom)) {
            config.StatusIcon.Show = true;
            config.SwapStyle = StatusSwapStyle.Replace;
            config.StatusSelectors[ZoneType.Overworld] = new StatusSelector(StatusPreset.OverworldLegacy);
        }
        Plugin.Settings.SelectorsDialogComplete = true;
        Plugin.Settings.Save();
        ForceRedisplay = false;
    }

    private static void NoIcons()
    {
        foreach (var config in Plugin.Settings.DisplayConfigs.Configs.Where(c => c.Preset != DisplayPreset.Custom)) {
            config.StatusIcon.Show = false;
        }
        Plugin.Settings.SelectorsDialogComplete = true;
        Plugin.Settings.Save();
        ForceRedisplay = false;
    }

    private static void Cancel()
    {
        ForceRedisplay = false;
    }
}
