using UnityEngine;

public partial class McOnMapController : MonoBehaviour
{
    public string cat_name { get; private set; }
    /// <summary>
    /// need setup building before using mc on map controller
    /// </summary>
    /// <param name="buildingId"></param>
    public void SetupBuildingId(string buildingId,string cat_name)
    {
        this.buildingId = buildingId;
        this.cat_name = cat_name; 
    }
}
