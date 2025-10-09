using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class McOnMapPresenter : MonoBehaviour
{

    //atributes
    public string buildingId;
    public int orderInBuilding = 0;

    //cached
    private GameObject mcGoCached = null;

    public bool IsArealdyLoaded()
    {
        return mcGoCached != null;
    }

    public void UpdateMcGameObject(GameObject mcGo)
    {
        if (mcGo == null)
        {
            Debug.LogWarning($"McOnMapPresenter > UpdateMcGameObject > mcGo is null!");
            return;
        }
        if (mcGoCached != null)
        {
            Debug.LogWarning($"McOnMapPresenter > UpdateMcGameObject > Character for BuildingId {buildingId} OrderInBuilding {orderInBuilding} is already loaded!");
            return;
        }
        mcGoCached = mcGo;
    }
}