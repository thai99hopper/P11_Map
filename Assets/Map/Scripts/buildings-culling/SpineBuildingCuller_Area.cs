using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BuildingCulling
{
    public class SpineBuildingCuller_Area : MonoBehaviour
    {
        private void Start()
        {
            var listBuildingInfos = RetrieveListBuildingObjs();
            SpineBuildingCuller.Instance.AddListBuildingObjs(listBuildingInfos);
        }

        private BuildingOnMapController[] RetrieveListBuildingObjs()
        {
            var buildings = GetComponentsInChildren<BuildingOnMapController>();
            return buildings;
        }
    }
}
