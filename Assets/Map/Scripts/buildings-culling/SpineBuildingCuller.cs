using System.Collections.Generic;
using UnityEngine;

public partial class SpineBuildingCuller : MonoBehaviour
{
    private static Vector2 ExtraDistance;

    #region core

    private class BuildingObjInfo
    {
        public BuildingOnMapController obj;
        public Bounds bounds;
    }
    
    public bool debugBounds;

    private readonly List<BuildingObjInfo> lBuildingObj = new();
    private Camera mainCamera;

    public static bool IsApply_CullingGroup => isApplyCullingGroup;
#if UNITY_IOS
    private static bool isApplyCullingGroup = false;
#else
    private static bool isApplyCullingGroup = true;
#endif

    private void Start()
    {
        RetrieveListBuildingObjs();

        if (isApplyCullingGroup)
        {
            SubscribesObservables();
        }
    }

    private void OnEnable()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {

        if (isApplyCullingGroup)
        {
            UpdateCullingGroupActiveState();
        }
        else
        {
            var cameraBounds = GetCameraBounds();
            ExtraDistance = cameraBounds.size / 2f;
            foreach (var obj in lBuildingObj)
            {
#if UNITY_IOS
                var visible = IntersectExtra(cameraBounds, obj.bounds);
#else
                var visible = Intersect(cameraBounds, obj.bounds);
#endif
                obj.obj.visibleRx.Value = visible;
            }
        }
    }

    #endregion

    #region private utils
    private void RetrieveListBuildingObjs()
    {
        var buildings = GetComponentsInChildren<BuildingOnMapController>();
        foreach (var building in buildings)
        {
            lBuildingObj.Add(new BuildingObjInfo()
            {
                obj = building,
                bounds = building.CalculateBounds(),
            });

            if (isApplyCullingGroup)
            {
                OnAddBuilding(building);
            }
        }
    }
    
    private Rect GetCameraBounds()
    {
        var height = mainCamera.orthographicSize * 2f;
        var width = height * mainCamera.aspect;
        var sz = new Vector2(width, height);
        var pos = mainCamera.transform.position;

        return new Rect(pos, sz);
    }
    
    #endregion
    
    #region check intersect

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

    #endregion


    #region check intersect extra

    private static bool IntersectExtra(Rect rect, Bounds bounds)
    {
        return IntersectExtraX(rect, bounds) && IntersectExtraY(rect, bounds);
    }

    private static bool IntersectExtraX(Rect rect, Bounds bounds)
    {
        var dist = Mathf.Abs(rect.position.x - bounds.center.x);
        return dist < rect.width / 2 + bounds.extents.x + ExtraDistance.x;
    }

    private static bool IntersectExtraY(Rect rect, Bounds bounds)
    {
        var dist = Mathf.Abs(rect.position.y - bounds.center.y);
        return dist < rect.height / 2 + bounds.extents.y + ExtraDistance.y;
    }

    #endregion

    #region gizmos

    private void OnDrawGizmos()
    {
        if (!debugBounds || !mainCamera)
        {
            return;
        }
        
        DrawRect(GetCameraBounds(), Color.red);
        foreach (var buildingObjInfo in lBuildingObj)
        {
            DrawBounds(buildingObjInfo.bounds, Color.green);
        }
    }

    private static void DrawBounds(Bounds bounds, Color color)
    {
        var min = bounds.min;
        var max = bounds.max;
        var p1 = new Vector2(min.x, min.y);
        var p2 = new Vector2(max.x, min.y);
        var p3 = new Vector2(max.x, max.y);
        var p4 = new Vector2(min.x, max.y);
        DrawLine(p1, p2, color);
        DrawLine(p2, p3, color);
        DrawLine(p3, p4, color);
        DrawLine(p4, p1, color);
    }
    
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
    
    #endregion
}
