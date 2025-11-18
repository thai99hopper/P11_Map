using DG.Tweening;
using UnityEngine;

public class AnimatorTextOnMapController_baker : AnimatorTextOnMapController
{
    [SerializeField] private GameObject objWhoosh;

    private Vector3 beginPos = Vector3.negativeInfinity;
    private Sequence lastSequence; 
    public void DoAnimationWhoosh()
    {
        if (lastSequence != null)
        {
            lastSequence.Restart();
            lastSequence.Kill();
        }
            
        
        if(beginPos == Vector3.negativeInfinity)
            beginPos = objWhoosh.transform.position;
        objWhoosh.transform.position = beginPos;
        
        lastSequence = animation.Run(objWhoosh); 
    }
}
