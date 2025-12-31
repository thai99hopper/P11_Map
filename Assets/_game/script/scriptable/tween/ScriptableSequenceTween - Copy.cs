using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// *NOTE : please note that if you add new variable into class , it will automatically reset all the data 
/// </summary>
[CreateAssetMenu(fileName = "ScriptableTween", menuName = "Scriptable/SequenceScriptable", order = 1)]
public class ScriptableSequenceTween : ScriptableObject
{
	[SerializeField] string nameTween;
	[SerializeField] SequenceStepConfigs[] sequenceStepConfigs;
	[SerializeField] bool resetAll = false;
	[SerializeField] bool resetPos = false;

	public string Name => nameTween;

	public float TotalDuration
	{
		get
		{
			float result = default;
			foreach (var step in sequenceStepConfigs)
			{
				result += step.delay;
				foreach (var tw in step.GetTweens)
				{
					result += tw.IsLoop ? tw.duration * tw.loop : tw.duration;
					result += tw.IsEnableInterval ? tw.interval : default;
				}
			}
			return result;
		}
	}

	public Sequence Run(GameObject obj, Func<GameObject> whenFinish = null)
	{
		Sequence mySeq = DOTween.Sequence();
		Vector3 old_pos = obj.transform.position;
		foreach (var step in sequenceStepConfigs)
		{
			bool fTween = true;
			foreach (var tween in step.GetTweens)
			{
				if (fTween)
				{
					fTween = false;
					mySeq.Append(tween.DoTween(obj, old_pos));
				}
				else
				{
					mySeq.Join(tween.DoTween(obj, old_pos));
				}
			}
			mySeq.AppendInterval(step.delay);
		}

		if (resetAll)
		{
			mySeq.AppendCallback(() =>
			{
				if (resetPos)
					obj.transform.position = old_pos;

				whenFinish?.Invoke();
				mySeq.Restart();
				mySeq.Kill();
			});
		}
		else
		{
			mySeq.AppendCallback(() =>
			{
				if (resetPos)
					obj.transform.position = old_pos;

				whenFinish?.Invoke();
			});
		}
		return mySeq;
	}


	[System.Serializable]
	public class SequenceStepConfigs
	{
		[SerializeField] ScriptableTween[] tweens;
		[SerializeField] public float delay;

		public ScriptableTween[] GetTweens => tweens;
	}
}
