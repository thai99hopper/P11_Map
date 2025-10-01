using System.Collections.Generic;
using UnityEngine;

public class VisualController : SingletonMonoBehaviour<VisualController>
{
    [SerializeField] private GameObject[] spineBuildings; // assign in editor 

    /// <summary>
    /// https://docs.google.com/spreadsheets/d/1_mlYIcyUeQBvM3uXl-Ww9lTVisApHwldjB7CPTYMjkI/edit?pli=1&gid=894564700#gid=894564700
    /// get from here
    /// </summary>
    Dictionary<string,string> mapping = new Dictionary<string, string>
    {
        { "A01", "A01" },
        { "A02", "A02" },
        { "A03", "A03" },
        { "A04", "A04" },
        { "A05", "A05" },
        { "A06", "A06" },
        { "A07", "A07" },
        { "A08", "A08" },
        { "A09", "A09" },
        { "A10", "A10" },
        { "A11", "A11" },
        { "A12", "A12" },
        { "A13", "A13" },
        { "A14", "A14" },
        { "A15", "A15" },
        { "A16", "A16" },
        { "B01", "B05" },
        { "B02", "B02" },
        { "B03", "B03" },
        { "B04", "B04" },
        { "B05", "B01" },
        { "B06", "B06" },
        { "B07", "B07" },
        { "B08", "B08" },
        { "B09", "B09" },
        { "B10", "B10" },
        { "B11", "B11" },
        { "B12", "B12" },
        { "B13", "B13" },
        { "B14", "B14" },
        { "B15", "B15" },
        { "B16", "B16" },
        { "B17", "B17" },
        { "B18", "B18" },
        { "B19", "B19" },
        { "B20", "B20" },
        { "B21", "B21" },
        { "B22", "B22" },
        { "B23", "B23" },
        { "B24", "B24" },
        { "B25", "B25" },
        { "B26", "B26" },
        { "B27", "B27" },
        { "B28", "B28" },
        { "B29", "B29" },
        { "B30", "B30" },
        { "B31", "B31" },
    };

    
    public void SpawnSpineBuilding(BuildingOnMapController buildingOnMapController)
    {
        foreach (var building in spineBuildings)
        {
            if (building.gameObject.name == mapping[buildingOnMapController.gameObject.name])
            {
                Instantiate(building, buildingOnMapController.transform);
                break;
            }
        }
    }
}
