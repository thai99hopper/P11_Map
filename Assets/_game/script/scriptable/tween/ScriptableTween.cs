using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ScriptableTween", menuName = "Scriptable/TweenScriptable", order = 1)]
public class ScriptableTween : ScriptableObject
{
    [Header("Base Configs")]
    [SerializeField] float _duration = 0.22f;

    [SerializeField] bool isLoop;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(isLoop))]
#endif
    [SerializeField] int _loop = int.MaxValue;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(isLoop))]
#endif
    [SerializeField] LoopType _loopType = LoopType.Yoyo;

    [Space(5f)]
    [Header("Scale")]
    [SerializeField] bool enableScale;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableScale))]
#endif
    [SerializeField] Vector3 _endScale = Vector3.one;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableScale))]
#endif
    [SerializeField] Ease _easeScale = Ease.Unset;

    [Space(5f)]
    [Header("Color")]
    public bool enableColor;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableColor))]
#endif
    [SerializeField] Color _endColor = Color.white;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableColor))]
#endif
    [SerializeField] Ease _easeColor = Ease.Unset;

    [Space(5f)]
    [Header("Move")]
    public bool enableMove;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableMove))]
#endif
    public bool returnMove;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableMove))]
#endif
    public bool localMove;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(localMove))]
#endif
    public bool addLocalMove;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableMove))]
#endif
    [SerializeField] Vector3 _endVectorMove;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableMove))]
#endif
    [SerializeField] public bool enableRandomMove;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableRandomMove))]
#endif
    [SerializeField] Vector3 _randomVectorMove;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableMove))]
#endif
    [SerializeField] Ease _easeMove = Ease.Unset;

    [Space(5)]
    [Header("Fill Amount")]
    public bool enableFillAmount;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableFillAmount))]
#endif
    [Range(0, 1f)][SerializeField] float _endFillAmount = default;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableFillAmount))]
#endif
    [SerializeField] Ease _easeFillAmount = Ease.Unset;


    [Space(5f)]
    [Header("Interval Tween")]
    public bool enableInterval;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableInterval))]
#endif
    [SerializeField] float _interval;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableInterval))]
#endif
    [SerializeField] Ease _easeInterval = Ease.Unset;

    [Space(5)]
    [Header("Fade")]
    public bool enableFade;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableFade))]
#endif
    public float endFade = default;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableFade))]
#endif
    public Ease easeFade = Ease.Unset;

    [Space(5)]
    [Header("Rotate")]
    public bool enableRotate;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableRotate))]
#endif
    public Vector3 endRotate = default;
#if UNITY_EDITOR
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(enableRotate))]
#endif
    public Ease easeRotate = Ease.Unset;

    public bool IsEnableInterval => enableInterval;
    public bool IsLoop => isLoop;
    public bool IsEnableRandomMove => enableRandomMove;
    public bool IsReturnMove => returnMove;
    public float duration => _duration;
    public int loop => _loop;
    public LoopType loopType => _loopType;
    public Vector3 endScale => _endScale;
    public Color endColor => _endColor;
    public Vector3 endVectorMove => _endVectorMove;
    public Vector3 randomVectorMove => _randomVectorMove;
    public float endFillAmount => _endFillAmount;
    public float interval => _interval;



    public Tweener DoTween(GameObject obj, Vector3 oldPos = default)
    {
        Tweener tw = null;
        if (enableColor)
        {
            if (obj.TryGetComponent<Image>(out var image))
                tw = image.DOColor(endColor, duration).SetEase(_easeColor);
            if (obj.TryGetComponent<TextMeshProUGUI>(out var text))
                tw = text.DOColor(endColor, duration).SetEase(_easeColor);
            if (obj.TryGetComponent<SpriteRenderer>(out var sr))
                tw = sr.DOColor(endColor, duration).SetEase(_easeColor);
            if(obj.TryGetComponent<TextMeshPro>(out var tmp))
                tw = tmp.DOColor(endColor, duration).SetEase(_easeColor);
        }

        if (enableFade)
        {
            if (obj.TryGetComponent<Image>(out var image))
                tw = image.DOFade(endFade, duration).SetEase(easeFade);
            if (obj.TryGetComponent<TextMeshProUGUI>(out var text))
                tw = text.DOFade(endFade, duration).SetEase(easeFade);
            if (obj.TryGetComponent<SpriteRenderer>(out var sr))
                tw = sr.DOFade(endFade, duration).SetEase(easeFade);
            if (obj.TryGetComponent<TextMeshPro>(out var tmp))
                tw = tmp.DOFade(endFade, duration).SetEase(easeFade);
        }

        if (enableFillAmount)
        {
            if (obj.TryGetComponent<Image>(out var image))
                tw = image.DOFillAmount(endFillAmount, duration).SetEase(_easeFillAmount);
        }

        if (enableInterval)
        {
            tw = DOTween.To(() => obj.transform.localScale, x => obj.transform.localScale = x, obj.transform.localScale, interval).SetEase(_easeInterval);
        }

        if (enableRotate)
            tw = DOTween.To(() => obj.transform.rotation, x => obj.transform.rotation = x, endRotate, duration).SetEase(easeRotate);

        if (enableScale)
            tw = DOTween.To(() => obj.transform.localScale, x => obj.transform.localScale = x, endScale, duration).SetEase(_easeScale);

        if (enableMove)
        {
            if (localMove)
            {
                var finalVector = endVectorMove;
                finalVector += addLocalMove ? obj.transform.localPosition : default;

                tw = obj.transform.DOLocalMove(finalVector, duration).SetEase(_easeMove);
            }
            else
            {
                var finalVector = IsEnableRandomMove ? new Vector3(Random.Range(endVectorMove.x, randomVectorMove.x), Random.Range(endVectorMove.y, randomVectorMove.y), Random.Range(endVectorMove.z, randomVectorMove.z)) : endVectorMove;
                finalVector += obj.transform.position;
                if (IsReturnMove)
                    finalVector = oldPos;

                tw = DOTween.To(() => obj.transform.position, x => obj.transform.position = x, finalVector, duration).SetEase(_easeMove);
            }

        }


        if (isLoop)
        {
            tw.SetLoops(loop, loopType);
        }

        return tw;
    }
}
