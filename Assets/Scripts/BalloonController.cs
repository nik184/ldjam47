﻿using System;
using UnityEngine;

public class BalloonController : MonoBehaviourWrapper
{
    public Vector2 anchorPoint = Vector2.down;
    public int ropeLength = 5;
    public float KickSwing = 5;
    
    private readonly float _archimedesPower = 1;
    private readonly float _kickPower = 15;
    private readonly float MinSpeedToKillEnemy = 0.5f;
    
    private AnchorController _anchor;
    private GumController _gum;
    private bool _balloonClicked;
    private float _balloonClickTime;

    private void Start()
    {
        base.Start();
        _anchor = FindObjectOfType<AnchorController>();
        _gum = FindObjectOfType<GumController>();
        _anchor.positionIt(anchorPoint);
    }

    private void OnGUI()
    {
        var mosuePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var kickVector = Vector2.zero;
        
        if (Input.GetMouseButton(0) && _balloonClicked)
        {
            if ((mosuePos - pos).magnitude > KickSwing)
            {
                kickVector = pos + (mosuePos - pos).normalized * KickSwing;
            }
            else
            {
                kickVector = mosuePos;
            }
            
            _gum.gameObject.SetActive(true);
            _gum.aiming(kickVector, pos);
            
            RB.velocity = Vector2.zero;
        }
        else if (_balloonClicked) 
        {
            _gum.gameObject.SetActive(false);
            _balloonClicked = false;
            
            Vector2 kick = pos - mosuePos;
            RB.velocity = Vector2.zero;
            RB.AddForce(kick * _kickPower);

        }
    }

    private void OnMouseDrag()
    {
        _balloonClicked = true;
        _balloonClickTime = Time.time;
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(_balloonClicked) return;
        
        var impulse = Vector2.up * _archimedesPower;
        Debug.DrawLine(pos, pos + impulse, Color.red);
        if ((pos - anchorPoint).magnitude >= ropeLength)
        {
            var ropeVecDir = (anchorPoint - pos).normalized;

            var angleBetweenRopeAndVel = Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(RB.velocity, anchorPoint - pos));
            var angleBetweenRopeAndArch = Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(impulse, anchorPoint - pos));
            
            var Ep = - ropeVecDir * Mathf.Sqrt(RB.velocity.magnitude) * angleBetweenRopeAndVel;
            var Ec = - ropeVecDir * impulse.magnitude * angleBetweenRopeAndArch;
            
            var ropePulling = Ec + Ep;

            if (Vector2.Angle(ropeVecDir, ropePulling) >= 90)
            {
                ropePulling = -ropePulling;
            }

            if ((pos - anchorPoint).magnitude >= ropeLength + 0.5f) 
                ropePulling += ropePulling.normalized * ((pos - anchorPoint).magnitude - ropeLength + 2) * 2;
            
            
            impulse += ropePulling;
            Debug.DrawLine(pos,  pos + ropePulling, Color.magenta);
            Debug.DrawLine(pos,  pos + impulse, Color.blue);
        }

        RB.AddForce(impulse * Time.deltaTime * 100);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var enemyController = collision.gameObject.GetComponentInParent<EnemyController>();
        
        Debug.Log(collision.collider.GetComponent<DamageableArea>());
        Debug.Log(collision.collider.GetComponent<DamageArea>());
        
        if (
            enemyController != null
            && collision.collider.GetComponent<DamageableArea>()
            && RB.velocity.magnitude >= MinSpeedToKillEnemy
            && Time.time - _balloonClickTime < 0.5
            && !_balloonClicked
        )
        {
            Destroy(enemyController.gameObject);
        } else if (enemyController != null && collision.collider.GetComponent<DamageArea>())
        {
            MakeDamage(enemyController.transform.position);
        }
    }

    private void MakeDamage(Vector2 enemyPos)
    {
        RB.AddForce((pos - enemyPos) * 50);
        Die();
    }

    private void Die()
    {
        MenuController.GetInstance().ReloadScene();
    }
}
