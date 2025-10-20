#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

public partial class McOnMapController
{
    
    public void SetupPosition_Editor(List<ScriptableCharacterOnMapData.CharacterOnMapData> datas)
    {
        if(datas.Count == 0)
            UnityEngine.Debug.LogError($"Data not implemented for {this.gameObject.name}");
        foreach (var data in datas)
        {
            var p = GetCharacterPosition(data.character_type);
            if (p != null)
            {
                p.pos.SetLocalPositionAndRotation(data.position,Quaternion.Euler(data.rotation));
                p.pos.localScale = data.scale; 
            }
            else
            {
                UnityEngine.Debug.LogError($"Data not implemented for {data.character_type} - {this.gameObject.name}");
            }
        }
    }
}
#endif
