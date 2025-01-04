using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Excel.Sheets;
using OmenTools.Helpers;

namespace BreakfastHuntTrainLeader;

public class ExcelHelper
{
    public static Dictionary<uint, TerritoryType> Zones => zones.Value;
    public static Dictionary<uint, World> Worlds => worlds.Value;
    public static Dictionary<uint, Map> Maps => maps.Value;

    #region Lazy

    private static readonly Lazy<Dictionary<uint, TerritoryType>> zones =
        new(() => LuminaCache.Get<TerritoryType>()
                             .Where(x => x.PlaceName.RowId > 0)
                             .ToDictionary(x => x.RowId, x => x));

    private static readonly Lazy<Dictionary<uint, World>> worlds =
        new(() => LuminaCache.Get<World>()
                             .Where(x => x.DataCenter.ValueNullable != null &&
                                         (x.DataCenter.ValueNullable?.Region ?? 0) != 0 &&
                                         !string.IsNullOrWhiteSpace(x.DataCenter.Value.Name.ExtractText()) &&
                                         !string.IsNullOrWhiteSpace(x.Name.ExtractText()) &&
                                         !string.IsNullOrWhiteSpace(x.InternalName.ExtractText()) &&
                                         !x.Name.ExtractText().Contains('-') &&
                                         !x.Name.ExtractText().Contains('_'))
                             .Where(x => x.DataCenter.Value.Region != 5 ||
                                         (x.RowId > 1000 && x.RowId != 1200))
                             .ToDictionary(x => x.RowId, x => x));

    private static readonly Lazy<Dictionary<uint, Map>> maps =
        new(() => LuminaCache.Get<Map>()
                             .Where(x => x.PlaceName.RowId > 0)
                             .ToDictionary(x => x.RowId, x => x));

    #endregion
}
