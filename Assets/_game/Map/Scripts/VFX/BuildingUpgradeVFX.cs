using Cysharp.Threading.Tasks;
using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUpgradeVFX : MonoBehaviour
{
    /// <summary>
    /// dont change the animation Name
    /// </summary>
    public bool isPart;

    public Transform parentScale;
    public Transform parentCatHammer;

    /// <summary>
    /// use to get lifetime of effect smoke
    /// </summary>
    public ParticleSystem mainParticle;
    public ParticleSystem subParticle;

    public float lifeTime;

    public Transform catHammer;

    /// <summary>
    /// left is positive else negative
    /// </summary>
    public Vector3 rotationDefault;

    /// <summary>
    /// y < 0 using anim back else use anim front
    /// </summary>
    public Vector3 rangePosMin;
    /// <summary>
    /// x < 0 is left else right
    /// </summary>
    public Vector3 rangePosMax;

    public async UniTask RunVFX()
    {
        var baselLocalScaleCat = parentCatHammer.localScale;
        var localScaleParent = parentScale.localScale;
        var localScaleCat = parentCatHammer.localScale;
        localScaleCat.x = localScaleCat.x / localScaleParent.x;
        localScaleCat.y = localScaleCat.y / localScaleParent.y;
        localScaleCat.z = localScaleCat.z / localScaleParent.z;

        parentCatHammer.localScale = localScaleCat;

        lifeTime = mainParticle.main.startLifetime.constant;

        RandomisePositionCatHammer().Forget();

        catHammer.gameObject.SetActive(true);

        await UniTask.Delay((int)(lifeTime * 1000));
        catHammer.gameObject.SetActive(false);

        parentCatHammer.localScale = baselLocalScaleCat;
        if (subParticle != null)
        {
            await UniTask.Delay((int)(subParticle.main.startLifetime.constant * 1000));
        }
    }

    private async UniTask RandomisePositionCatHammer()
    {
        var step = 4;
        var lifeTimeStep = lifeTime / step;

        var isLeftArray = new List<bool>() { false, true, false, true };


        for (int i = 0; i < step; i++)
        {
            RandomCatTransform(isLeftArray[i]);
            await UniTask.Delay((int)(lifeTimeStep * 1000));
        }
    }

    private void RandomCatTransform(bool isLeft)
    {
        var middle = Vector3.zero;

        var z = 0f;

        if (isLeft)
        {
            z = Random.Range(rangePosMin.z, middle.z - Mathf.Abs(rangePosMin.z / 2f));
        }
        else
        {
            z = Random.Range(middle.z + Mathf.Abs(rangePosMax.z / 2f), rangePosMax.z);
        }

        //wanna 1 cat per side at least so rane z bit diff;
        var y = Random.Range(rangePosMin.y, rangePosMax.y);

        UpdateAttributeCatHammer(catHammer, y, z);
    }

    private void UpdateAttributeCatHammer(Transform catHammer, float y, float z)
    {
        var localPos = catHammer.transform.localPosition;
        localPos.y = y;
        localPos.z = z;
        catHammer.transform.localPosition = localPos;
        catHammer.transform.localEulerAngles = GetRotateCorrect(catHammer, z);

        if (isPart) return;
        catHammer.GetComponent<SkeletonAnimation>().AnimationName = GetAnimationName(y);
    }

    private Vector3 GetRotateCorrect(Transform catHammer, float zValue)
    {
        var rotation = catHammer.transform.localEulerAngles;
        if (zValue < 0)
        {
            rotation.y = rotationDefault.y;
        }
        else
        {
            rotation.y = -rotationDefault.y;
        }
        return rotation;
    }

    private string GetAnimationName(float yValue)
    {
        if (yValue < 0)
            return "back";
        return "front";
    }
}
