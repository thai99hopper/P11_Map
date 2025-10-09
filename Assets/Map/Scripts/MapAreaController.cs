
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class MapAreaController : MonoBehaviour
{
    [SerializeField] private MapArea mapArea;

    //Attributes:
    [SerializeField] private GameObject skinned_objects; // MC presenter parent
    private List<McOnMapPresenter> mcPresenters = new List<McOnMapPresenter>();

    public MapAreaSetupChecker SetupChecker { get { return setupChecker; } set { setupChecker = value; } }
    private MapAreaSetupChecker setupChecker;

    private void Awake()
    {
        setupChecker = new MapAreaSetupChecker(mapArea);
        RetrieveAllMcPresenters();
    }


    private void RetrieveAllMcPresenters()
    {
        if (skinned_objects == null)
        {
            Debug.LogWarning($"MapAreaController > RetrieveAllMcPresenters > skinned_objects is not assigned in inspector!");
            return;
        }

        mcPresenters.Clear();
        var presenters = skinned_objects.GetComponentsInChildren<McOnMapPresenter>(true);
        if (presenters == null || presenters.Length == 0)
        {
            Debug.LogWarning($"MapAreaController > RetrieveAllMcPresenters > No McOnMapPresenter found in skinned_objects!");
            return;
        }

        mcPresenters.AddRange(presenters);
    }
}