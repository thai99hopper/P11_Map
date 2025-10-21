using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EditorSpineBuildingPos", order = 1)]
public class ScriptableDataSpineBuildings : ScriptableObject
{
    public List<SpineBuildingPos> lSpineBuildings = new();

    public SpineBuildingPos GetSpineBuildingPos(string buildingId)
    {
        foreach(var pos in lSpineBuildings)
            if (pos.buildingId == buildingId)
                return pos;
        return null;
    }

    [System.Serializable]
    public class SpineBuildingPos
    {
        public string spineName; 
        public string buildingId;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale = Vector3.one; 
    }
}
