using UnityEngine;

public class AnimatorTextOnMapController_clam_raker : AnimatorTextOnMapController
{
    [SerializeField] private GameObject textFirstBlub;
    [SerializeField] private GameObject textSecondBlub;
    
    public void DoAnimationFirstWhoosh()
    {
        animation.Run(textFirstBlub); 
    }
    
    public void DoAnimationSecondWhoosh()
    {
        animation.Run(textSecondBlub);
    }
}
