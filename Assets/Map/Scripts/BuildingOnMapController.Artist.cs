
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BuildingOnMapController : MonoBehaviour
{
    public string buildingId;
    public SkeletonAnimation skeletonAnimation;
    public GameplayButton gameplayButton;
    public Transform popupUpgradePos;
    public Transform parentPartsPos;
    public Transform parentVfxsPos;
    public List<BuildingSlotObject> slotObjects;

    public ParticleSystem building;
    public ParticleSystem small;


    public List<string> nameAnims = new List<string>();

    int index = 1;

    public bool up1, up2;

    public List<Transform> posChilds = new List<Transform>();

    static string TEST = "building_lv1";

    private void Start()
    {
        var animations = skeletonAnimation.skeletonDataAsset.GetSkeletonData(true).Animations;
        foreach (var anim in animations)
        {
            nameAnims.Add(anim.Name);
        }

        foreach (Transform child in parentPartsPos)
        {
            posChilds.Add(child);
        }

        skeletonAnimation.AnimationName = nameAnims[index];
    }

    private void Update()
    {
        if (up1 != up2 && index < nameAnims.Count)
        {
            index++;
            skeletonAnimation.AnimationName = nameAnims[index];
            up1 = !up1;

            if (TEST.Length < nameAnims[index].Length)
            {
                if (small == null) Debug.LogError("reference the object into building when want to test");
                var transo = Instantiate(small, posChilds[index]);
                transo.transform.localPosition = Vector3.zero;
            }
            else
            {
                if (building == null) Debug.LogError("reference the object into building when want to test");
                var transo = Instantiate(building, posChilds[index]);
                transo.transform.localPosition = Vector3.zero;
            }
        }
    }
}