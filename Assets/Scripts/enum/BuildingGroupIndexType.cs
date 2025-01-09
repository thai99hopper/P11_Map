public enum BuildingGroupIndexType
{
    building_01,
    building_02,
    building_03,
    building_04,
    building_05,
    building_06,
    building_07,
    building_08,
    building_09,
    building_10,
    building_11,
    building_12,
    building_13,
    building_14,
    building_15,
    building_16,
    building_17,
    building_18,
    building_19,
    building_20,

}

public static class BuildingGroupIndexTypeExt
{
    /// <summary>
    /// https://docs.google.com/spreadsheets/d/1_mlYIcyUeQBvM3uXl-Ww9lTVisApHwldjB7CPTYMjkI/edit?gid=1218883865#gid=1218883865
    /// Get index through building group column name : building_item_index_ref
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ToItemIndex(this BuildingGroupIndexType type)
    {
        return type switch
        {
            BuildingGroupIndexType.building_01 => "building_basic_01",
            _ => "none",
        };
    }
}
