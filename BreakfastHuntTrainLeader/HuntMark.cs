using System.Numerics;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Helpers;

namespace BreakfastHuntTrainLeader;
public class HuntMark
{
    public int ServerIndex { get; set; } = 0;
    public uint TerritoryId { get; set; } = 0;
    public uint InstanceId { get; set; } = 0;
    public uint MapId { get; set; } = 0;
    public string Server => ImGuiWidget.大区[Plugin.Config.大区名][ServerIndex];
    public string Instance => InstanceId == 0 ? string.Empty : Plugin.Config.分线模板.Format("","",InstanceId.SEChar());
    public Vector2 Position { get; set; }
    [JsonIgnore] public string? Territory => ExcelHelper.Zones[TerritoryId].PlaceName.Value?.Name.RawString;
    [JsonIgnore] public Vector2 MapPos => HelpersOm.WorldToMap(Position, ExcelHelper.Maps[MapId]);

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
        MapId = agentMap->FlagMapMarker.MapId;
        return true;
    }

    public void Relay()
    {
        if (DService.ClientState.LocalPlayer == null) return;
        FlagMark();
        if (ExcelHelper.Worlds[DService.ClientState.LocalPlayer.CurrentWorld.Id].Name.RawString == Server)
            foreach (var command in Plugin.Config.RelayCommands)
                ChatHelper.Instance.SendMessage(command + " " + Plugin.Config.同服扩散模板.Format(Instance));
        else
            foreach (var command in Plugin.Config.RelayCommands)
                ChatHelper.Instance.SendMessage(command + " " + Plugin.Config.跨服扩散模板.Format(Instance, Server));
        
    }

    public unsafe bool FlagMark()
    {
        var agentMap = AgentMap.Instance();
        if (agentMap == null) return false;
        if (!agentMap->IsAgentActive() || agentMap->SelectedMapId != MapId)
            agentMap->OpenMap(MapId, TerritoryId);
        agentMap->SetFlagMapMarker(TerritoryId, MapId, new Vector3(Position.X, 0f, Position.Y));
        return true;
    }
}
