using System;
using UnityEngine;

public class BalloonController : MonoBehaviourWrapper
{
    public Vector2 anchorPoint;
    public int ropeLength;
    
    private AnchorController _anchor;
    private GumController _gum;
    private float _archimedesPower = 1;
    private float _kickPower = 15;
    private bool _balloonClicked;

    private void Start()
    {
        base.Start();
        _anchor = FindObjectOfType<AnchorController>();
        _gum = FindObjectOfType<GumController>();
        _anchor.positionIt(anchorPoint);
    }

    private void OnGUI()
    {
        var mosuePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            /*var mosuePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 kick = (transform.position - mosuePos).normalized;
            RB.velocity = Vector2.zero;
            RB.AddForce(kick * _kickPower);*/
        }


        if (Input.GetMouseButton(0) && _balloonClicked)
        {
            _gum.gameObject.SetActive(true);
            RB.velocity = Vector2.zero;
            _gum.aiming(mosuePos, pos);
        }
        else if (_balloonClicked) {
            _gum.gameObject.SetActive(false);
            _balloonClicked = false;
            
            Vector2 kick = (transform.position - mosuePos);
            RB.velocity = Vector2.zero;
            RB.AddForce(kick * _kickPower);

        }
    }

    private void OnMouseDown()
    {
        _balloonClicked = true;
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
}
