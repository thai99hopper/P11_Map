using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterOnMapData", order = 1)]
public class ScriptableCharacterOnMapData : ScriptableObject
{
    public List<CharacterOnMapData> characterOnMapDataList = new();

    public List<CharacterOnMapData> GetCharacterOnMapData(string buildingId, string catName)
    {
        var l = new List<CharacterOnMapData>();
        foreach (var data in characterOnMapDataList)
        {
            if (data.building_id == buildingId && data.cat_name == catName)
                l.Add(data); 
        }
        return l;  
    }

    public CharacterOnMapData GetCharacterOnMapData(string buildingId, string catName, CharacterOnMapType characterType)
    {
        foreach (var data in characterOnMapDataList)
        {
            if (data.character_type == characterType && data.building_id == buildingId && data.cat_name == catName)
                return data;
        }
        return null;
    }
    

    [System.Serializable]
    public class CharacterOnMapData
    {
        public string building_id;
        public string cat_name;
        public CharacterOnMapType character_type;
        public float position_x;
        public float position_y;
        public float position_z;
        public float rotation_x;
        public float rotation_y;
        public float rotation_z;
        public float scale_x;
        public float scale_y;
        public float scale_z;
        
        public Vector3 position => new Vector3(position_x, position_y, position_z);
        public Vector3 rotation => new Vector3(rotation_x, rotation_y, rotation_z);
        public Vector3 scale => new Vector3(scale_x, scale_y, scale_z);
    }
}
