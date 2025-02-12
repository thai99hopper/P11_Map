
using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class SpineCuller : MonoBehaviour
{
    #region core

    public bool debugBounds;

    private List<BoxCollider2D> listColliders = new List<BoxCollider2D>();
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        RetrieveListColliders();
    }

    private void RetrieveListColliders()
    {
        var spines = GetComponentsInChildren<SkeletonAnimation>();
        foreach (var spine in spines)
        {
            var boxCollider = spine.GetComponent<BoxCollider2D>();
            if (boxCollider)
            {
                
                listColliders.Add(boxCollider);
            }
            else
            {
                throw new Exception($"game object {spine.gameObject.name} does not have BoxCollider2D component");
            }
        }
    }

    private void Update()
    {
        var cameraBounds = GetCameraBounds();
        foreach (var boxCollider2D in listColliders)
        {
            var boxBounds = GetBoxColliderBounds(boxCollider2D);
            var visible = Intersect(cameraBounds, boxBounds);
            boxCollider2D.gameObject.SetActive(visible);
        }
    }

    #endregion

    #region get bounds

    private Rect GetCameraBounds()
    {
        var height = mainCamera.orthographicSize * 2f;
        var width = height * mainCamera.aspect;
        var sz = new Vector2(width, height);
        var pos = mainCamera.transform.position;

        return new Rect(pos, sz);
    }

    private Rect GetBoxColliderBounds(BoxCollider2D boxCollider)
    {
        var scale = (Vector2)boxCollider.transform.lossyScale;
        var positiveScale = new Vector2(Mathf.Abs(scale.x), Mathf.Abs(scale.y));

        var sz = boxCollider.size * positiveScale;
        var offset = boxCollider.offset * scale;
        var pos = (Vector2)boxCollider.transform.position + offset;

        return new Rect(pos, sz);
    }

    #endregion

    #region check intersect

    private bool Intersect(Rect rect1, Rect rect2)
    {
        return IntersectX(rect1, rect2) && IntersectY(rect1, rect2);
    }

    private bool IntersectX(Rect rect1, Rect rect2)
    {
        var dist = Mathf.Abs(rect1.position.x - rect2.position.x);
        return dist < rect1.width / 2 + rect2.width / 2;
    }

    private bool IntersectY(Rect rect1, Rect rect2)
    {
        var dist = Mathf.Abs(rect1.position.y - rect2.position.y);
        return dist < rect1.height / 2 + rect2.height / 2;
    }

    #endregion

    #region gizmos

    private void OnDrawGizmos()
    {
        if (!debugBounds)
        {
            return;
        }

        DrawRect(GetCameraBounds());
        foreach (var boxCollider2D in listColliders)
        {
            DrawRect(GetBoxColliderBounds(boxCollider2D));
        }
    }

    private void DrawRect(Rect rect)
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
    
    private void DrawLine(Vector2 p1, Vector2 p2)
    {
        var color = Gizmos.color;
        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(p1, p2);
        
        Gizmos.color = color;
    }

    #endregion
}