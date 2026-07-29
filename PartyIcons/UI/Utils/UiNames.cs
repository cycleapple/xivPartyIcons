using System;
using PartyIcons.Configuration;
using PartyIcons.Runtime;

namespace PartyIcons.UI.Utils;

public static class UiNames
{
    public static string GetName(NameplateMode mode)
    {
        return mode switch
        {
            NameplateMode.Default => "遊戲預設",
            NameplateMode.Hide => "隱藏",
            NameplateMode.BigJobIcon => "大型職業圖示",
            NameplateMode.SmallJobIcon => "小型職業圖示與名稱",
            NameplateMode.SmallJobIconAndRole => "小型職業圖示、站位與名稱",
            NameplateMode.BigJobIconAndPartySlot => "大型職業圖示與隊伍編號",
            NameplateMode.RoleLetters => "站位代號",
            _ => $"未知（{(int)mode}/{mode}）"
        };
    }

    public static string GetName(ZoneType zoneType)
    {
        return zoneType switch
        {
            ZoneType.Overworld => "野外",
            ZoneType.Dungeon => "迷宮",
            ZoneType.Raid => "大型任務",
            ZoneType.AllianceRaid => "聯盟大型任務",
            ZoneType.ChaoticRaid => "混沌大型任務",
            ZoneType.FieldOperation => "特殊場域探索",
            _ => $"未知（{(int)zoneType}/{zoneType}）"
        };
    }

    public static string GetName(StatusConfig config)
    {
        return config.Preset switch
        {
            StatusPreset.Custom => config.Name ?? "<未命名>",
            StatusPreset.Overworld => "野外",
            StatusPreset.Instances => "副本",
            StatusPreset.FieldOperations => "特殊場域探索",
            StatusPreset.OverworldLegacy => "野外（舊版）",
            _ => config.Preset + "/" + config.Name + "/" + config.Id
        };
    }

    public static string GetName(StatusSelector selector)
    {
        return GetName(Plugin.Settings.GetStatusConfig(selector));
    }

    public static string GetName(DisplayConfig config)
    {
        if (config.Preset == DisplayPreset.Custom) {
            return $"{GetName(config.Mode)} ({config.Name})";
        }

        return GetName(config.Mode);
    }

    public static string GetName(DisplaySelector selector)
    {
        return GetName(Plugin.Settings.GetDisplayConfig(selector));
    }

    public static string GetName(IconSetId id)
    {
        return id switch
        {
            IconSetId.EmbossedFramed => "外框、依職能著色",
            IconSetId.EmbossedFramedSmall => "外框、依職能著色（小）",
            IconSetId.Gradient => "漸層、依職能著色",
            IconSetId.Glowing => "發光",
            IconSetId.Embossed => "浮雕",
            IconSetId.Inherit => "<使用全域設定>",
            _ => id.ToString()
        };
    }

    public static string GetName(ChatMode mode)
    {
        return mode switch
        {
            ChatMode.GameDefault => "遊戲預設",
            ChatMode.Role => "站位",
            ChatMode.Job => "職業簡稱",
            _ => throw new ArgumentException($"Unknown chat mode {mode}")
        };
    }

    public static string GetName(RoleDisplayStyle style)
    {
        return style switch
        {
            RoleDisplayStyle.None => "無",
            RoleDisplayStyle.Role => "站位",
            RoleDisplayStyle.PartyNumber => "隊伍編號",
            _ => throw new ArgumentException($"Unknown RoleDisplayStyle {style}")
        };
    }

    public static string GetName(NameplateSizeMode mode)
    {
        return mode switch
        {
            NameplateSizeMode.Smaller => "較小",
            NameplateSizeMode.Medium => "中等",
            NameplateSizeMode.Bigger => "較大",
            NameplateSizeMode.Custom => "自訂",
            _ => mode.ToString()
        };
    }

    public static string GetName(StatusSwapStyle style)
    {
        return style switch
        {
            StatusSwapStyle.None => "無",
            StatusSwapStyle.Swap => "交換",
            StatusSwapStyle.Replace => "取代",
            _ => style.ToString()
        };
    }

    public static string GetName(StatusVisibility visibility)
    {
        return visibility switch
        {
            StatusVisibility.Hide => "隱藏",
            StatusVisibility.Show => "顯示",
            StatusVisibility.Important => "重要",
            StatusVisibility.Unset => "未設定",
            StatusVisibility.Unknown => "未知",
            StatusVisibility.Unexpected => "非預期",
            _ => visibility.ToString()
        };
    }
}
