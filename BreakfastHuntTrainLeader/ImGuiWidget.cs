using ImGuiNET;
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

    public static bool ServerSelectCombo(ref int selected)
    {
        return ImGui.Combo("##选择服务器", ref selected, 豆豆柴.Values.ToArray(), 5);
    }
}
