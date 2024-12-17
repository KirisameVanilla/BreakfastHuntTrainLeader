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

    #region Lazy

    private static readonly Lazy<Dictionary<uint, TerritoryType>> zones =
        new(() => LuminaCache.Get<TerritoryType>()
                             .Where(x => x.PlaceName.Row > 0)
                             .ToDictionary(x => x.RowId, x => x));

    #endregion
}
