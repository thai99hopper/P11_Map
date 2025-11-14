using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableCharacterInitializePosition", order = 1)]
public class ScriptableCharacterInitializePosition : ScriptableObject
{
    public List<CharacterInitializeConfig> characterConfigs = new();

    public BuildingInitializeConfig GetBuildingConfig(string characterName, string buildingId)
    {
        foreach (var config in characterConfigs)
        {
            if (config.characterName != characterName) continue;
            foreach (var building in config.buildingConfigs)
            {
                if (building.buildingId == buildingId)
                    return building;
            }
        }
        return null;
    }
    

    [System.Serializable]
    public class CharacterInitializeConfig
    {
        public string characterName;
        public List<BuildingInitializeConfig> buildingConfigs = new();
    }

    [System.Serializable]
    public class BuildingInitializeConfig
    {
        public string buildingId;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale; 
    }
}
