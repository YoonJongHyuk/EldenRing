using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yoon;
using UnityEngine.UI;

public class PlayerContorler : MonoBehaviour
{
    // �̵� ���� ������
    public float MoveSpeed; // �̵� �ӵ�
    float h, v; // ���� �� ���� �Է°�
    bool isMove;
    bool Run; // �޸��� ����
    Vector3 moveVec; // �̵� ����
    public Transform cameraTransform; // ī�޶� Transform

    // ���� ���� ������
    bool isJump = false; // ���� ����
    Vector3 gravityPower; // �߷� ��

    // ������ ���� ������
    bool isDodge; // ������ ����
    Vector3 dodgeVec; // ������ ����

    // �齺�� ���� ����
    bool Backstep = false;

    // ���� ���� ������
    //public GameObject nearObject; // ��ó ������Ʈ
    public GameObject[] Weapon; // ���� �迭
    public Sward equipWeapon; // ������ ����
    public bool[] hasWeapone; // ���� ���� ����
    public bool hiding; // �÷��̾��� ���� ���� ����
    bool iDown; // ��ȣ�ۿ� Ű �Է� ����
    bool Swap1, Swap2, Swap3; // ���� ��ü Ű �Է� ����
    bool isSwap; // ���� ��ü ������ ����

    // ȸ�� ������ ���� ������
    public GameObject potionprefab; // ���� ������
    public float healAmount = 50f; // ȸ����
    public float maxHP = 100; // �ִ� ü��
    public float currentHP = 10; // ���� ü�� (�׽�Ʈ��)
    float nextHP;
    float currentTime = 0;
    private float lerpDuration = 1.5f; // ü���� õõ�� ���� �ð� (1.5��)
    private float delayDuration = 1.5f; // ������
    private int totalDamageTaken = 0; // ���� ������

    // ���� ���� ������
    bool MeleeAttack; // ���� ���� ����
    float AttackDelay; // ���� ���� �ð�
    private List<Scorpion> scorps; // �浹�� Monster ��ü
    private List<Scorpion> hitMonster;
    bool isAttack;

    // ���� ���� ������
    public GameObject ShieldPrefab;
    public bool isShieldActive = false; //���� ����
    public bool isShieldHit = false; // �´� ���л���

    // ȭ�� ���� ������
    public Transform ArrowPos; // ȭ�� ��ġ
    public GameObject Arrow2; // ȭ�� ������Ʈ

    // ��Ÿ ��� ���� ������
    Animator animator; // �ִϸ�����
    Rigidbody rb; // ������ٵ�

    // ���¹̳� ���� ������
    public float maxStamina = 10.0f; // �ִ� ���¹̳�
    public float currentStamina = 10.0f; // ���� ���¹̳�
    public float staminaRecoveryRate = 4.0f; // ���¹̳� ȸ�� �ӵ�
    public float staminaDrainRateAttack = 2.0f; // ���� �� ���¹̳� �Ҹ�
    public float staminaDrainRateDodge = 3.0f; // ������ �� ���¹̳� �Ҹ�

    [SerializeField]
    private Slider _hpBar;

    [SerializeField]
    private Slider _nextHpBar; // nextHP �� �����̴� �߰�

    Scorpion Scorpion;

    // Start�� ù ������ ������Ʈ ���� ȣ��˴ϴ�.
    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // ������ٵ� ������Ʈ ��������
        animator = GetComponentInChildren<Animator>(); // �ڽ� ��ü���� �ִϸ����� ������Ʈ ��������
        hiding = true;
        scorps = new List<Scorpion>(); // Scorpion ����Ʈ �ʱ�ȭ
        hitMonster = new List<Scorpion>(); // hitMonster ����Ʈ �ʱ�ȭ
    }

    void Start()
    {
        // currentHP = maxHP; // ȸ���� ���̱� ���� �ʱ�ȭ ����
        currentStamina = maxStamina; // ���� ���¹̳��� �ִ밪���� ����
        equipWeapon = Weapon[0].GetComponent<Sward>();
        Cursor.lockState = CursorLockMode.Locked;
        currentHP = maxHP;
        nextHP = currentHP;
        _hpBar.maxValue = currentHP;
        _hpBar.value = currentHP;
        _nextHpBar.maxValue = currentHP; // nextHP �����̴� �ʱ�ȭ
        _nextHpBar.value = currentHP;
        isShieldActive =ShieldPrefab.GetComponent<BoxCollider>(); // ���� �ڽ��ݶ��̴� �������
    }

    // Update�� �� ������ ȣ��˴ϴ�.
    void Update()
    {
        Move(); // �̵� ó��
        Rotate(); // ȸ�� ó��
        Jump(); // ���� ó��
        //Interation(); // ��ȣ�ۿ� ó��
        Attack(); // ���� ó��
        Swap(); // ���� ��ü ó��
        Swapout(); // ���� ��ü ���� ó��
        potion(); // ���� ��� ó��
        HPBar();
        Shield();
        BackStep();
        Dodge();
    }

    void HPBar()
    {
        if (_nextHpBar == null) return; // nextHP �����̴��� �Ҵ���� �ʾ����� ����

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
    }

    // �⺻ ������ ó��
    void Move()
    {
        h = Input.GetAxisRaw("Horizontal"); // ���� �Է� �� ��������
        v = Input.GetAxisRaw("Vertical"); // ���� �Է� �� ��������
        Run = Input.GetButton("shift"); // �޸��� �Է� �� ��������

        // ī�޶��� forward�� right ���� ��������
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // y�� �� 0���� �����Ͽ� ���� �̵��� ���
        cameraForward.y = 0;
        cameraRight.y = 0;

        // ���� ����ȭ
        cameraForward.Normalize();
        cameraRight.Normalize();

        // �̵� ���� ���
        moveVec = (cameraForward * v + cameraRight * h).normalized;
        if (isDodge)
           // moveVec = dodgeVec; // ������ ���̶�� ������ ���ͷ� �̵�

        // �̵� ó��
        if (Run)
        {
            transform.position += moveVec * MoveSpeed * 2.0f * Time.deltaTime; // �޸��� �ӵ��� �̵�
            animator.SetBool("isRun", true); // �޸��� �ִϸ��̼� ����
        }
        else
        {
            transform.position += moveVec * MoveSpeed * Time.deltaTime; // �ȱ� �ӵ��� �̵�
            animator.SetBool("isRun", false); // �޸��� �ִϸ��̼� ����
        }

        animator.SetBool("isWalk", moveVec != Vector3.zero); // �ȱ� �ִϸ��̼� ����

        // ���¹̳� ȸ�� ó��
        currentStamina = Mathf.Clamp(currentStamina + staminaRecoveryRate * Time.deltaTime, 0, maxStamina);
    }

    // ȸ�� ó�� (���� ���콺 �Է� �̱���)
    void Rotate()
    {
        transform.LookAt(transform.position + moveVec); // �̵� �������� ȸ��
    }

    // ���� ó�� (�ִϸ��̼� exit ����)
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse); // ���� ���� ���� ����
            animator.SetTrigger("isJump"); // ���� �ִϸ��̼� ����
            isJump = true; // ���� ���� ����
        }
        
        isJump = false;
    }

    void BackStep()
    {
        if (Input.GetKeyDown(KeyCode.N) && !Run && !isJump && !isShieldActive && !isShieldHit)
        {
            Backstep = true;
            Vector3 backVec = -transform.forward * 3.0f;

            transform.Translate(backVec, Space.World);

            animator.SetBool("Backstep", true);

        }
        else
        {
            animator.SetBool("Backstep", false);
           // Backstep = false;

        }
    }

    // ���� ������ ���� ����
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("isJUmp", false); // ���� �ִϸ��̼� ����
            isJump = false; // ���� ���� ����
        }
    }

    // ������ ó�� (�ִϸ��̼� ����)
    void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.M) && !isJump)
        {
            dodgeVec = transform.forward * 3.0f;// ������ ���� ����
            transform.Translate(dodgeVec, Space.World);
           // rb.AddForce(Vector3.up * 5, ForceMode.Impulse); // ���� ���� ���� ������
            animator.SetBool("isDodge", true); // ������ �ִϸ��̼� ����
            isDodge = true; // ������ ���� ����
            Invoke("DodgeOut", 0.5f); // 0.5�� �� ������ ����
            currentStamina -= currentStamina - 3; // ���¹̳� �Ҹ�
        }
        else
        {
            // isDodge = false; // ������ ���� ����
            animator.SetBool("isDodge", false);

        }
    }

    public void GetDamage(int damage)
    {
        if (currentHP > 0)
        {
            currentHP -= damage;
            _hpBar.value = currentHP;
            nextHP = currentHP;
            currentTime = 0.0f; // �� �������� ���� ������ currentTime�� �ʱ�ȭ
            totalDamageTaken += damage; // ���� ������ ������Ʈ
        }
    }

    // ���� ó��
    void Attack()
    {

        if (equipWeapon == null)
            return; // ���Ⱑ ���ٸ� ���� ����


        if (Input.GetButtonDown("Fire1") && !isDodge && !isSwap)
        {
            isAttack = true;
            animator.SetTrigger("isAttack"); // ���� �ִϸ��̼� ����

            currentStamina -= currentStamina - 4; // ���¹̳� �Ҹ�

            // ȭ�� ���� (�̱���)
            //if (Input.GetButtonUp("Fire2"))
        }
        isAttack = false;
    }

    // ȸ�� ������ ��� (K Ű ������ �۵�)
    void potion()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            // ü�� ȸ��
            currentHP += healAmount;
            _hpBar.value = currentHP;
            _nextHpBar.value = currentHP;

            // ü���� �ִ밪���� ����
            if (currentHP >= maxHP)
            {
                currentHP = maxHP;
            }
        }
    }

    // ���� ��ü ó�� (����� �۵����� ����, �ֿ� ���Ⱑ ����)
    void Swap()
    {
        int weaponIndex = -1;
        if (Input.GetButtonDown("Swap1")) weaponIndex = 0;
        if (Input.GetButtonDown("Swap2")) weaponIndex = 1;
        if (Input.GetButtonDown("Swap3")) weaponIndex = 2;

        if ((Input.GetButtonDown("Swap1") || Input.GetButtonDown("Swap2") || Input.GetButtonDown("Swap3")) && !isJump && !isDodge)
        {
            if (equipWeapon != null)
            {
                Weapon[weaponIndex].gameObject.SetActive(false); // ���� ���� ��Ȱ��ȭ
                equipWeapon = Weapon[weaponIndex].GetComponent<Sward>(); // ���� ��ü
                Weapon[weaponIndex].gameObject.SetActive(true); // ���ο� ���� Ȱ��ȭ

                animator.SetTrigger("Swap"); // ���� ��ü �ִϸ��̼� ����
                isSwap = true; // ���� ��ü ���� ����

                Invoke("SwapOut", 0.4f); // 0.4�� �� ���� ��ü ����
            }
        }
    }

    // ���� ��ü ����
    void Swapout()
    {
        isSwap = false; // ���� ��ü ���� ����
    }
    void Shield()
    {
       // ShieldCollider = ShieldPrefab.GetComponent<BoxCollider>();
        if(Input.GetMouseButton(1))
        {
           // if (isShieldActive) return; // ���尡 �̹� Ȱ��ȭ �Ǿ��ִٸ� ��ȯ
            
            isShieldActive = true; // ������� Ʈ�� ����
            animator.SetTrigger("isShield"); // �ִϸ��̼� ���
            
            //isShieldActive�� true�ϋ� �����ǿ��� ������ ���ϴ°ɷ� �Ǿ��ִµ� ... �� ������ ��밡 �����ϴµ� �� �������� �����ϰ� �ѹ� �����ϸ� isShieldActive �� false �Ǵ°ǵ� ��... �������� ����

            print("������");
            if(isShieldActive && Scorpion.isAttackTrue ) // ��������϶� ������ Shield hit �ִϸ��̼� ��� 
            {
                isShieldHit = true;
                animator.SetBool("isShieldHit", true);
                print("shield hit!");
                isShieldActive = false;
            }
        }
        else 
        {
            //������� false ���� 
            //���� �ڽ��ݶ��̴� ��Ȱ��ȭ 
            isShieldActive = false;
            animator.SetBool("isShieldHit", false);
        }
        
    }

    // ������ ó�� (������ ������ �Է� ó�� �̱���)
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Scorpion mob = other.GetComponent<Scorpion>();
            if (mob != null && !hitMonster.Contains(mob))
            {
                hitMonster.Add(mob);
                mob.anim.SetTrigger("Hit"); // �ǰ� �ִϸ��̼� ����
            }

        }
        
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ���� �޼���
    public void ApplyDamage()
    {
        if (hitMonster != null)
        {
            foreach (Scorpion monster in hitMonster)
            {
                if (monster != null)
                {
                    monster.GetDamage(equipWeapon.attackPower);
                }
            }
            hitMonster.Clear(); // �������� ���� �� ����Ʈ �ʱ�ȭ
        }
    }

    // ������ �Ա� ó�� (������ٵ� ����Ͽ� ������ ó�� �̱���)
    void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
        currentHP = Mathf.Clamp(currentHP - atkPower, 0, maxHP); // ü�� ���� �� ����

        if (currentHP <= 0) // ü���� 0 ������ ��
        {
            currentHP = 0;

            animator.SetTrigger("Die"); // ��� �ִϸ��̼� ����
            GetComponent<CharacterController>().enabled = false; // ĳ���� ��Ʈ�ѷ� ��Ȱ��ȭ
        }
        else
        {
            // �ǰ� �ִϸ��̼� ����
            animator.SetTrigger("Hit");
        }
    }
}
