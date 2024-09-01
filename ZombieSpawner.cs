using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject _zombiePrefab;
    public List<Transform> _spawnPoints;
    public int _spawnInterval = 5;
    public int _maxZombies = 20;
    private int _spawnCount = 1;
    void Start()
    {
        StartCoroutine(SpawnZombie());
    }
    IEnumerator SpawnZombie()
    {
        while (_spawnCount < _maxZombies)
        {
            yield return new WaitForSeconds(_spawnInterval);

            int spawnIndex = Random.Range(0, _spawnPoints.Count);
            Transform spawnPoint = _spawnPoints[spawnIndex];

            Instantiate(_zombiePrefab, spawnPoint.position, spawnPoint.rotation);

            _spawnCount++;
        }
    }
}