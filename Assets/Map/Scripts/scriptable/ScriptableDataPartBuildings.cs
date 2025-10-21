using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EditorPartPosBuilding", menuName = "ScriptableObjects/EditorPartPosBuilding", order = 1)]
public partial class ScriptableDataPartBuildings : ScriptableObject
{
    // NEW: Hierarchical structure - 1 building chứa nhiều parts
    public List<PosDataParent> buildingDataList = new();
    [System.Serializable]
    public class PosData
    {
        [Header("Part Info")]
        [Tooltip("Tên của part")]
        public string name;

        [Space(5)]
        [Tooltip("Danh sách mapping configs cho part này")]
        public List<PosDataDetail> details = new(); 
    }

    [System.Serializable]
    public class PosDataDetail
    {
        [Header("Mapping")]
        [Tooltip("Building ID mà config này sẽ map tới")]
        public string mappingBuildingId;
        
        [Header("Type & Transform")]
        public PartBuildingType type; 
        
        [Space(5)]
        public Vector3 localPosition;
        public Vector3 rotation;
        public Vector3 scale;
        
        // Helper properties
        public float position_x => localPosition.x;
        public float position_y => localPosition.y;
        public float position_z => localPosition.z;
        public float rotation_x => rotation.x;
        public float rotation_y => rotation.y;
        public float rotation_z => rotation.z;
        public float scale_x => scale.x;
        public float scale_y => scale.y;
        public float scale_z => scale.z;
    }

    [System.Serializable]
    public class PosDataParent
    {
        [Header("Building Info")]
        [Tooltip("ID của building này")]
        public string buildingId;
        
        [Space(10)]
        [Tooltip("Danh sách parts thuộc building này")]
        public List<PosData> parts = new();
        
        public int PartsCount => parts?.Count ?? 0;
        public int DetailsCount => parts?.Sum(p => p.details?.Count ?? 0) ?? 0;
    }
}

public partial class ScriptableDataPartBuildings
{
    
    #region Helper Methods
    
    /// <summary>
    /// Thêm hoặc update building với list parts
    /// </summary>
    public void AddOrUpdateBuilding(string buildingId, List<PosData> parts)
    {
        var existing = buildingDataList.FirstOrDefault(b => b.buildingId == buildingId);
        
        if (existing != null)
        {
            // Update existing building
            existing.parts = parts;
        }
        else
        {
            // Add new building
            buildingDataList.Add(new PosDataParent
            {
                buildingId = buildingId,
                parts = parts
            });
        }
    }
    
    /// <summary>
    /// Merge parts vào building (update existing hoặc add new)
    /// </summary>
    public void MergePartsToBuilding(string buildingId, List<PosData> newParts)
    {
        var building = buildingDataList.FirstOrDefault(b => b.buildingId == buildingId);
        
        if (building == null)
        {
            // Tạo building mới
            building = new PosDataParent { buildingId = buildingId, parts = new List<PosData>() };
            buildingDataList.Add(building);
        }
        
        foreach (var newPart in newParts)
        {
            var existingPart = building.parts.FirstOrDefault(p => p.name == newPart.name);
            
            if (existingPart != null)
            {
                // Merge details
                foreach (var newDetail in newPart.details)
                {
                    var existingDetail = existingPart.details.FirstOrDefault(
                        d => d.mappingBuildingId == newDetail.mappingBuildingId && d.type == newDetail.type
                    );
                    
                    if (existingDetail != null)
                    {
                        // Update transform data
                        existingDetail.localPosition = newDetail.localPosition;
                        existingDetail.rotation = newDetail.rotation;
                        existingDetail.scale = newDetail.scale;
                    }
                    else
                    {
                        // Add new detail
                        existingPart.details.Add(newDetail);
                    }
                }
            }
            else
            {
                // Add new part
                building.parts.Add(newPart);
            }
        }
    }
    
    /// <summary>
    /// Bulk update mappingBuildingId cho tất cả parts trong một building
    /// </summary>
    public void UpdateMappingBuildingId(string buildingId, string oldMappingId, string newMappingId)
    {
        var building = buildingDataList.FirstOrDefault(b => b.buildingId == buildingId);
        
        if (building != null)
        {
            foreach (var part in building.parts)
            {
                foreach (var detail in part.details)
                {
                    if (detail.mappingBuildingId == oldMappingId)
                    {
                        detail.mappingBuildingId = newMappingId;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Sync mappingBuildingId với buildingId cho một building cụ thể
    /// </summary>
    public void SyncMappingWithBuildingId(string buildingId)
    {
        var building = buildingDataList.FirstOrDefault(b => b.buildingId == buildingId);
        
        if (building != null)
        {
            foreach (var part in building.parts)
            {
                foreach (var detail in part.details)
                {
                    detail.mappingBuildingId = buildingId;
                }
            }
        }
    }
    
    /// <summary>
    /// Sync mappingBuildingId với buildingId cho tất cả buildings
    /// </summary>
    public void SyncAllMappingWithBuildingId()
    {
        foreach (var building in buildingDataList)
        {
            SyncMappingWithBuildingId(building.buildingId);
        }
    }
    
    /// <summary>
    /// Tổng số buildings
    /// </summary>
    public int GetBuildingCount() => buildingDataList.Count;
    
    /// <summary>
    /// Tổng số parts (tất cả buildings)
    /// </summary>
    public int GetTotalPartsCount() => buildingDataList.Sum(b => b.parts.Count);
    
    /// <summary>
    /// Tổng số details (tất cả parts trong tất cả buildings)
    /// </summary>
    public int GetTotalDetailsCount() => buildingDataList.Sum(b => b.parts.Sum(p => p.details.Count));
    
    #endregion

} 
