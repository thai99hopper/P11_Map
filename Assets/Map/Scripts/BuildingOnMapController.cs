using UnityEngine;

public partial class BuildingOnMapController : MonoBehaviour
{
    [SerializeField] int levelTest;
    [SerializeField] int partTest;
    private void Update()
    {
        SetSkeletonAnimation(levelTest, partTest);
    }
    private void SetSkeletonAnimation(int level, int part)
    {
        var strPart = part == 0 ? "" : $"_{part}";
        level += 0;
        skeletonAnimation.AnimationName = $"building_lv{level}{strPart}";
        foreach (var slotObject in slotObjects)
        {
            slotObject.SetActiveObject(level, part);
        }
    }
}