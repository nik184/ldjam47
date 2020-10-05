using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private void Start()
    {
        Destroy(gameObject,1);
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    private void Update()
    {
        _spriteRenderer.color = new Color(1,1,1, _spriteRenderer.color.a - Time.deltaTime);
        transform.position += Vector3.up * Time.deltaTime;
    }
}
