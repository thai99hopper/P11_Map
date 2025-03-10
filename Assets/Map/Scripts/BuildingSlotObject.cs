using UnityEngine;

[System.Serializable]
public class BuildingSlotObject
{
    public int level;
    public int part;
    public GameObject obj;

    public void SetActiveObject(int level, int part)
    {
        var isActive = false;
        isActive |= level > this.level;
        isActive |= level == this.level && part >= this.part;

        obj.SetActive(isActive);
    }
}
