using UnityEngine;

public partial class McOnMapController : MonoBehaviour
{
    /// <summary>
    /// need setup building before using mc on map controller
    /// </summary>
    /// <param name="buildingId"></param>
    public void SetupBuildingId(string buildingId)
    {
        this.buildingId = buildingId; 
    }
}
