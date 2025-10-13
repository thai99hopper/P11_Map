using System.Collections.Generic;
using UnityEngine;

public class VisualController : SingletonMonoBehaviour<VisualController>
{
    [SerializeField] private GameObject[] spineBuildings; // assign in editor
    [SerializeField] private ScriptableDataBuildings scriptableDataBuildings; 

    
    public void SpawnSpineBuilding(BuildingOnMapController buildingOnMapController)
    {
        var data = scriptableDataBuildings.GetBuildingData(buildingOnMapController.buildingId);
        if (data == null) return; 
        
        foreach (var spine in spineBuildings)
        {
            var spineId = spine.gameObject.name;   
            if (data.buildingIdSpineMapping == spineId)
            {
                Instantiate(spine, buildingOnMapController.transform);
                break;
            }
        }
    }
}
