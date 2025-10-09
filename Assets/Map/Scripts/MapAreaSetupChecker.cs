

using System.Collections.Generic;

public class MapAreaSetupChecker
{
	#region Core:
	public MapArea mapArea;

	//Init building
	private Dictionary<string, bool> buildingLoadStatus = new Dictionary<string, bool>();
	public bool IsBuildingsLoadFinished => _isBuildingsLoadFinished;
	public bool _isBuildingsLoadFinished = false;
	public bool IsCatCharacterLoadFinished => _isCatCharacterLoadFinished;
	private bool _isCatCharacterLoadFinished = false;
	public bool IsSetupFinished
	{
		get
		{
			return IsBuildingsLoadFinished && _isCatCharacterLoadFinished;
        }
    }

	public MapAreaSetupChecker(MapArea area)
	{
		mapArea = area;
		buildingLoadStatus = new Dictionary<string, bool>();
		_isBuildingsLoadFinished = false;
		_isCatCharacterLoadFinished = false;

    }

	#endregion Core!!!

	#region Init Building:

	public bool IsAnyBuildingLoading()
	{
		return buildingLoadStatus != null && buildingLoadStatus.Count > 0;
    }


    public void CheckingStartLoadBuilding(string buildingId)
	{
		if (_isBuildingsLoadFinished)
		{
			return;
		}

		if (buildingLoadStatus == null)
		{
			return;
		}

		if (buildingLoadStatus.Count == 0)
		{
			//TimeTracking.StartProcess(TrackingProcess.init_building, TrackingProcess.run_scene_map);
		}

		if (!buildingLoadStatus.ContainsKey(buildingId))
		{
			buildingLoadStatus.Add(buildingId, false);
		}
		else
		{
			buildingLoadStatus[buildingId] = false; // Reset the status if already exists
		}
	}

	public void CheckingFinishLoadBuilding(string buildingId)
	{
		if (_isBuildingsLoadFinished)
		{
			return;
		}

		if (buildingLoadStatus == null)
		{
			return;
		}

		if (buildingLoadStatus.ContainsKey(buildingId))
		{
			buildingLoadStatus[buildingId] = true;
		}

		if (CheckAllBuildingsLoadFinished())
		{
			//UnityEngine.Debug.Log("All buildings have finished loading.");
			//TimeTracking.EndProcess(TrackingProcess.init_building);
            _isBuildingsLoadFinished = true;
		}
	}

	private bool CheckAllBuildingsLoadFinished()
	{
		foreach (var status in buildingLoadStatus.Values)
		{
			if (!status)
			{
				return false; // If any building is not loaded, return early
			}
		}
		return true;
	}
    #endregion Init Building!!!

    #region Setup Cat Character:

	public void CheckStartSetupCatCharacter()
	{
		_isCatCharacterLoadFinished = false;
    }

	public void CheckFinishSetupCatCharacter()
	{
		_isCatCharacterLoadFinished = true;
	}

    #endregion Setup Cat Character!!!
}