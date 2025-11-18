using UnityEngine;

public class AnimatorTextOnMapController_fisher_man : AnimatorTextOnMapController
{
    [SerializeField] private GameObject firstSleepText;
    [SerializeField] private GameObject secondSleepText;


    public void DoAnimationFirstSleep()
    {
        animation.Run(firstSleepText); 
    }

    public void DoAnimationSecondSleep()
    {
        animation.Run(secondSleepText); 
    }
}
