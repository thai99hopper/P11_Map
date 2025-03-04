using UnityEngine;

public partial class McOnMapController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform model;
    [SerializeField] Transform movePos;
    [SerializeField] Transform emoPos;

    [Header("Idle")]
    [SerializeField] float minFreeTime = 5f;
    [SerializeField] float maxFreeTime = 10f;
    [SerializeField] float idleTime;

    [Header("Movement")]
    [SerializeField] float speedMove = 5f;
    [SerializeField] bool isMove = false;
    [SerializeField] bool isMoveAnim = false;
    [SerializeField] Transform posWillMove; 
    [SerializeField] int movePosIdx;

    [Header("Emo")]
    [SerializeField] float minEmoTime = 30f;
    [SerializeField] float maxEmoTime = 45f;
    [SerializeField] bool isEmo = false;
    [SerializeField] float emoTime;

    [Header("Other")]
    bool isOutsideScreen = false;
    [SerializeField] Vector3 firstAngleModel;
    [SerializeField] int idleIdx;

    [Header("Debug")]
    [SerializeField] bool isAutoEmo = false;           

    private void Start()
    {
        firstAngleModel = model.localEulerAngles;
        SetupMovePos();
        ChangedIdleAnim();
        RandomFreeTime();
        isOutsideScreen = IsObjectOutsideCamera();
    }

    private void Update()
    {
        //UpdateOutsideCam();

        if (!isMove && !isEmo)
        {
            idleTime -= Time.deltaTime;

            if (idleTime <= 0)
            {
                var status = Random.Range(0, 2);
                if (status == 0 || isAutoEmo)
                    StartEmo();
                else
                    StartMovement();
            }
        }

        if (isMove && isMoveAnim)
        {
            UpdateMovement();
        }

        if (!isMove && isEmo)
        {
            UpdateEmo();
        }
    }

    #region movement
    void StartMovement()
    {
        SetActiveMoveAni(true);

        int indexNext;
        do
        {
            indexNext = Random.Range(1, movePos.childCount + 1);
        } while (indexNext == movePosIdx);

        movePosIdx = indexNext;
        posWillMove = movePos.Find($"pos-{movePosIdx}");
    }

    void UpdateMovement()
    {
        if (Vector3.SqrMagnitude(posWillMove.position - model.position) < 0.1f * 0.1f)
        {
            if (isEmo)
            {
                SetActiveEmoAni(true);
            }

            SetActiveMoveAni(false);
            RandomFreeTime();

            var angleX = isEmo ? emoPos.localEulerAngles.x : firstAngleModel.x;
            var angleY = isEmo ? emoPos.localEulerAngles.y : firstAngleModel.y;
            var angleZ = isEmo ? emoPos.localEulerAngles.z : firstAngleModel.z;
            model.localRotation = Quaternion.Euler(angleX, angleY, angleZ);
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

    void SetActiveMoveAni(bool val)
    {
        animator.SetBool("move", val);
        isMove = val;
    }
    #endregion

    #region emo
    void StartEmo()
    {
        isEmo = true;
        RandomEmoTime();
        posWillMove = emoPos;
        SetActiveMoveAni(true);
    }

    void UpdateEmo()
    {
        emoTime -= Time.deltaTime;

        if (emoTime < 0)
        {
            isEmo = false;
            SetActiveEmoAni(false);
            StartMovement();
        }
    }

    void RandomEmoTime()
    {
        emoTime = Random.Range(minEmoTime, maxEmoTime);
    }

    void SetActiveEmoAni(bool val)
    {
        animator.SetBool("emo", val);
    }
    #endregion

    #region outside
    bool IsObjectOutsideCamera()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewportPos.x < 0 || viewportPos.x > 1 ||
               viewportPos.y < 0 || viewportPos.y > 1 ||
               viewportPos.z < 0;
    }

    void UpdateOutsideCam()
    {
        var isOutsideScreen = IsObjectOutsideCamera();
        if (!isOutsideScreen && this.isOutsideScreen && !isMove)
        {
            //EnableEmoAnim();
        }
        this.isOutsideScreen = isOutsideScreen;
    }
    #endregion

    void SetupMovePos()
    {
        foreach (Transform item in movePos)
        {
            var posCurrent = item.localPosition;
            item.localPosition = new Vector3(posCurrent.x, posCurrent.y, posCurrent.y);
        }

        //var emoPosCurrent = emoPos.localPosition;
        //emoPos.localPosition = new Vector3(emoPosCurrent.x, emoPosCurrent.y, emoPosCurrent.y);
    }

    void RandomFreeTime()
    {
        idleTime = Random.Range(minFreeTime, maxFreeTime);
        isMoveAnim = false;
    }

    void ChangedIdleAnim()
    {
        if (idleIdx != 1 || isMove)
        {
            idleIdx = 1;
        }
        else
        {
            idleIdx = Random.Range(2, 5);
        }
        animator.SetInteger("idle-type", idleIdx);
    }

    public void OnTriggerEndIdleAnim()
    {
        ChangedIdleAnim();
    }

    public void OnTriggerStartMove()
    {
        isMoveAnim = true;
    }
}
