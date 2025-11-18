using System.Collections.Generic;
using UnityEngine;

public class MyObjectEffect : MonoBehaviour
{
	[System.Serializable]
	public class EffectObject
	{
		public ScriptableSequenceTween seq;
		public List<GameObject> obj;
	}
	[SerializeField] List<EffectObject> effects;
	[SerializeField] bool effectWhenEnableObj; 
	private void Start() => effects.ForEach(val => val.obj.ForEach(obj =>
	{
		if (obj != null)
			val.seq.Run(obj);
	}));

	private void OnEnable() 
	{ 
		if(effectWhenEnableObj)
		{
			effects.ForEach(val => val.obj.ForEach(obj =>
			{
				if (obj != null)
					val.seq.Run(obj);
			}));
		}
	}
}
