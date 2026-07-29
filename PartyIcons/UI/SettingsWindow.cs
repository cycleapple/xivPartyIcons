using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using PartyIcons.UI.Settings;
using PartyIcons.UI.Utils;
using System.Numerics;

namespace PartyIcons.UI;

public sealed class SettingsWindow : Window
{
    private readonly GeneralTab _generalTab = new();
    private readonly NameplateTab _nameplateTab = new();
    private readonly AppearanceTab _appearanceTab = new();
    private readonly StaticAssignmentsTab _staticAssignmentsTab = new();

    private readonly FlashingText _flashingText = new();

    public SettingsWindow() : base("Party Icons 設定")
    {
        Size = new Vector2(450, 600);
        SizeCondition = ImGuiCond.FirstUseEver;

        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(450, 350),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        if (!Plugin.Settings.SelectorsDialogComplete || UpgradeGuideTab.ForceRedisplay) {
            UpgradeGuideTab.Draw();
            return;
        }

        using var tabBar = ImRaii.TabBar("##tabbar");
        if (tabBar) {
            var text = _flashingText.PushColor(Plugin.Settings.TestingMode);
            using (var tab = ImRaii.TabItem("一般")) {
                text.Dispose();
                if (tab) {
                    using var contents = ImRaii.Child("##general_content");
                    _generalTab.Draw();
                }
            }
            using (var tab = ImRaii.TabItem("名牌")) {
                if (tab) {
                    using var contents = ImRaii.Child("##nameplates_content");
                    _nameplateTab.Draw();
                }
            }
            using (var tab = ImRaii.TabItem("外觀")) {
                if (tab) {
                    using var contents = ImRaii.Child("##appearance_content");
                    _appearanceTab.Draw();
                }
            }
            using (var tab = ImRaii.TabItem("狀態圖示")) {
                if (tab) {
                    using var contents = ImRaii.Child("##statuses_content");
                    StatusTab.Draw();
                }
            }
            using (var tab = ImRaii.TabItem("聊天名稱")) {
                if (tab) {
                    using var contents = ImRaii.Child("##chat_names_content");
                    ChatNameTab.Draw();
                }
            }
            using (var tab = ImRaii.TabItem("團隊站位")) {
                if (tab) {
                    using var contents = ImRaii.Child("##static_assignments_content");
                    _staticAssignmentsTab.Draw();
                }
            }
        }
    }
}
