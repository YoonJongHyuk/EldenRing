using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FinalMob : MonoBehaviour
{
    public enum MobState
    {
        WaypointMove,
        ChasePlayer,
        Attack,
        Dead
    }

    public MobState mobState = MobState.WaypointMove;

    public float distanceToPlayer;
    public float moveSpeed = 1f;
    private float currentSpeed;

    private int nextIndex = 0;

    public TMP_Text hpText;
    public bool isDead = false;
    public bool isAttackTrue = false;
    public bool isDelayTrue = false;

    public int thisHP;
    public float attackRange = 2.0f;
    public float stopDistance = 1.5f; // 멈추는 최소 거리
    public float checkRange = 10.0f;

    public Animator anim;
    private Transform player;

    public Transform wayPointGroup;
    private Transform[] points;

    public int scorpionDamage = 5;
    public int scorpionHP;
    private int nextHP;
    private float currentTime = 0f;
    private float lerpDuration = 1.5f;
    private float delayDuration = 1.5f;
    private int totalDamageTaken = 0;

    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private Slider _hpBar;
    [SerializeField]
    private Slider _nextHpBar;

    [SerializeField]
    private Transform target;

    private Camera mainCamera;
    public float hideDistance = 10f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;

        if (wayPointGroup != null)
        {
            points = wayPointGroup.GetComponentsInChildren<Transform>();
            List<Transform> pointList = new List<Transform>(points);
            pointList.RemoveAt(0);
            points = pointList.ToArray();
        }

        scorpionHP = thisHP;
        nextHP = scorpionHP;
        _hpBar.maxValue = scorpionHP;
        _hpBar.value = scorpionHP;
        _nextHpBar.maxValue = scorpionHP;
        _nextHpBar.value = scorpionHP;

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isDead)
        {
            mobState = MobState.Dead;
            Die();
        }

        switch (mobState)
        {
            case MobState.WaypointMove:
                WayPointMove();
                if (Vector3.Distance(transform.position, player.position) < checkRange)
                {
                    mobState = MobState.ChasePlayer;
                }
                break;

            case MobState.ChasePlayer:
                moveSpeed = 0;
                NavMeshPlayer();
                if (Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    navMeshAgent.ResetPath();
                    mobState = MobState.Attack;
                }
                break;

            case MobState.Attack:
                if (!isDelayTrue)
                {
                    anim.SetTrigger("Attack");
                    AttackDelay();
                    
                }
                break;

            case MobState.Dead:
                break;
        }

        if (_nextHpBar.value != nextHP)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= delayDuration)
            {
                float t = (currentTime - delayDuration) / lerpDuration;
                _nextHpBar.value = Mathf.Lerp(_nextHpBar.value, nextHP, t);
                if (Mathf.Abs(_nextHpBar.value - nextHP) < 0.01f)
                {
                    _nextHpBar.value = nextHP;
                    currentTime = 0.0f;
                }
            }
        }

        UpdateHPBarVisibility();
    }

    void Die()
    {
        anim.SetTrigger("Death");
        navMeshAgent.isStopped = true;
        moveSpeed = 0;
        StartCoroutine(DestroyMonster());
    }

    IEnumerator DestroyMonster()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    void NavMeshPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > stopDistance)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            // 플레이어와의 거리가 너무 가까우면 멈추도록 함
            navMeshAgent.isStopped = true;
        }
    }

    void AttackDelay()
    {
        isDelayTrue = true;
        navMeshAgent.isStopped = true;
        StartCoroutine(AttackDelayCoroutine());
    }

    IEnumerator AttackDelayCoroutine()
    {
        yield return new WaitForSeconds(4f);
        isDelayTrue = false;
        mobState = MobState.ChasePlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isDead)
        {
            player.GetComponent<PlayerContorler>().GetDamage(scorpionDamage);
        }
    }

    void WayPointMove()
    {
        navMeshAgent.isStopped = true; // NavMeshAgent 비활성화
        moveSpeed = currentSpeed;

        if (points.Length == 0) return;

        Vector3 lookDirection = new Vector3(points[nextIndex].position.x, transform.position.y, points[nextIndex].position.z);
        transform.LookAt(lookDirection);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, points[nextIndex].position) < 1f)
        {
            nextIndex = (nextIndex + 1) % points.Length;
        }
    }

    private void UpdateHPBarVisibility()
    {
        if (Vector3.Distance(target.position, transform.position) > hideDistance)
        {
            _hpBar.gameObject.SetActive(false);
        }
        else
        {
            _hpBar.gameObject.SetActive(true);
        }
    }
}
