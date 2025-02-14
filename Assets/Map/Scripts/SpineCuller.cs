using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using UnityEngine;

public class SpineCuller : MonoBehaviour
{
    #region core

    private class SpineObjInfo
    {
        public GameObject obj;
        public Bounds bounds;
    }

    public bool debugBounds;

    private List<SpineObjInfo> listSpineObjs = new List<SpineObjInfo>();
    private Camera mainCamera;

    private async UniTask Start()
    {
        mainCamera = Camera.main;

        //wait for Spine initialize mesh
        await UniTask.DelayFrame(1);
        
        RetrieveListSpineObjs();
    }

    private void Update()
    {
        var cameraBounds = GetCameraBounds();
        foreach (var obj in listSpineObjs)
        {
            var visible = Intersect(cameraBounds, obj.bounds);
            obj.obj.SetActive(visible);
        }
    }

    #endregion

    #region private utils

    private void RetrieveListSpineObjs()
    {
        var spines = GetComponentsInChildren<SkeletonAnimation>();
        foreach (var spine in spines)
        {
            var mesh = spine.GetComponent<MeshRenderer>();
            listSpineObjs.Add(new SpineObjInfo()
            {
                obj = mesh.gameObject,
                bounds = mesh.bounds,
            });
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

    #region gizmos

    private void OnDrawGizmos()
    {
        if (!debugBounds || !mainCamera)
        {
            return;
        }

        DrawRect(GetCameraBounds());
        foreach (var obj in listSpineObjs)
        {
            DrawBounds(obj.bounds);
        }
    }
    
    private static void DrawBounds(Bounds bounds)
    {
        var min = bounds.min;
        var max = bounds.max;
        var p1 = new Vector2(min.x, min.y);
        var p2 = new Vector2(max.x, min.y);
        var p3 = new Vector2(max.x, max.y);
        var p4 = new Vector2(min.x, max.y);
        DrawLine(p1, p2);
        DrawLine(p2, p3);
        DrawLine(p3, p4);
        DrawLine(p4, p1);
    }

    private static void DrawRect(Rect rect)
    {
        var p1 = rect.position + 0.5f * new Vector2(-rect.width, -rect.height);
        var p2 = rect.position + 0.5f * new Vector2(-rect.width, rect.height);
        var p3 = rect.position + 0.5f * new Vector2(rect.width, rect.height);
        var p4 = rect.position + 0.5f * new Vector2(rect.width, -rect.height);
        DrawLine(p1, p2);
        DrawLine(p2, p3);
        DrawLine(p3, p4);
        DrawLine(p4, p1);
    }
    
    private static void DrawLine(Vector2 p1, Vector2 p2)
    {
        var color = Gizmos.color;
        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(p1, p2);
        
        Gizmos.color = color;
    }

    #endregion
}