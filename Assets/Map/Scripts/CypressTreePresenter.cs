using System;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class CypressTreePresenter : MonoBehaviour
{
    private const string ANIM_LOOP = "loop";
    private const string ANIM_DISSAPEAR = "dissapear";
    
    [SerializeField] private SkeletonAnimation skelAnim;
    [SerializeField] private bool debug; 
    
    #region UNITY_EDITOR

    private void Reset()
    {
        skelAnim = GetComponent<SkeletonAnimation>(); 
    }

    #endregion

    private void Awake()
    {
        gameObject.SetActive(debug);
    }

    public async UniTask DoTreeDisappear()
    {
        await SetSkeletonAnimation(ANIM_DISSAPEAR, false); 
    }

    private async UniTask SetSkeletonAnimation(string animName,bool loop)
    {
        skelAnim.ClearState();
        skelAnim.enabled = false;
        skelAnim.loop = loop; 

        skelAnim.AnimationName = animName;
        await UniTask.DelayFrame(1);
        skelAnim.enabled = true;
    }
    
    public void ShowHideTree(bool enabled)
    {
        gameObject.SetActive(enabled);
        if (enabled)
            SetSkeletonAnimation(ANIM_LOOP, true).Forget(); 
    }
}
