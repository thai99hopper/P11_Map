
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public partial class BuildingOnMapController : MonoBehaviour
{
    public string buildingId;
    public SkeletonAnimation skeletonAnimation;
    public GameplayButton gameplayButton;
    public Transform popupUpgradePos;
    public Transform parentPartsPos;
    public List<BuildingSlotObject> slotObjects;
}