using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using OmenTools;

namespace BreakfastHuntTrainLeader;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public void SaveConfig() => DService.PI.SavePluginConfig(this);
    public readonly HashSet<string> RelayCommands = [];
    public List<HuntMark> Marks { get; set; } = [];
    public string 同服扩散模板 { get; set; } = "下一站-【本服】-<flag>{0}";
    public string 跨服扩散模板 { get; set; } = "下一站-【{1}】-<flag>{0}";
    public string 分线模板 { get; set; } = "-{2}线";
    public string 大区名 { get; set; } = "豆豆柴";
}
