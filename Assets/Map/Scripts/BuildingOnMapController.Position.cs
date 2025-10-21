using System.Collections.Generic;
using UnityEngine;

public partial class BuildingOnMapController
{
    [System.Serializable]
    public class ObjectPos
    {
        public string name;
        public Transform transf; 
    }
    
    [Header("Position References")] 
    [SerializeField] private List<ObjectPos> lPosParts = new();
    [SerializeField] private List<ObjectPos> lPosVfxs = new();
}
