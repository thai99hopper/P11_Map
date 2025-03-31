using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IdleAnimationObject
{
    public int idleIdx;
    public List<GameObject> models;
}

[System.Serializable]
public class InteractiveModel
{
    public Transform interactiveModel;
    public Transform interactiveModelEmo;
}

public partial class McOnMapController : MonoBehaviour
{
    #region base
    [SerializeField] string buildingId;
    bool isCanChangeToEmo = true;
    bool isEnableModel = true;
    bool isBuildingMaxLv = false;
    [SerializeField] Animator animator;
    [SerializeField] Transform model;
    [SerializeField] Transform modelEmo;
    [SerializeField] Transform movePos;

    [Header("Interactive Model (Can Null)")]
    [SerializeField] List<InteractiveModel> interactiveModels;

    [Header("Idle")]
    [SerializeField] float minFreeTime = 40f;
    [SerializeField] float maxFreeTime = 55f;
    [SerializeField] float idleTime;
    [SerializeField] List<IdleAnimationObject> idleAnimationObjects;

    [Header("Movement")]
    [SerializeField] float speedMove = 5f;
    [SerializeField] bool isMove = false;
    [SerializeField] bool isMoveAnim = false;
    [SerializeField] Transform posWillMove;
    [SerializeField] int movePosIdx;

    [Header("Emo")]
    [SerializeField] bool isEmo = false;
    [SerializeField] int buildingLevelAllowEmo;
    [SerializeField] int buildingPartAllowEmo;

    [Header("Emo Move (Can Null)")]
    [SerializeField] float speedEmoMove = 5f;
    [SerializeField] Transform moveEmoPos;
    [SerializeField] Transform posWillMoveEmo;
    [SerializeField] bool isRotationEmoMoveEnd = false;
    [SerializeField] bool isStartPoint = true;
    [SerializeField] bool isEmoMove = false;
    Transform GetFirstPositionEmoMove { get => moveEmoPos == null ? modelEmo.transform : moveEmoPos.Find($"pos-1"); }
    bool isHaveEmoMove { get => moveEmoPos != null; }

    [Header("Emo Stand (Can Null - if emo have move, stand can't use)")]
    [SerializeField] Transform standEmoPos;
    bool isHaveEmoStand { get => standEmoPos != null; }

    [Header("Other")]
    [SerializeField] bool isOutsideScreen = false;
    [SerializeField] Vector3 firstAngleModel;
    [SerializeField] int idleIdx;

    bool isHaveEmoSpecial => isHaveEmoMove || isHaveEmoStand;

    void SetAnimBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    void SetAnimInteger(string name, int value)
    {
        animator.SetInteger(name, value);
    }
    #endregion

    #region life-cycle
    private void Start()
    {
        if (isBuildingMaxLv)
            isEmo = true;

        firstAngleModel = model.localEulerAngles;
        SetupMovePos();
        ChangedIdleAnim();
        RandomFreeTime();
        SetActiveModel();
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

        if (isMove)
            SetActiveMoveAni(true);
        else
            SetIsMoveAnim(false);

        if (isMove && isMoveAnim)
        {
            UpdateMovement();
        }

        if (isEmoMove)
        {
            UpdateMovementEmo();
        }
    }
    #endregion

    #region emo
    void UpdateMovementEmo()
    {
        if (moveEmoPos == null) return;
        var normalized = (posWillMoveEmo.position - modelEmo.position).normalized;
        modelEmo.position += speedEmoMove * normalized * Time.deltaTime;
    }

    void UpdateWillMoveEmo()
    {
        if (moveEmoPos == null) return;
        int indexPoint = isStartPoint ? 2 : 1;
        posWillMoveEmo = moveEmoPos.Find($"pos-{indexPoint}");

        isStartPoint = !isStartPoint;
        if (isRotationEmoMoveEnd)
        {
            var currentScale = modelEmo.localScale;
            modelEmo.localScale = new Vector3(currentScale.x * -1, currentScale.y, currentScale.z);
        }
    }
    #endregion

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
    bool IsObjectOutsideCamera(Vector3 viewportPos)
    {
        return viewportPos.x < 0 || viewportPos.x > 1 ||
            viewportPos.y < 0 || viewportPos.y > 1 ||
            viewportPos.z < 0;
    }

    bool IsObjectOutsideCamera()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(model.transform.position);
        Vector3 viewportEmoPos = Camera.main.WorldToViewportPoint(GetFirstPositionEmoMove.position);

        var isOutside = true;
        isOutside &= IsObjectOutsideCamera(viewportPos);
        isOutside &= IsObjectOutsideCamera(viewportEmoPos);

        foreach (InteractiveModel item in interactiveModels)
        {
            if (item.interactiveModel != null)
            {
                Vector3 viewportInteractivePos = Camera.main.WorldToViewportPoint(item.interactiveModel.transform.position);
                isOutside &= IsObjectOutsideCamera(viewportInteractivePos);
            }

            if (item.interactiveModelEmo != null)
            {
                Vector3 viewportInteractiveEmoPos = Camera.main.WorldToViewportPoint(item.interactiveModelEmo.transform.position);
                isOutside &= IsObjectOutsideCamera(viewportInteractiveEmoPos);
            }
        }

        if (standEmoPos != null)
        {
            foreach(Transform child in standEmoPos)
            {
                Vector3 viewportStandEmoPos = Camera.main.WorldToViewportPoint(child.position);
                isOutside &= IsObjectOutsideCamera(viewportStandEmoPos);
            }    
        }

        return isOutside;
    }

    void UpdateOutsideCam()
    {
        var isOutsideScreen = IsObjectOutsideCamera();
        if (isOutsideScreen && !this.isOutsideScreen)
        {
            isEmo = !isEmo;
            if(!isCanChangeToEmo)
                isEmo = false;

            if(isBuildingMaxLv)
                isEmo = true;

            SetActiveModel();
        }
        this.isOutsideScreen = isOutsideScreen;
    }
    #endregion

    #region idle
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

        SetIdleAnim();
    }

    void SetIdleAnim()
    {
        foreach (var idleAnim in idleAnimationObjects)
        {
            var isActive = idleAnim.idleIdx == idleIdx;

            foreach (var model in idleAnim.models)
            {
                if (model == null)
                    Debug.LogError($"[MC Missing] missing idle model {this.name}");
                else
                    model.gameObject.SetActive(isActive);
            }
        }

        SetAnimInteger("idle-type", idleIdx);
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

        //if (moveEmoPos != null)
        //{
        //    foreach (Transform item in moveEmoPos)
        //    {
        //        var posCurrent = item.localPosition;
        //        item.localPosition = new Vector3(posCurrent.x, posCurrent.y, posCurrent.y);
        //    }
        //}

        //var emoPosCurrent = emoPos.localPosition;
        //emoPos.localPosition = new Vector3(emoPosCurrent.x, emoPosCurrent.y, emoPosCurrent.y);
    }

    void RandomFreeTime()
    {
        idleTime = Random.Range(minFreeTime, maxFreeTime);
    }

    void SetActiveModel()
    {
        model.gameObject.SetActive(!isEmo && isEnableModel);
        modelEmo.gameObject.SetActive(isEmo && isEnableModel);

        if (isEmo && isHaveEmoSpecial)
        {
            isStartPoint = true;
            isEmoMove = false;

            var posWillMoveEmo = transform;
            if (isHaveEmoMove)
            {
                posWillMoveEmo = moveEmoPos.Find($"pos-1");
            }
            else if(isHaveEmoStand)
            {
                var indexPos = Random.Range(1, standEmoPos.childCount + 1);
                posWillMoveEmo = standEmoPos.Find($"pos-{indexPos}");
            }    

            modelEmo.position = posWillMoveEmo.position;

            if (isRotationEmoMoveEnd)
            {
                var currentScale = modelEmo.localScale;
                modelEmo.localScale = new Vector3(System.Math.Abs(currentScale.x), currentScale.y, currentScale.z);
            }
        }

        if (!isEmo)
        {
            idleIdx = 1;
            isMove = false;
            isMoveAnim = false;
            SetIdleAnim();
        }
    }
    #endregion

    #region trigger
    public void OnTriggerEndIdleAnim()
    {
        ChangedIdleAnim();
    }

    public void OnTriggerStartMove()
    {
        SetIsMoveAnim(true);
    }

    public void OnTriggerStartEmoMove()
    {
        UpdateWillMoveEmo();
        isEmoMove = true;
    }

    public void OnTriggerEndEmoMove()
    {
        isEmoMove = false;
    }
    #endregion
}
