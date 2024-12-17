using System;
using System.Collections.Generic;
using Dalamud.Configuration;

namespace BreakfastHuntTrainLeader;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public void SaveConfig() => Service.PluginInterface.SavePluginConfig(this);

    public List<HuntMark> Marks { get; set; } = [];
}
