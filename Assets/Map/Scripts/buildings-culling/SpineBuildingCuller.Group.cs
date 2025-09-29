

using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// SpineBuildingCuller.Group.cs:
/// Check each update to show/hide a group of buildings.
/// </summary>

///Note:
///1. Init building into the group:
///- Each group has obj min / max to determine the bound of group => called is groupRect.
///- Each time BuildingOnMapController is added to the scene, it will be registered to the group.
///
///2. Check each update to Active/Inactive the group:
///- If the building is existed in at least one active group => it will be active.
public partial class SpineBuildingCuller
{
    #region Attributes:

    public List<BuildingCullingGroup> cullingGroups = new List<BuildingCullingGroup>();
    public List<BuildingCullingGroup> ActiveGroups => cullingGroups.FindAll(group => group.IsActive.Value);
    public List<ReactiveProperty<bool>> IsGroupActiveState => cullingGroups.Select(group => group.IsActive).ToList();
    public Dictionary<string, bool> activeBuildings = new Dictionary<string, bool>();

    #endregion Attributes!!!


    #region OnAddBuilding:

    private void OnAddBuilding(BuildingOnMapController building)
    {
        if (building == null) return;
        // Register the building to the culling group.
        foreach (var group in cullingGroups)
        {
            group.OnAddBuilding(building);
        }
    }

    #endregion OnAddBuilding!!!

    #region Observable Handling:
    private IDisposable disposable;

    private void SubscribesObservables()
    {
        disposable = GetOnGroupsActiveChanged()
            .Subscribe(activeStates =>
            {
                //LogMsg($"SubscribesObservables: GetOnGroupsActiveChanged: {IsGroupActiveState.ToArray()}");
                CheckUpdateBuildingActiveState();
            })
            .AddTo(this);
    }

    private Observable<bool[]> GetOnGroupsActiveChanged()
    {
        if (IsGroupActiveState.Count == 0)
        {
            return Observable.Empty<bool[]>();
        }

        return Observable.CombineLatest(IsGroupActiveState.ToArray());
    }

    #endregion Observable Handling!!!

    #region Check/Update Active Building:

    private void UpdateCullingGroupActiveState()
    {
        var cameraBounds = GetCameraBounds();
        foreach (var group in cullingGroups)
        {
            group.UpdateGroupIntersectCamera(cameraBounds);
        }
    }

    private void CheckUpdateBuildingActiveState()
    {
        var isVisible = false;
        foreach (var building in lBuildingObj)
        {
            isVisible = isCheckBuildingIsActive(building.obj.buildingId);
            building.obj.visibleRx.Value = isVisible;
        }
    }

    private bool isCheckBuildingIsActive(string buildingId)
    {
        foreach (var group in ActiveGroups)
        {
            if (group.Buildings.Contains(buildingId))
            {
                return true; // If the building is in any active group, it is considered active.
            }
        }
        return false; // If not found in any active group, it is considered inactive.
    }

    #endregion Check/Update Active Building!!
}