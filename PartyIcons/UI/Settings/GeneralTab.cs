using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using PartyIcons.UI.Utils;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Numerics;

namespace PartyIcons.UI.Settings;

public sealed class GeneralTab
{
    private readonly Notice _notice = new();
    private readonly FlashingText _flashingText = new();

    public void Draw()
    {
        ImGui.Dummy(new Vector2(0, 2f));

        using (ImRaii.PushColor(ImGuiCol.CheckMark, 0xFF888888)) {
            var usePriorityIcons = true;
            ImGui.Checkbox("##usePriorityIcons", ref usePriorityIcons);
            ImGui.SameLine();
            ImGui.Text("優先顯示狀態圖示");
            using (ImRaii.PushIndent())
            using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudOrange)) {
                ImGui.TextWrapped(
                    "注意：現在可在「外觀」分頁中，透過「交換方式」為各種名牌類型設定狀態圖示的優先方式。也能在「狀態圖示」分頁中設定哪些圖示屬於重要狀態。");
            }
            ImGui.Dummy(new Vector2(0, 3));
        }

        var testingMode = Plugin.Settings.TestingMode;
        if (ImGui.Checkbox("##testingMode", ref testingMode)) {
            Plugin.Settings.TestingMode = testingMode;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        using (_flashingText.PushColor(Plugin.Settings.TestingMode)) {
            ImGui.Text("啟用測試模式");
        }
        ImGuiComponents.HelpMarker("將設定套用至所有玩家，而不只是在隊伍中的玩家。");

        var chatContentMessage = Plugin.Settings.ChatContentMessage;

        if (ImGui.Checkbox("##chatmessage", ref chatContentMessage)) {
            Plugin.Settings.ChatContentMessage = chatContentMessage;
            Plugin.Settings.Save();
        }

        ImGui.SameLine();
        ImGui.Text("進入任務時顯示聊天訊息");
        ImGuiComponents.HelpMarker("可在載入完成前確認任務類型。");

        ImGuiExt.Spacer(10);
        if (ImGuiExt.ButtonEnabledWhen(ImGui.GetIO().KeyCtrl, "再次顯示升級指南")) {
            UpgradeGuideTab.ForceRedisplay = true;
        }
        ImGuiExt.HoverTooltip("按住 Ctrl 才能點擊");

        _notice.DisplayNotice();
    }
}

public sealed class Notice
{
    private string? _noticeString;
    private string? _noticeUrl;

    public Notice()
    {
        DownloadAndParseNotice();
    }

    private void DownloadAndParseNotice()
    {
        using var httpClient = new HttpClient();
        try {
            var strArray = httpClient.GetStringAsync("https://shdwp.github.io/ukraine/xiv_notice.txt").Result.Split('|');

            if (strArray.Length > 0) {
                _noticeString = strArray[0].Replace("\n", "\n\n");
            }

            if (strArray.Length < 2) {
                return;
            }

            _noticeUrl = strArray[1];

            if (!(_noticeUrl.StartsWith("http://") || _noticeUrl.StartsWith("https://"))) {
                Service.Log.Warning($"Received invalid noticeUrl {_noticeUrl}, ignoring");
                _noticeUrl = null;
            }
        }
        catch (Exception) {
            // ignored
        }
    }

    public void DisplayNotice()
    {
        if (_noticeString == null)
            return;

        ImGui.Dummy(new Vector2(0.0f, 15f));

        using var col = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DPSRed);

        ImGui.TextWrapped(_noticeString);

        if (_noticeUrl != null && ImGui.Button(_noticeUrl)) {
            try {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _noticeUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception) {
                // ignored
            }
        }
    }
}
