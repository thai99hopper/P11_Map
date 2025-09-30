
using UnityEngine;

namespace BuildingCulling
{
    public partial class SpineBuildingCuller
    {
        public bool isDebugMode = false;

        public void LogMsg(string message, string color = "white")
        {
            if (!isDebugMode)
            {
                return;
            }

            Debug.Log($"<color={color}>[BuildingCulling]</color> {message}");
        }

        public void LogWrn(string message, string color = "white")
        {
            if (!isDebugMode)
            {
                return;
            }
            Debug.LogWarning($"<color={color}>[BuildingCulling]</color> {message}");
        }

        public void LogErr(string message, string color = "white")
        {
            if (!isDebugMode)
            {
                return;
            }
            Debug.LogError($"<color={color}>[BuildingCulling]</color> {message}");
        }
    }
}

