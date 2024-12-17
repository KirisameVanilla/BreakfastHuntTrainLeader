using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace BreakfastHuntTrainLeader;

public class ImGuiWidget
{
    public static Dictionary<int, string> 豆豆柴 = new()
    {
        { 1, "银泪湖" },
        { 2, "太阳海岸" },
        { 3, "水晶塔"},
        { 4, "红茶川"},
        { 5, "伊修加德"}
    };

    public static void ServerSelectCombo(ref int selected)
    {
        ImGui.Combo("选择服务器", ref selected, 豆豆柴.Values.ToArray(), 5);
    }
}
