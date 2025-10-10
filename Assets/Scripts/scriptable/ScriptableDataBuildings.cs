using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class ScriptableDataBuildings : ScriptableObject
{
    [SerializeField] public List<BuildingData> buildings;

    public BuildingData GetBuildingData(string buildingId)
    {
        return buildings.Find(x => x.buildingId == buildingId);
    }

    public BuildingData GetBuildingDataSpine(string buildingId)
    {
        return buildings.Find(x => x.buildingIdSpine == buildingId);
    }

    [System.Serializable]
    public class BuildingData
    {
        public string buildingId;
        public string buildingIdMapping;
        public string nameTextLocalizeMapping;
        public string nameTextTooltipLocalizeMapping;
        public int buildingCharacterResourceIdMapping;
        public List<ModelLoadPath> modelLoadPaths;
        public string buildingIdSpine;
        public string buildingIdSpineMapping;
        
        public ModelLoadPath GetModelLoadPath(int orderBuilding)
        {
            return modelLoadPaths.Find(x => x.orderBuilding == orderBuilding);
        }
    }

    [System.Serializable]
    public class ModelLoadPath
    {
        public string modelPath;
        public int orderBuilding; 
    }
}
