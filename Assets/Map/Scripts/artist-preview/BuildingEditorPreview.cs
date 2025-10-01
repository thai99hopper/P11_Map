
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BuildingEditorPreview : MonoBehaviour
{
    public string AreaName => this.transform.parent.gameObject.name;

    public List<BuildingOnMapController> GetBuldingList()
    {
        var list = this.transform.GetComponentsInChildren<BuildingOnMapController>(true);
        return new List<BuildingOnMapController>(list);
    }
}