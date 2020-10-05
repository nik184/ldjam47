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
        yield return new WaitForSeconds(1);
        while (true)
        {
            if (_alreadySpawned >= SpawnerSize) yield break;

            if (FindObjectsOfType<EnemyController>().Length < 3)
            {
                var r = Mathf.RoundToInt(Random.Range(0, 1.2f));
                var newEnemy = Instantiate(Enemies[r], transform);
                newEnemy.gameObject.SetActive(true);
                newEnemy.WasSpawned = true;
                newEnemy.speed = EnemiesSpeed;
                _alreadySpawned++;
            } 
            
            yield return new WaitForSeconds(SpawnDuration);
        }
    }
}
