﻿using System;
using UnityEngine;

public abstract class MonoBehaviourWrapper : MonoBehaviour
{
    protected Rigidbody2D RB;
    protected BoxCollider2D BC;
    protected CircleCollider2D CC;
    protected Vector2 pos;
    
    protected virtual void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        BC = GetComponent<BoxCollider2D>();
        CC = GetComponent<CircleCollider2D>();
    }

    protected virtual void FixedUpdate()
    {
        pos = transform.position;
    }
}
