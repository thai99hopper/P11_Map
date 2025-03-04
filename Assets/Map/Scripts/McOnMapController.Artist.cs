using UnityEngine;
using UnityEngine.UIElements;

public partial class McOnMapController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform model;
    [SerializeField] Transform movePos;
    [SerializeField] float speedMove;
    [SerializeField] float rotateSpeed;
    float rangeAnim = 3;
    float timeStartIdleAnim;

    [SerializeField] float freeTime;
    [SerializeField] bool isMove = false;
    [SerializeField] Transform posWillMove;

    bool isOutsideScreen = false;
    [SerializeField] float firstAngleModel;

    private void Start()
    {
        firstAngleModel = model.localEulerAngles.y;
        SetupMovePos();
        ChangedIdleAnim();
        RandomFreeTime();
        isOutsideScreen = IsObjectOutsideCamera();
    }

    private void Update()
    {
        UpdateOutsideCam();

        if (!isMove)
        {
            freeTime -= Time.deltaTime;

            if (freeTime <= 0)
                StartMovement();
        }
        isMove = freeTime < 0;

        if (!isMove)
        {
            if (IsCompleteIdleAnimCurrent())
                ChangedIdleAnim();
        }
        else
        {
            UpdateMovement();
        }
    }

    void SetupMovePos()
    {
        foreach(Transform item in movePos)
        {
            var posCurrent = item.localPosition;
            item.localPosition = new Vector3(posCurrent.x, posCurrent.y, posCurrent.y);
        }    
    }    

    void UpdateOutsideCam()
    {
        var isOutsideScreen = IsObjectOutsideCamera();
        if (!isOutsideScreen && this.isOutsideScreen && !isMove)
        {
            EnableEmoAnim();
        }
        this.isOutsideScreen = isOutsideScreen;
    }    

    void StartMovement()
    {
        SetActiveMoveAni(true);

        var index = Random.Range(1, movePos.childCount+1);
        posWillMove = movePos.Find($"pos-{index}");
    }    

    void UpdateMovement()
    {
        if (Vector3.SqrMagnitude(posWillMove.position - model.position) < 0.1f * 0.1f)
        {
            SetActiveMoveAni(false);
            RandomFreeTime();
            model.localRotation = Quaternion.Euler(0, firstAngleModel, 0);
        }
        else
        {
            var normalized = (posWillMove.position - model.position).normalized;
            model.position += speedMove * normalized * Time.deltaTime;

            UpdateRotation();
        }    
    }

    protected void UpdateRotation()
    {
        var normalized = (posWillMove.localPosition - model.localPosition).normalized;

        float targetYAngle = Mathf.Atan2(normalized.x, normalized.z) * Mathf.Rad2Deg;
        model.localRotation = Quaternion.Euler(0, targetYAngle, 0);
    }

    bool IsObjectOutsideCamera()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewportPos.x < 0 || viewportPos.x > 1 ||
               viewportPos.y < 0 || viewportPos.y > 1 ||
               viewportPos.z < 0;
    }

    void RandomFreeTime()
    {
        freeTime = Random.Range(7f, 15f);
    }

    bool IsCompleteIdleAnimCurrent()
    {
        return Time.realtimeSinceStartup - timeStartIdleAnim >= rangeAnim;
    }      

    void ChangedIdleAnim()
    {
        timeStartIdleAnim = Time.realtimeSinceStartup;
        var val = Random.Range(1, 5);
        animator.SetInteger("idle-type", val);
    }    

    void EnableEmoAnim()
    {
        animator.SetTrigger("emo");
    }    

    void SetActiveMoveAni(bool val)
    {
        animator.SetBool("move", val);
    }    
}
