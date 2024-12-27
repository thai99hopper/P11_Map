using UnityEngine;

public class TargetFramerate : MonoBehaviour
{
    [SerializeField] public int targetFrameRate = 60; 
    void Start()
    {
        Application.targetFrameRate = targetFrameRate; 
    }
}
