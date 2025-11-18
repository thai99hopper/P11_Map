using System;
using TMPro;
using UnityEngine;

public partial class TextOnMapController : MonoBehaviour
{
    public TextMeshPro tmpAmount;
    public SoundOnMapType type;
    
    #region unity life-cycle
    
    private void Start()
    {
        Setup();
    }

    #endregion 

    partial void Setup(); 
}
