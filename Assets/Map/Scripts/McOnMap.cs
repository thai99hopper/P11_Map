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

    public void OnTriggerStartEmoMove()
    {
        mcController.OnTriggerStartEmoMove();
    }

    public void OnTriggerEndEmoMove()
    {
        mcController.OnTriggerEndEmoMove();
    }
}
