using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPointSpawner : MonoBehaviour
{
    public int spawnDuration = 10;

    public HealPoint healPoint;
    private BalloonController _balloonController;

    private float _lastTakenTime;
    
    void Start()
    {
        StartCoroutine(nameof(SpawnCoroutine));
        _balloonController = FindObjectOfType<BalloonController>();
        _lastTakenTime = -1000;
    }

    private IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(3);
        while (true)
        {
            if (
                FindObjectsOfType<HealPoint>().Length < 1
                && (Time.time - _lastTakenTime) > spawnDuration
                && !_balloonController.HasHealPoint
                )
            {
                var newPoint = Instantiate(healPoint, transform);
                newPoint.gameObject.SetActive(true);
                _lastTakenTime = Time.time;
            }
            
            yield return new WaitForSeconds(3);
        }
    }
}
