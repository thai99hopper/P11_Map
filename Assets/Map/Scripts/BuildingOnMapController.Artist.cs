using System;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public partial class BuildingOnMapController : MonoBehaviour
{
    [Serializable]
    public class BuildingBounds
    {
        public float upOffset;
        public float downOffset;
        public float leftOffset;
        public float rightOffset;
        public bool debugBounds = true;
    }
    
    public string buildingId;
    public SkeletonAnimation skeletonAnimation;
    public GameplayButton gameplayButton;
    public Transform popupUpgradePos;
    public Transform parentPartsPos;
    public Transform parentVfxsPos;
    public List<BuildingSlotObject> slotObjects;
    public BuildingBounds buildingBounds;

    public Bounds CalculateBounds()
    {
        var leftUp = transform.position + new Vector3(-buildingBounds.leftOffset, buildingBounds.upOffset, 0);
        var rightBottom = transform.position + new Vector3(buildingBounds.rightOffset, -buildingBounds.downOffset, 0);
        var center = (leftUp + rightBottom) / 2f;
        var sz = new Vector2(Mathf.Abs(leftUp.x - rightBottom.x), Mathf.Abs(leftUp.y - rightBottom.y));
        return new Bounds(center, sz);
    }

    #region gizmos

    private void OnDrawGizmosSelected()
    {
        if (!buildingBounds.debugBounds)
        {
            return;
        }

        DrawBounds(CalculateBounds());
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
    
    private static void DrawLine(Vector2 p1, Vector2 p2)
    {
        var color = Gizmos.color;
        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(p1, p2);
        
        Gizmos.color = color;
    }
    
    #endregion
}