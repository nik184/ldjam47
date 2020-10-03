using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviourWrapper
{

    public MovingType movingType = MovingType.Linear;
    public Vector2 direction = Vector2.right;

    protected override void Start()
    {
        base.Start();
        RB.AddForce(direction * 2000000);
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
        //transform.position = pos + direction * Time.deltaTime;
    }
}

public enum MovingType
{
    Linear,
    Random
}
