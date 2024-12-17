using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;
using OmenTools;
using OmenTools.Helpers;

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

    public unsafe void Relay()
    {
        var agentMap = AgentMap.Instance();
        agentMap->OpenMapByMapId(ExcelHelper.Zones[TerritoryId].Map.Row, TerritoryId);
        agentMap->SetFlagMapMarker(TerritoryId, ExcelHelper.Zones[TerritoryId].Map.Row, Position.X, Position.Y);
        if (ExcelHelper.Worlds[DService.ClientState.LocalPlayer.CurrentWorld.Id].Name.RawString == Server)
        {
            ChatHelper.Instance.SendMessage($"/sh 下一站-【本服】-<flag>");
        }
        else
        {
            ChatHelper.Instance.SendMessage($"/sh 下一站-【{Server}】-<flag>");
        }
    }
}
