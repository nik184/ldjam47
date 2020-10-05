using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BalloonController : MonoBehaviourWrapper
{
    public int ropeLength = 5;
    public float kickSwing = 5;

    private const float ArchimedesPower = 2;
    private const float KickPower = 25;
    private const float MinSpeedToKillEnemy = 0.5f;
    private readonly Vector2 _anchorPoint = new Vector2(0, -2);

    private SpriteRenderer _spriteRenderer;
    private ScoreBoard _scoreBoard;
    private AnchorController _anchor;
    private GumController _gum;
    private PanelPause _pausePanel;
    private bool _balloonClicked;
    private float _balloonClickTime;
    public bool IsAlive { get; private set; } = true;

    private bool _hasHealPoint = false;
    public bool HasHealPoint => _hasHealPoint;
    private float _lastKillTime;
    


    private void Start()
    {
        base.Start();
        _anchor = FindObjectOfType<AnchorController>();
        _gum = FindObjectOfType<GumController>();
        _scoreBoard = FindObjectOfType<ScoreBoard>();
        _anchor.positionIt(_anchorPoint);
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        StaticData.CurrentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnGUI()
    {
        
        if(!IsAlive || MenuController.Paused) return;
        
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

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            MenuController.GetInstance().Pause();
        }
    }

    private void OnMouseDrag()
    {
        if(!IsAlive) return;
        _balloonClicked = true;
        _balloonClickTime = Time.time;
    }

    private void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(_balloonClicked || !IsAlive) return;
        
        var impulse = Vector2.up * ArchimedesPower;
        if ((pos - _anchorPoint).magnitude >= ropeLength)
        {
            var ropeVecDir = (_anchorPoint - pos).normalized;

            var angleBetweenRopeAndVel = Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(RB.velocity, _anchorPoint - pos));
            var angleBetweenRopeAndArch = Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(impulse, _anchorPoint - pos));
            
            var Ep = - ropeVecDir * Mathf.Sqrt(RB.velocity.magnitude) * angleBetweenRopeAndVel;
            var Ec = - ropeVecDir * impulse.magnitude * angleBetweenRopeAndArch;
            
            var ropePulling = Ec + Ep;

            if (Vector2.Angle(ropeVecDir, ropePulling) >= 90)
            {
                ropePulling = -ropePulling;
            }

            if ((pos - _anchorPoint).magnitude >= ropeLength + 0.5f) 
                ropePulling += ropePulling.normalized * ((pos - _anchorPoint).magnitude - ropeLength + 2) * 2;
            
            
            impulse += ropePulling;
        }

        RB.AddForce(impulse * Time.deltaTime * 100);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!IsAlive) return;
        
        var enemyController = collision.gameObject.GetComponentInParent<EnemyController>();
        
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
        if (_hasHealPoint)
        {
            SwitchHealpoint();
            var explosionSound = Resources.Load<AudioClip>("Sounds/damageRecieved");
            AudioObject.GetInstance().Play(explosionSound);
        }
        else StartCoroutine(nameof(Loose));
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

        if (Time.time - _lastKillTime < 0.5f)
        {
            Instantiate(
                Resources.Load("Images/Combo"), transform.up * 10, quaternion.identity
            );
        }

        _lastKillTime = Time.time;

        if (StaticData.KilledEnemies == StaticData.TotalEnemies)
        {
            StartCoroutine(nameof(Win));
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(!IsAlive) return;
        
        RopeBoost ropeBoost = collider.GetComponent<RopeBoost>();

        if (ropeBoost != null)
        {
            ropeLength += ropeBoost.boostLength;
            Destroy(ropeBoost.gameObject);
        }
        
        HealPoint healPoint = collider.GetComponent<HealPoint>();

        if (healPoint != null && !_hasHealPoint)
        {
            SwitchHealpoint();
            Destroy(healPoint.gameObject);
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
        Destroy(RB);
        Destroy(BC);
        var explosion = Resources.Load<GameObject>("FX/CFX3_Skull_Explosion");
        
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), 2);
        yield return new WaitForSeconds(2);
        MenuController.GetInstance().LooseScene();
    }

    private void SwitchHealpoint()
    {
        _hasHealPoint = !_hasHealPoint;
        Debug.Log(_hasHealPoint);
        _spriteRenderer.color = _hasHealPoint ? Color.magenta : Color.white;
    }
}
