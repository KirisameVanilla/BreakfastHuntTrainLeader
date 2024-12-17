using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;

namespace BreakfastHuntTrainLeader;
public class HuntMark
{
    public int ServerIndex { get; set; } = 0;
    public uint TerritoryId { get; set; } = 0;
    public string Server => ImGuiWidget.豆豆柴[ServerIndex];
    public string Territory => ExcelHelper.Zones[TerritoryId].PlaceName.Value.Name.RawString;
    public Vector2 Position { get; set; }

    public HuntMark(int index)
    {
        ServerIndex = index;
    }
    public HuntMark() {}


    public unsafe bool InitByFlag()
    {
        var agentMap = AgentMap.Instance();
        if (agentMap == null || agentMap->IsFlagMarkerSet == 0) return false;
        Position = new(agentMap->FlagMapMarker.XFloat, agentMap->FlagMapMarker.YFloat);
        TerritoryId = agentMap->FlagMapMarker.TerritoryId;
        return true;
    }
}
