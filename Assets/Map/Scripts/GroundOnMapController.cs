using System;
using UnityEngine;

public partial class GroundOnMapController : MonoBehaviour
{
    public enum GroundZone 
    {
        zone1, 
        zone2,
        zone3,
    }

    [SerializeField] private GroundZone zone;

    private void Start()
    {
        SetupDev();
    }
}
