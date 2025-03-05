using UnityEngine;

public partial class McOnMapController : MonoBehaviour
{
    #region base
    [SerializeField] Animator animator;
    [SerializeField] Transform model;
    [SerializeField] Transform modelEmo;
    [SerializeField] Transform movePos;

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
    [SerializeField] bool isEmo = false;

    [Header("Other")]
    bool isOutsideScreen = false;
    [SerializeField] Vector3 firstAngleModel;
    [SerializeField] int idleIdx;

    [Header("Debug")]
    [SerializeField] bool isAutoEmo = false;

    void SetAnimBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    void SetAnimInteger(string name, int value)
    {
        animator.SetInteger(name, value);
    }
    #endregion

    private void Start()
    {
        firstAngleModel = model.localEulerAngles;
        SetupMovePos();
        ChangedIdleAnim();
        RandomFreeTime();
        UpdateModel();
        isOutsideScreen = IsObjectOutsideCamera();
    }

    private void Update()
    {
        UpdateOutsideCam();

        if (!isMove && !isEmo)
        {
            idleTime -= Time.deltaTime;

            if (idleTime <= 0)
            {
                StartMovement();
            }
        }

        if (isMove && isMoveAnim)
        {
            UpdateMovement();
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
            SetActiveMoveAni(false);
            RandomFreeTime();

            var angleX = firstAngleModel.x;
            var angleY = firstAngleModel.y;
            var angleZ = firstAngleModel.z;
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
        SetAnimBool("move", val);
        isMove = val;

        if (!val)
            SetIsMoveAnim(false);
    }

    void SetIsMoveAnim(bool val)
    {
        isMoveAnim = val;
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
        if (isOutsideScreen && !this.isOutsideScreen)
        {
            isEmo = !isEmo;
            UpdateModel();
        }
        this.isOutsideScreen = isOutsideScreen;
    }
    #endregion

    #region other
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

        SetAnimInteger("idle-type", idleIdx);
    }

    void UpdateModel()
    {
        model.gameObject.SetActive(!isEmo);
        modelEmo.gameObject.SetActive(isEmo);
    }    
    #endregion

    #region trigger
    public void OnTriggerEndIdleAnim()
    {
        SetIsMoveAnim(false);
        ChangedIdleAnim();
    }

    public void OnTriggerStartMove()
    {
        SetIsMoveAnim(true);
    }
    #endregion
}
