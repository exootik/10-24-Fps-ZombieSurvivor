using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[Serializable]
public struct AreaSpawn
{
    public Vector3 spawnPoint;
    public float size;
}

[Serializable]
public struct Wave
{
    public int numberOfEnnemis;
    public float when;

    [Tooltip("-3: Everywhere\n-2: In any single random SpawnArea\n-1: In any random SpawnArea")]
    public int SpawnArea;
}

[RequireComponent(typeof(NavMeshSurface))]
public class Spawner : MonoBehaviour
{
    [Tooltip("Game object to spawn")] public GameObject prefab;

    [Tooltip("Game object the prefab will follow")]
    public GameObject Target;

    public AreaSpawn[] SpawnArea;
    public List<Wave> Waves;
    private float _currentTime;
    private float _nextWaveTime;
    private NavMeshSurface _surface;
    private Vector3 _surfaceCenter;
    private float _surfaceSize;

    private void Start()
    {
        _surface = GetComponent<NavMeshSurface>();
        Follow component;
        prefab.TryGetComponent(out component);
        if (component != null) component.target = Target.transform;

        var vect = _surface.navMeshData.sourceBounds.extents;
        _surfaceSize = math.max(math.max(vect.x, vect.y), vect.z);
        _surfaceCenter = _surface.navMeshData.position + _surface.navMeshData.sourceBounds.center;

        Waves = Waves.OrderBy(wave => wave.when).ToList();
        _nextWaveTime = Waves[0].when;
    }

    // Update is called once per frame
    private void Update()
    {
        _currentTime += Time.smoothDeltaTime;
        if (Waves.Count > 0 && _currentTime > _nextWaveTime)
        {
            switch (Waves[0].SpawnArea)
            {
                case -3:
                {
                    SummonMultiple(Waves[0].numberOfEnnemis);
                    break;
                }
                case -2:
                {
                    SummonMultiple(SpawnArea[Random.Range(0, SpawnArea.Length)], Waves[0].numberOfEnnemis);
                    break;
                }
                case -1:
                {
                    SummonMultipleRandomArea(Waves[0].numberOfEnnemis);
                    break;
                }
                default:
                {
                    if (Waves[0].SpawnArea >= SpawnArea.Length)
                        Debug.LogError("Incorrect SpawnArea number");
                    else
                        SummonMultiple(SpawnArea[Waves[0].SpawnArea], Waves[0].numberOfEnnemis);

                    break;
                }
            }

            Waves.RemoveAt(0);
            Waves.TrimExcess();
            if (Waves.Count > 0)
                _nextWaveTime = Waves[0].when;
        }
    }

    private void Summon()
    {
        Summon(_surfaceCenter, _surfaceSize);
    }

    private void SummonRandomArea()
    {
        Summon(SpawnArea[Random.Range(0, SpawnArea.Length)]);
    }

    private void Summon(AreaSpawn area)
    {
        Summon(area.spawnPoint, area.size);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Summon(Vector3 where, float size)
    {
        var summoned = false;
        while (!summoned)
        {
            var randomDirection = Random.insideUnitSphere * size + where;
            if (NavMesh.SamplePosition(randomDirection, out var hit, size, _surface.layerMask))
                summoned = Instantiate(prefab, hit.position, Quaternion.identity);
            else
                Debug.Log("Failed to get a point in NavMesh");
        }
    }

    private void SummonMultiple(int number)
    {
        for (var i = 0; i < number; i++) Summon();
    }

    private void SummonMultiple(AreaSpawn area, int number)
    {
        for (var i = 0; i < number; i++) Summon(area);
    }

    private void SummonMultipleRandomArea(int number)
    {
        for (var i = 0; i < number; i++) SummonRandomArea();
    }
}