using System.Collections.Generic;
using UnityEngine;

public partial class McOnMapController
{
    [SerializeField] private List<CharacterPosition> characterPositions = new(); 
    
    public CharacterPosition GetCharacterPosition(CharacterOnMapType type)
    {
        foreach (var p in characterPositions)
        {
            if (p.type == type)
                return p;
        }
        return null; 
    }
    
    [System.Serializable]
    public class CharacterPosition
    {
        public Transform pos;
        public CharacterOnMapType type; 
    }
}
