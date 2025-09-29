using R3;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCullingGroup : MonoBehaviour
{
    #region Attributes:
    public Transform limit_min;
    public Transform limit_max;

    [HideInInspector]
    public ReactiveProperty<bool> IsActive => isActive;
    private ReactiveProperty<bool> isActive = new ReactiveProperty<bool>(false);


    [HideInInspector]
    public Rect GroupRect => groupRect;
    private Rect groupRect;

    public List<string> Buildings => buildings;
    private List<string> buildings = new List<string>();
    #endregion Attributes!!!

    #region MonoBehaviour

    private void Awake()
    {

        if (!SpineBuildingCuller.IsApply_CullingGroup)
        {
            return;
        }

        groupRect = CalculateGroupBounds();
        IsActive.Subscribe(active =>
        {
            LogMsg($"Group {gameObject.name} is now {(active ? "Active" : "Inactive")}");
        }).AddTo(this);
    }

    private void OnDrawGizmos()
    {
        DrawGroupBounds();
    }

    public void DrawGroupBounds()
    {
        LogMsg("DrawGroupBounds");
        if (limit_min == null || limit_max == null)
        {
            return;
        }
        var groupRect = CalculateGroupBounds();
        DrawRect(groupRect, Color.blue);
    }

    #endregion MonoBehaviour!!!

    #region OnAddBuilding:
    public void OnAddBuilding(BuildingOnMapController building)
    {
        if (building == null) return;

        LogMsg($"AddBuilding {building.buildingId} to group: {gameObject.name}");
        if (!Intersect(building))
        {
            return;
        }

        //Add buildint to group
        if (!buildings.Contains(building.buildingId))
        {
            LogMsg($"AddBuilding 2 {building.buildingId} to group: {gameObject.name}");
            buildings.Add(building.buildingId);
        }
    }


    #region Check Intersect
    private bool Intersect(BuildingOnMapController building)
    {
        if (building == null || limit_min == null || limit_max == null)
        {
            return false;
        }

        var buildingBounds = building.CalculateBounds();
        return Intersect(groupRect, buildingBounds);
    }

    private static bool Intersect(Rect rect, Bounds bounds)
    {
        return IntersectX(rect, bounds) && IntersectY(rect, bounds);
    }

    private static bool IntersectX(Rect rect, Bounds bounds)
    {
        var dist = Mathf.Abs(rect.position.x - bounds.center.x);
        return dist < rect.width / 2 + bounds.extents.x;
    }

    private static bool IntersectY(Rect rect, Bounds bounds)
    {
        var dist = Mathf.Abs(rect.position.y - bounds.center.y);
        return dist < rect.height / 2 + bounds.extents.y;
    }

    //Intersect Rect with Rect:

    private static bool Intersect(Rect rect1, Rect rect2)
    {
        return IntersectX(rect1, rect2) && IntersectY(rect1, rect2);
    }

    private static bool IntersectX(Rect rect1, Rect rect2)
    {
        var dist = Mathf.Abs(rect1.position.x - rect2.position.x);
        return dist < rect1.width / 2 + rect2.width / 2;
    }

    private static bool IntersectY(Rect rect1, Rect rect2)
    {
        var dist = Mathf.Abs(rect1.position.y - rect2.position.y);
        return dist < rect1.height / 2 + rect2.height / 2;
    }

    #endregion Check Intersect!!!
    #endregion OnAddBuilding!!!

    #region Checking Group Active State

    public void UpdateGroupIntersectCamera(Rect cameraRect)
    {
        isActive.Value = IsIntersectCameraBounds(cameraRect);
    }

    private bool IsIntersectCameraBounds(Rect cameraRect)
    {
        return Intersect(cameraRect, groupRect);
    }

    #endregion Checking Group Active State!!!

    #region Utils:


    private Rect CalculateGroupBounds()
    {
        Vector2 center = (limit_min.position + limit_max.position) * 0.5f;
        var width = Mathf.Abs(limit_max.position.x - limit_min.position.x);
        var height = Mathf.Abs(limit_max.position.y - limit_min.position.y);
        return new Rect(center.x, center.y, width, height);
    }

    private void LogMsg(string msg)
    {
        return;
        Debug.Log($"<color=white>[BuildingCullingGroup]</color> {msg}");
    }
    #endregion Utils!!!

    #region Gizmos

    private static void DrawRect(Rect rect, Color color)
    {
        var p1 = rect.position + 0.5f * new Vector2(-rect.width, -rect.height);
        var p2 = rect.position + 0.5f * new Vector2(-rect.width, rect.height);
        var p3 = rect.position + 0.5f * new Vector2(rect.width, rect.height);
        var p4 = rect.position + 0.5f * new Vector2(rect.width, -rect.height);
        DrawLine(p1, p2, color);
        DrawLine(p2, p3, color);
        DrawLine(p3, p4, color);
        DrawLine(p4, p1, color);
    }

    private static void DrawLine(Vector2 p1, Vector2 p2, Color color)
    {
        var cacheColor = Gizmos.color;
        Gizmos.color = color;

        Gizmos.DrawLine(p1, p2);

        Gizmos.color = cacheColor;
    }

    #endregion Gizmos!!!
}
