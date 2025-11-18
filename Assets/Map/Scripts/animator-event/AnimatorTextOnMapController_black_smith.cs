using DG.Tweening;
using UnityEngine;

public class AnimatorTextOnMapController_black_smith : AnimatorTextOnMapController
{
    [SerializeField] private GameObject objClang;

    private Vector3 beginPos = Vector3.negativeInfinity;
    private Sequence lastSequence; 
    public void DoAnimationClang()
    {
        if (lastSequence != null)
        {
            lastSequence.Restart();
            lastSequence.Kill();
        }
            
        
        if(beginPos == Vector3.negativeInfinity)
            beginPos = objClang.transform.position;
        objClang.transform.position = beginPos;
        
        lastSequence = animation.Run(objClang); 
    }
}
