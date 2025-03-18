using System.Collections.Generic;
using UnityEngine;

public class RainyVFX : MonoBehaviour
{
    [SerializeField] Transform parent;

    [Header("Attribute times (seconds)")]
    public Vector2 rangeTimeCooldownFirstTime;
    public Vector2 rangeTimeCooldown;
    public Vector2 rangeTimeRainy;

    [Header("Runtime (tracking in editor)")]
    public float timeCooldownRainy;
    public float timeRainyLeft;

    public List<Transform> rainyParticles = new List<Transform>();

    public bool isRainy = false;

    private void Start()
    {
        timeCooldownRainy = Random.Range(rangeTimeCooldownFirstTime.x, rangeTimeCooldownFirstTime.y);
        foreach (Transform child in parent)
        {
            rainyParticles.Add(child);
        }
        ShowRainy(-1);
    }

    private void Update()
    {
        if (timeCooldownRainy <= 0)
        {
            if (!isRainy)
            {
                timeRainyLeft = Random.Range(rangeTimeRainy.x, rangeTimeRainy.y);
            }

            if (timeRainyLeft <= 0)
            {
                timeCooldownRainy = Random.Range(rangeTimeCooldown.x, rangeTimeCooldown.y);
                ShowRainy(-1);
            }
            else
            {
                timeRainyLeft -= Time.deltaTime;
                Rainy();
            }
        }
        else
        {
            timeCooldownRainy -= Time.deltaTime;
        }
    }

    private void Rainy()
    {
        if (isRainy)
        {
            return;
        }

        int count = rainyParticles.Count;
        var index = Random.Range(0, rainyParticles.Count);
        ShowRainy(index);

    }

    /// <summary>
    /// input -1 to hide all
    /// </summary>
    /// <param name="index"></param>
    private void ShowRainy(int index)
    {
        int count = rainyParticles.Count;
        for (int i = 0; i < count; i++)
        {
            rainyParticles[i].gameObject.SetActive(i == index);
        }
        isRainy = index != -1;
    }
}
