﻿using System.Collections;
 using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviourWrapper
{

    public MovingType movingType = MovingType.Linear;
    public List<Vector2> trajectory;
    public int speed;

    [SerializeField] private List<float> trajectoryLengths = new List<float>();
    private Vector2 _currentFragment;
    private int _currentStartPoint;
    private int _currentEndPoint;

    protected override void Start()
    {
        base.Start();

        
        if (movingType == MovingType.Random) StartCoroutine(nameof(RandomMoving));
        else if (movingType == MovingType.Linear)
        {
            if(trajectory.Count < 1) return;
        
            for (int i = 0; i < trajectory.Count - 1; i++)
            {
                trajectoryLengths.Add((trajectory[i] - trajectory[i + 1]).magnitude);
            }

            SwitchFragment();
            transform.position = trajectory[0];
        }
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
        if(movingType == MovingType.Linear) LinearMoving();
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


    private IEnumerator RandomMoving()
    {
        while (true)
        {
            var t = Random.Range(2, 5);
            var x = Random.Range(-1, 1);
            var y = Random.Range(-1, 1);
            
            RB.AddForce(new Vector2(x, y) * 200000 * speed);
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
    
}

public enum MovingType
{
    Linear,
    Random
}
