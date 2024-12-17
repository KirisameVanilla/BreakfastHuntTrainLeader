using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface;
using ImGuiNET;
using OmenTools.Helpers;
using OmenTools.ImGuiOm;
using System.Collections.Generic;
using System.Linq;

namespace BreakfastHuntTrainLeader;

public class ImGuiWidget
{
    public static Dictionary<int, string> 豆豆柴 = new()
    {
        { 0, "银泪湖" },
        { 1, "太阳海岸" },
        { 2, "水晶塔"},
        { 3, "红茶川"},
        { 4, "伊修加德"}
    };

    public static Dictionary<int, string> 莫古力 = new()
    {
        { 0, "神拳痕" },
        { 1, "白金幻象" },
        { 2, "白银乡"},
        { 3, "潮风亭"},
        { 4, "拂晓之间"},
        { 5, "龙巢神殿"},
        { 6, "旅人栈桥"},
        { 7, "梦羽宝境"}
    };

    public static Dictionary<string, Dictionary<int, string>> 大区 = new()
    {
        { "豆豆柴", 豆豆柴 },
        { "莫古力", 莫古力 }
    };

    public static bool ServerSelectCombo(ref int selected, string 大区名 = "豆豆柴") => ImGui.Combo("##选择服务器", ref selected, 大区[大区名].Values.ToArray(), 大区[大区名].Count);
    public static bool DataCenterSelectCombo(ref int selected, out string 大区名)
    {
        var 大区列表 = 大区.Keys.ToArray();
        var res = ImGui.Combo("##选择大区", ref selected, 大区列表, 大区列表.Length);
        大区名 = 大区列表[selected];
        return res;
    }

    private static readonly HashSet<string> SupportedRelayCommands = ["/sh", "/y", "/b", "/p", "/fc", "/cwl1", "/cwl2", "/cwl3", "/cwl4", "/cwl5", "/cwl6", "/cwl7", "/cwl8", "/ls1", "/ls2", "/ls3", "/ls4", "/ls5", "/ls6", "/ls7", "/ls8"];


    public static void ChatChannelCombo()
    {
        using var combo = ImRaii.Combo("###RelayCommandCombo",
                                       $"当前已选中 {Plugin.Config.RelayCommands.Count} 个扩散频道",
                                       ImGuiComboFlags.HeightLarge);
        if (combo)
        {
            foreach (var command in SupportedRelayCommands)
            {
                if (ImGui.Selectable(command, Plugin.Config.RelayCommands.Contains(command), ImGuiSelectableFlags.DontClosePopups))
                {
                    if (!Plugin.Config.RelayCommands.Remove(command))
                        Plugin.Config.RelayCommands.Add(command);
                    Plugin.Config.SaveConfig();
                }
            }
        }
    }

    public static bool SwapButtons<T>(int index, List<T> list)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        using (ImRaii.Disabled(index < 1))
            if (ImGuiOm.ButtonIcon($"##moveUp{index}", FontAwesomeIcon.ArrowUp))
            {
                list.Swap(index, index - 1);
                ImGui.PopFont();
                return true;
            }
        ImGui.SameLine();
        using (ImRaii.Disabled(index >= Plugin.Config.Marks.Count - 1))
            if (ImGuiOm.ButtonIcon($"##moveDown{index}", FontAwesomeIcon.ArrowDown))
            {
                list.Swap(index, index + 1);
                ImGui.PopFont();
                return true;
            }
        ImGui.PopFont();
        return false;
    }

    public static void HelpMarker(string text) => ImGuiOm.HelpMarker(text, useStaticFont:true);
}
