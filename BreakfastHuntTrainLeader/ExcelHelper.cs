using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dalamud.Interface.Textures;
using Lumina.Excel.GeneratedSheets;
using OmenTools.Helpers;

namespace BreakfastHuntTrainLeader;

public class ExcelHelper
{
    public static Dictionary<uint, TerritoryType> Zones => zones.Value;
    public static Dictionary<uint, World> Worlds => worlds.Value;

    #region Lazy

    private static readonly Lazy<Dictionary<uint, TerritoryType>> zones =
        new(() => LuminaCache.Get<TerritoryType>()
                             .Where(x => x.PlaceName.Row > 0)
                             .ToDictionary(x => x.RowId, x => x));

    private static readonly Lazy<Dictionary<uint, World>> worlds =
        new(() => LuminaCache.Get<World>()
                             .Where(x => x.DataCenter.Value != null &&
                                         (x.DataCenter?.Value?.Region ?? 0) != 0 &&
                                         !string.IsNullOrWhiteSpace(x.DataCenter?.Value?.Name?.RawString) &&
                                         !string.IsNullOrWhiteSpace(x.Name.RawString) &&
                                         !string.IsNullOrWhiteSpace(x.InternalName.RawString) &&
                                         !x.Name.RawString.Contains('-') &&
                                         !x.Name.RawString.Contains('_'))
                             .Where(x => x.DataCenter.Value.Region != 5 ||
                                         (x.RowId > 1000 && x.RowId != 1200))
                             .ToDictionary(x => x.RowId, x => x));

    #endregion
}
