using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

public class LeavesVFX : MonoBehaviour
{
    public enum EnvironmentLeaves
    {
        none = -1,
        Green,
        Sakura,
        Yellow,
        Count,
    };

    [System.Serializable]
    public struct ParentLeaves
    {
        public EnvironmentLeaves environmentLeaves;
        public List<ParticleSystem> particles;
    }

    public List<ParentLeaves> parents;

    [Header("Attribute times (seconds)")]
    public Vector2 rangeTimeCooldownFirstTime;
    public Vector2 rangeTimeCooldown;
    public Vector2 rangeTimeFallen;

    [Header("Runtime (tracking in editor)")]
    public EnvironmentLeaves environmentFalling;
    public float timeCooldownLeavesFallen;
    public float timeLeavesFallenLeft;
    public float timeLeavesLifetime;

    private void Start()
    {
        timeCooldownLeavesFallen = Random.Range(rangeTimeCooldownFirstTime.x, rangeTimeCooldownFirstTime.y);
        environmentFalling = EnvironmentLeaves.none;
    }

    private void Update()
    {
        if (timeCooldownLeavesFallen <= 0)
        {
            if (environmentFalling == EnvironmentLeaves.none)
            {
                var enviromentType = (EnvironmentLeaves)Random.Range((int)EnvironmentLeaves.Green, (int)EnvironmentLeaves.Count);
                environmentFalling = enviromentType;
                timeLeavesFallenLeft = Random.Range(rangeTimeFallen.x, rangeTimeFallen.y);
            }
            if (timeLeavesFallenLeft <= 0)
            {
                timeCooldownLeavesFallen = Random.Range(rangeTimeCooldown.x, rangeTimeCooldown.y);
                environmentFalling = EnvironmentLeaves.none;
            }
            else
            {
                timeLeavesFallenLeft -= Time.deltaTime;
                EmitLeaves();
            }
        }
        else
        {
            timeCooldownLeavesFallen -= Time.deltaTime;
        }
    }

    private void EmitLeaves()
    {
        if (timeLeavesLifetime > 0)
        {
            timeLeavesLifetime -= Time.deltaTime;
            return;
        }

        var parent = GetParentLeaves(environmentFalling);
        var particle = parent.particles.First();
        var duration = particle.main.duration + particle.main.startLifetime.constantMax;
        particle.Emit(particle.main.maxParticles);

        timeLeavesLifetime = duration;
    }

    private ParentLeaves GetParentLeaves(EnvironmentLeaves environmentType)
    {
        return parents.Find(parent => parent.environmentLeaves == environmentType);
    }
}
