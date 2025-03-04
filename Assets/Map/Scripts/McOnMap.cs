using UnityEngine;

public class McOnMap : MonoBehaviour
{
    [SerializeField] McOnMapController mcController;

    public void OnTriggerEndIdleAnim()
    {
        mcController.OnTriggerEndIdleAnim();
    }

    public void OnTriggerStartMove()
    {
        mcController.OnTriggerStartMove();
    }    
}
