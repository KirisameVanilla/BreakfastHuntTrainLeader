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

    public List<HuntMark> Marks { get; set; } = [];
}
