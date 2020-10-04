using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    
    public int SpawnDuration = 1;
    public int SpawnerSize = 1;
    public int EnemiesSpeed = 1;

    public List<EnemyController> Enemies = new List<EnemyController>();

    private int _alreadySpawned = 0;
    
    void Start()
    {
        StartCoroutine(nameof(SpawnCoroutine));
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (_alreadySpawned >= SpawnerSize)
            {
                yield break;
            }
            
            yield return new WaitForSeconds(SpawnDuration);
            
            var newEnemy = Instantiate(Enemies[1], transform);
            newEnemy.gameObject.SetActive(true);
            newEnemy.WasSpawned = true;
            newEnemy.speed = EnemiesSpeed;
            _alreadySpawned++;
        }
    }
}
