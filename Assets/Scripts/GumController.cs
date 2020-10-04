﻿using UnityEngine;

public class GumController : MonoBehaviourWrapper
{
    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;

    protected override void Start()
    {
        base.Start();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void aiming(Vector2 from, Vector2 to)
    {
        Vector3[] points = new Vector3[2];
        _lineRenderer.positionCount = points.Length;

        points[0] = from;
        points[1] = to;

        _lineRenderer.SetPositions(points);
        _spriteRenderer.gameObject.transform.position = from;
    }
}
