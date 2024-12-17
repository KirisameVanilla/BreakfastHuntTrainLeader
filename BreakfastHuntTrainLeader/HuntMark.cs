using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OmenTools;
using OmenTools.Helpers;

namespace BreakfastHuntTrainLeader;
public class HuntMark
{
    public int ServerIndex { get; set; } = 0;
    public uint TerritoryId { get; set; } = 0;
    public uint InstanceId { get; set; } = 0;
    public string Server => ImGuiWidget.大区[Plugin.Config.大区名][ServerIndex];
    public string Territory => ExcelHelper.Zones[TerritoryId].PlaceName.Value.Name.RawString;
    public string Instance => InstanceId == 0 ? string.Empty : Plugin.Config.分线模板.Format("","",InstanceId.SEChar());
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
            foreach (var command in Plugin.Config.RelayCommands)
            {
                ChatHelper.Instance.SendMessage(command + " " + Plugin.Config.同服扩散模板.Format(Instance));
            }
        }
        else
        {
            foreach (var command in Plugin.Config.RelayCommands)
            {
                ChatHelper.Instance.SendMessage(command + " " + Plugin.Config.跨服扩散模板.Format(Instance, Server));
            }
        }
    }
}
