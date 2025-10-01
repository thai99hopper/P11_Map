using UnityEngine;

public class VisualController : SingletonMonoBehaviour<VisualController>
{
    [SerializeField] private GameObject[] spineBuildings; // assign in editor 

    public void SpawnSpineBuilding(BuildingOnMapController buildingOnMapController)
    {
        foreach (var building in spineBuildings)
        {
            if (building.gameObject.name == buildingOnMapController.gameObject.name)
            {
                Instantiate(building, buildingOnMapController.transform);
                break;
            }
        }
    }
}
