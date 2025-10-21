using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectBuildings", menuName = "ScriptableObjects/ObjectBuildings", order = 1)]
public class ScriptableDataObjectBuildings : ScriptableObject
{
    [SerializeField] private List<ObjectData> objectDatas = new();

    public List<ObjectData> GetObjects(string buildingId)
    {
        var l = new List<ObjectData>();
        foreach (var data in objectDatas)
        {
            if (data.buildingId == buildingId)
                l.Add(data);
        }
        return l; 
    }

    [System.Serializable]
    public class ObjectData
    {
        public string buildingId;
        public int lvMain;
        public int lvPart;
    }
}
