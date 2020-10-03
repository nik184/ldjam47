using UnityEngine;

public class BalloonController : MonoBehaviourWrapper
{
    public Vector2 anchorPoint;
    public int ropeLength;
    
    private AnchorController _anchor;
    private float _archimedesPower = 1;
    private float _kickPower = 100;
    private Vector2 pos;

    private void Start()
    {
        base.Start();
        _anchor = FindObjectOfType<AnchorController>();
        _anchor.positionIt(anchorPoint);
    }

    private void OnGUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mosuePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 kick = (transform.position - mosuePos).normalized;
            RB.velocity = Vector2.zero;
            RB.AddForce(kick * _kickPower);
        }
    }

    private void FixedUpdate()
    {
        pos = transform.position;
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
