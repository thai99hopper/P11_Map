using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonData", menuName = "ScriptableEffect/ScriptableButton", order = 1)]
public class ScriptableButtonEffect : ScriptableObject
{
	[System.Serializable]
	public class Data
	{
		public ScriptableSequenceTween tween;
		public ButtonEffectType type; 
	}

	[SerializeField] List<Data> effects = new List<Data>();

	public ScriptableSequenceTween GetSequence(ButtonEffectType type) => effects.Find(v => v.type == type).tween;
}
