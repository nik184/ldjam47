﻿using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Screen;
 using Random = UnityEngine.Random;

 public class EnemyController : MonoBehaviourWrapper
{

    public MovingType movingType = MovingType.Linear;
    public int speed;
    public List<EnemyMovingPoint> points = new List<EnemyMovingPoint>();

    private List<Vector2> trajectory = new List<Vector2>();
    [SerializeField] private List<float> trajectoryLengths = new List<float>();
    private Vector2 _currentFragment;
    private int _currentStartPoint;
    private int _currentEndPoint;

    private bool _wasSpawned = false;
    private Vector2 _anchor;
    private Vector2 _aimCoord;
    private AimType _aim;
    private SpawnerController[] _spawners;
    public bool WasSpawned
    {
        set => _wasSpawned = value;
    }

    protected override void Start()
    {
        base.Start();

        var _anchor = FindObjectOfType<AnchorController>().transform.position;
        
        if (_wasSpawned)
        {
            _spawners = FindObjectsOfType<SpawnerController>();
            _aimCoord = _anchor;
            _aim = AimType.Player;
            Debug.Log(_aimCoord);
        } else switch (movingType)
        {
            case MovingType.Random:
                StartCoroutine(nameof(RandomMovingCoroutine));
                break;
            case MovingType.Linear when points.Count < 1:
                return;
            case MovingType.Linear:
            {
                for (int i = 0; i < points.Count; i++)
                {
                    trajectory.Add(points[i].transform.position);
                }
        
                for (int i = 0; i < trajectory.Count - 1; i++)
                {
                    trajectoryLengths.Add((trajectory[i] - trajectory[i + 1]).magnitude);
                }

                SwitchFragment();
                transform.position = trajectory[0];
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
        if(_wasSpawned) SpawnedMoving();
        else if(movingType == MovingType.Linear) LinearMoving();
        else if(movingType == MovingType.Random) RandomMoving();
    }
    
    private void SpawnedMoving()
    {
        transform.position += ((Vector3)_aimCoord - transform.position).normalized * (speed * Time.deltaTime);
        
        if (!(((Vector3) _aimCoord - transform.position).magnitude < 0.5f)) return;
        switch (_aim)
        {
            case AimType.Player:
                _aimCoord = _spawners[
                    Mathf.RoundToInt(Random.Range(0, _spawners.Length - 1))
                ].transform.position;
                _aim = AimType.Spawner;
                break;
            case AimType.Spawner:
                _aimCoord = _anchor;
                _aim = AimType.Player;
                break;
        }
    }
    
    private void LinearMoving()
    {
        if(trajectory.Count < 1) return;
        transform.position += (Vector3)_currentFragment.normalized * (speed * Time.deltaTime);
        
        if (_currentFragment.magnitude <= (pos - trajectory[_currentStartPoint]).magnitude)
        {
            SwitchFragment();
        }
    }
    private void RandomMoving()
    {
        if (pos.x < -width/2 || pos.x > width/2 || pos.y < -height/2 || pos.y > height/2)
        {
            SwitchDirection();
        }
    }


    private IEnumerator RandomMovingCoroutine()
    {
        while (true)
        {
            SwitchDirection();
            var t = Random.Range(1, 3);
            yield return new WaitForSeconds(t);
        }
    }

    private void SwitchFragment()
    {
        if (_currentStartPoint == _currentEndPoint)
        {
            _currentStartPoint = 0;
            _currentEndPoint = 1;
        }
        else if (_currentStartPoint < _currentEndPoint)
        {
            if (_currentEndPoint < trajectory.Count - 1)
            {
                _currentEndPoint++;
                _currentStartPoint++;
            }
            else
            {
                _currentEndPoint--;
                _currentStartPoint++;
            }
        }
        else if (_currentStartPoint > _currentEndPoint)
        {
            if (_currentEndPoint > 0)
            {
                _currentEndPoint--;
                _currentStartPoint--;
            }
            else
            {
                _currentEndPoint++;
                _currentStartPoint--;
            }
        }
        
        _currentFragment = trajectory[_currentEndPoint] - trajectory[_currentStartPoint];
    }

    private void SwitchDirection()
    {
        var x = Random.Range(-1, 1);
        var y = Random.Range(-1, 1);
        RB.AddForce(new Vector2(x, y) * 200000 * speed);
    }
}

 public enum MovingType
 {
     Linear,
     Random
 }
 
 public enum AimType
 {
     Spawner,
     Player
 }
