using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using static yoon.Mob;

public class FinalMob : MonoBehaviour
{

    public float distanceToPlayer; // �÷��̾���� �Ÿ�

    public float moveSpeed = 1f;
    float currentSpeed;

    public TMP_Text hpText;

    //public int thisHP;

    public bool isDead = false;
    public bool isAttackTrue; // ���� ���� üũ
    public bool isDelayTrue = false;

    public bool monsterHit = false;

    public int thisHP;

    public float attackRange; // ���� ����
    public float checkRange; // �þ� ����

    public bool isFrontTrue = false;
    // �齺�ܿ� �Լ�
    public bool isBackPlayer = false;
    public bool isJumpTrue = false;
    public float curveTime = 5.0f;
    public float interval = 0.1f;
    public float jumpPower = 10f;
    public float mass = 5;
    float maxJumpDistance = 10;

    public Animator anim;


    float damping = 3f;
    float rotationSpeedMultiplier = 1f; // �̵� �ӵ��� ���� ȸ�� �ӵ� ���� ����
    private int nextIndex = 0;

    Transform player;

    public Transform wayPointGroup; // �ν����� â���� �Ҵ��� �� �ֵ��� public���� ����
    private Transform[] points;

    public int scorpionDamage = 5;
    public int scorpionHP;
    int nextHP;
    float currentTime = 0;
    private float lerpDuration = 1.5f; // ü���� õõ�� ���� �ð� (1.5��)
    private float delayDuration = 1.5f; // ������
    private int totalDamageTaken = 0; // ���� ������
    int wayPointNum = 0;


    PlayerContorler playerScript;
    TestScripts playerTestScript;
    public bool playerHideTrue;

    NavMeshAgent navMeshAgent;

    [SerializeField]
    private Slider _hpBar;

    [SerializeField]
    private Slider _nextHpBar; // nextHP �� �����̴� �߰�

    [SerializeField]
    Transform target;



    private Camera mainCamera; // ���� ī�޶� ����
    public float hideDistance = 10f; // ü�¹ٸ� ���� �Ÿ�

    private void Start()
    {
        playerHideTrue = true;
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerContorler>();
        mainCamera = Camera.main; // ���� ī�޶� �ʱ�ȭ
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;

        if (wayPointGroup != null)
        {
            points = wayPointGroup.GetComponentsInChildren<Transform>();

            // ù ��° ���(�θ�)�� �ǳʶٰ� �迭�� �ʱ�ȭ
            List<Transform> pointList = new List<Transform>(points);
            pointList.RemoveAt(0);
            points = pointList.ToArray();
        }

    }

    private void Awake()
    {
        scorpionHP = thisHP;
        nextHP = scorpionHP;
        _hpBar.maxValue = scorpionHP;
        _hpBar.value = scorpionHP;
        _nextHpBar.maxValue = scorpionHP; // nextHP �����̴� �ʱ�ȭ
        _nextHpBar.value = scorpionHP;
    }

    private void Update()
    {
        if (!monsterHit && !isDead)
        {
            if (!isAttackTrue)
            {
                RangeCheck();
            }

            if (playerHideTrue)
            {
                WayPointMove();
            }
            else if (!playerHideTrue)
            {
                NavMeshPlayer();
            }

            if (isJumpTrue && !isAttackTrue && isDead != true)
            {
                print("�̰� ������");
                anim.SetTrigger("JumpAttack");
                AttackDelay();

                navMeshAgent.isStopped = true;
            }

            else if (isJumpTrue && isAttackTrue)
            {
                FrontJump();
            }

            if (moveSpeed != 0)
            {
                anim.SetBool("Move", true);
            }
            else if (moveSpeed == 0)
            {
                anim.SetBool("Move", false);
            }

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitFront, 6f, (1 << 10)))
            {
                // Y�� ȸ���� �����ϸ鼭 X�� ȸ���� 0���� ����
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
        else
        {

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


    void FrontJump()
    {
        if (!isJumpTrue)
        {
            navMeshAgent.isStopped = true;
            //print("BackJump");

            Vector3 dir = transform.forward + transform.up;
            dir.Normalize();
            Rigidbody rb = GetComponent<Rigidbody>();

            rb.AddForce(dir * jumpPower, ForceMode.Impulse);

            StartCoroutine(CheckLanding()); // ���� üũ�� �ڷ�ƾ���� ����
        }
    }

    IEnumerator CheckLanding()
    {
        isJumpTrue = true; // ���� ���·� ����
        yield return new WaitForSeconds(0.1f); // ���� �� 0.1�� ���
        while (isJumpTrue)
        {
            // �Ʒ� �������� Raycast�� ��� �ٴ� �浹 Ȯ��
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
            {
                if (hit.distance < 0.1f) // �ٴڿ� ���� ����� ���
                {
                    // �ٴڿ� ����� ��� ���� ó��
                    transform.position = hit.point; // �ٴڿ� ��Ȯ�� ����
                    isBackPlayer = false; // ������ ����
                    isJumpTrue = false; // ���� ���� ����
                    moveSpeed = currentSpeed; // �̵� �ӵ� ����
                    //print("���� isAttackTrue " + isAttackTrue);
                    navMeshAgent.isStopped = false; // �׺���̼� ��Ȱ��ȭ
                }
            }
            yield return new WaitForSeconds(0.1f); // ���� ���θ� �ݺ������� Ȯ��
        }
    }

    public void GetDamage(int damage)
    {
        if (scorpionHP > 0 && !isDead)
        {
            scorpionHP -= damage;
            _hpBar.value = scorpionHP;
            nextHP = scorpionHP;
            currentTime = 0.0f; // �� �������� ���� ������ currentTime�� �ʱ�ȭ
            totalDamageTaken += damage; // ���� ������ ������Ʈ
            UpdateDamageText(); // ������ �ؽ�Ʈ ������Ʈ

            if (scorpionHP <= 0)
            {
                isDead = true;
                Die();
            }
            else
            {
                anim.SetTrigger("Hit");
                AttackDelay();
            }
        }
    }




    private void UpdateDamageText()
    {
        hpText.text = totalDamageTaken.ToString();
    }


    void NavMeshPlayer()
    {
        
        //print("NavMeshPlayer");
        navMeshAgent.isStopped = false;
        moveSpeed = currentSpeed;
        
        if (target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }

    void RangeCheck()
    {
        if (isJumpTrue)
            return;
        //print("RangeCheck");
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Ray ���� ��ġ�� ���� transform.position���� y���� 0.5�� ����
        Vector3 rayOrigin = transform.position;
        rayOrigin.y += 0.5f; // y���� 0.5�� ����

        Ray ray = new Ray(rayOrigin, transform.forward);
        Ray backRay = new Ray(rayOrigin, -transform.forward);


        distanceToPlayer = Vector3.Distance(target.position, transform.position);



        // �÷��̾ �߰������� ���� ������ ���� ���� ���
        if (checkRange >= distanceToPlayer && attackRange < distanceToPlayer && !isBackPlayer)
        {
            playerHideTrue = false;
            isJumpTrue = false;
            transform.LookAt(target.transform.position);
            navMeshAgent.isStopped = false;
        }
        // ���� ������ ���� ���
        else if (attackRange >= distanceToPlayer && !isBackPlayer)
        {
            isJumpTrue = true;
            // �߰� ������ �ʿ��� ��� ���⿡ �ۼ�
        }
        // �÷��̾ �߰� ������ �Ѿ ���
        else if (checkRange < distanceToPlayer)
        {
            playerHideTrue = true;
            isJumpTrue = false;
        }
        
    }


    public void Attack()
    {
        if ((!isDead))
        {
            print("Attack");
            isAttackTrue = true;
        }
    }

    public void AttackAfter()
    {
        if ((!isDead))
        {
            print("AttackAfter");
            isAttackTrue = false;
            isJumpTrue = true;
        }
    }

    void AttackDelay()
    {
        currentTime += Time.deltaTime;
        moveSpeed = 0;
        transform.LookAt(playerScript.transform.position);
        if (currentTime > 4f)
        {
            currentTime = 0;
            anim.SetTrigger("delayFinish");
            navMeshAgent.isStopped = false;
            isDelayTrue = false;
            moveSpeed = currentSpeed;
        }
    }

    void HitAfter()
    {
        monsterHit = false;
        if (isAttackTrue == true)
        {
            isAttackTrue = false;
        }
        isDelayTrue = false;
        NavMeshPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Melee") && playerScript.hit && !isDead)
        {
            Sward sward = other.GetComponent<Sward>();
            print("hiyMonster");
            playerScript.hit = false;
            monsterHit = true;
            GetDamage(sward.attackPower);
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            //PlayerContorler player = other.GetComponent<PlayerContorler>();
            print("�÷��̾� �ǰ�");

            //player.GetDamage(scorpionDamage);
            //isAttackTrue = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttackTrue && !isDead)
        {
            PlayerContorler player = collision.gameObject.GetComponent<PlayerContorler>();
            print("�÷��̾� �ǰ�");

            playerScript.playerHit = true;
            player.GetDamage(scorpionDamage);
            isAttackTrue = false;
        }
    }



    #region ��������Ʈ �Լ�
    void WayPointMove()
    {
        moveSpeed = currentSpeed;
        
        navMeshAgent.isStopped = true; // NavMeshAgent ��Ȱ��ȭ
        if (points.Length == 0) return;

        Vector3 direction = points[nextIndex].position - transform.position;
        Quaternion rot = Quaternion.LookRotation(direction);
        float adjustedDamping = damping * (moveSpeed * rotationSpeedMultiplier); // �̵� �ӵ��� ���� ȸ�� �ӵ� ����
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * adjustedDamping);

        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // ���� ��������Ʈ�� �̵��� ���� Ȯ��
        if (Vector3.Distance(transform.position, points[nextIndex].position) < 0.1f)
        {
            nextIndex = (++nextIndex >= points.Length) ? 0 : nextIndex;
            wayPointNum++;
            if (wayPointNum % 3 == 0)
            {
                wayPointNum = 0;
            }
        }
        if (!playerHideTrue)
        {

        }
    }
    #endregion

    // ���� ����� ��������Ʈ�� ���� ��������Ʈ�� ����
    private int GetClosestWayPointIndex()
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, points[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    #region HPBar �Լ�
    private void LateUpdate()
    {
        if (_hpBar != null)
        {
            _hpBar.transform.LookAt(_hpBar.transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
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
    #endregion


    private void OnDrawGizmos()
    {
        // target�� null�� �ƴ� ��쿡�� �׸����� �մϴ�.
        if (target != null)
        {
            // Ray ���� ��ġ�� ���� transform.position���� y���� 0.5�� ����
            Vector3 rayOrigin = transform.position;
            rayOrigin.y += 0.5f; // y���� 0.5�� ����

            // Ray ����
            Ray ray = new Ray(rayOrigin, transform.forward);

            Ray backRay = new Ray(rayOrigin, -transform.forward);

            // Ray�� � ������ �׷����� �����մϴ�.
            Gizmos.color = Color.red;

            // Ray�� �׸��ϴ�.
            Gizmos.DrawRay(ray.origin, ray.direction * attackRange);

            Gizmos.DrawRay(backRay.origin, backRay.direction * attackRange);

            // Ray�� ���� �������� ��ü�� �׷��ݴϴ�.
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, attackRange, (1 << 11)))
            {
                Gizmos.color = Color.green; // ���� ��� �ʷϻ����� ǥ��
                Gizmos.DrawSphere(hitInfo.point, 0.2f); // ���� ������ ���� ��ü�� �׸�
            }
        }
    }

}
