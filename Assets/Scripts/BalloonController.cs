using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BalloonController : MonoBehaviourWrapper
{
    public Vector2 anchorPoint = Vector2.down;
    public int ropeLength = 5;
    public float kickSwing = 5;

    private const float ArchimedesPower = 2;
    private const float KickPower = 25;
    private const float MinSpeedToKillEnemy = 0.5f;

    private ScoreBoard _scoreBoard;
    private AnchorController _anchor;
    private GumController _gum;
    private bool _balloonClicked;
    private float _balloonClickTime;
    public bool IsAlive { get; private set; } = true;


    private void Start()
    {
        base.Start();
        _anchor = FindObjectOfType<AnchorController>();
        _gum = FindObjectOfType<GumController>();
        _scoreBoard = FindObjectOfType<ScoreBoard>();
        _anchor.positionIt(anchorPoint);

        StaticData.CurrentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnGUI()
    {
        
        if(!IsAlive) return;
        
        var mosuePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var kickVector = Vector2.zero;
        
        if (Input.GetMouseButton(0) && _balloonClicked)
        {
            if ((mosuePos - pos).magnitude > kickSwing)
            {
                kickVector = pos + (mosuePos - pos).normalized * kickSwing;
            }
            else
            {
                kickVector = mosuePos;
            }
            
            _gum.gameObject.SetActive(true);
            _gum.aiming(kickVector, pos);
            
            RB.velocity = Vector2.zero;
            
            var explosionSound = Resources.Load<AudioClip>("Sounds/ball_rope");
            AudioObject.GetInstance().Play(explosionSound);
        }
        else if (_balloonClicked) 
        {
            _gum.gameObject.SetActive(false);
            _balloonClicked = false;
            
            Vector2 kick = pos - mosuePos;
            RB.velocity = Vector2.zero;
            RB.AddForce(kick * KickPower);

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
        
        if(_balloonClicked || !IsAlive) return;
        
        var impulse = Vector2.up * ArchimedesPower;
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
            Kill(enemyController);
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
        StartCoroutine(nameof(Loose));
    }

    private void Kill(EnemyController enemy)
    {
        StaticData.IncrementScore();
        _scoreBoard.RedrawScore();
        var explosion = Resources.Load<GameObject>("FX/CFX3_Fire_Explosion");
        Destroy(Instantiate(explosion, enemy.transform.position, Quaternion.identity), 2);
        Destroy(enemy.gameObject);
        
        var explosionSound = Resources.Load<AudioClip>("Sounds/punch" + Mathf.RoundToInt(Random.Range(1,3.4f)));
        AudioObject.GetInstance().Play(explosionSound);


        if (StaticData.KilledEnemies == StaticData.TotalEnemies)
        {
            StartCoroutine(nameof(Win));
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        RopeBoost ropeBoost = collider.GetComponent<RopeBoost>();

        if (ropeBoost != null)
        {
            ropeLength += ropeBoost.boostLength;
            Destroy(ropeBoost.gameObject);
        }
    }

    private IEnumerator Win()
    {
        yield return new WaitForSeconds(2);
        MenuController.GetInstance().WinScene();
    }

    private IEnumerator Loose()
    {
        IsAlive = false;
        
        Destroy(GetComponentInChildren<SpriteRenderer>().gameObject);
        var explosion = Resources.Load<GameObject>("FX/CFX3_Skull_Explosion");
        
        var explosionSound = Resources.Load<AudioClip>("Sounds/damageRecieved");
        AudioObject.GetInstance().Play(explosionSound);
        
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 2);
        yield return new WaitForSeconds(2);
        MenuController.GetInstance().LooseScene();
    }
}
