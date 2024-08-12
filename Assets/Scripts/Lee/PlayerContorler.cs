using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yoon;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;

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
    Transform pivot;

    // �齺�� ���� ����
    bool Backstep = false;
    Vector3 BackVec;

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
    public int portionNum = 5;
    public TMP_Text portionText;

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
    public bool isAttack;
    public bool hit;

    public bool playerHit = false;

    // ���� ���� ������
    public GameObject ShieldPrefab;
    public bool isShieldActive = false; //���� ����
    public bool isShieldHit = false; // �´� ���л���

    // ȭ�� ���� ������
    public Transform ArrowPos; // ȭ�� ��ġ
    public GameObject Arrow2; // ȭ�� ������Ʈ

    // ��Ÿ ��� ���� ������
    public Animator animator; // �ִϸ�����
    Rigidbody rb; // ������ٵ�

    // ���¹̳� ���� ������
    public float maxStamina = 1.0f; // �ִ� ���¹̳�
    public float currentStamina; // ���� ���¹̳�
    public float staminaRecoveryRate = 0.004f; // ���¹̳� ȸ�� �ӵ�
    public float staminaDrainRateAttack = 0.2f; // ���� �� ���¹̳� �Ҹ�
    public float staminaDrainRateDodge = 0.3f; // ������ �� ���¹̳� �Ҹ�
    bool block = true;

    //���
    public GameObject DiePanel;

    [SerializeField]
    private Slider _hpBar;

    [SerializeField]
    private Slider _nextHpBar; // nextHP �� �����̴� �߰�

    // �ó׸ӽ� ī�޶� 
    public CinemachineFreeLook Vcamera;
    public Transform L_target;
    public bool targetLocked = false;


    //��� (������)
    public bool isDead = false;
    Transform Respawn;
    private Vector3 respawnPosition;
    private bool canSetRespawn = false; // ������ ����Ʈ ���� ���� ����


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
        //�ó׸ӽ�ī�޶�
        Vcamera = FindObjectOfType<CinemachineFreeLook>();
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
        isShieldActive = ShieldPrefab.GetComponent<BoxCollider>(); // ���� �ڽ��ݶ��̴� �������
        portionText.text = portionNum.ToString();
    }

    // Update�� �� ������ ȣ��˴ϴ�.
    void Update()
    {
        if (!playerHit && !isDead && block)
        {
            Attack(); // ���� ó��
            //Shield(); // ����
            BackStep(); // ������
            Stamina(); // ���׹̳� ó�� 
            Dodge(); // ������
        }
        if(!playerHit && !isDead)
        {
            Move(); // �̵� ó��
            Rotate(); // ȸ�� ó��
            Jump(); // ���� ó��
            Death(); 
            potion(); // ���� ��� ó��
        }
        HPBar(); // ü�¹�
        
        //Cam(); // Lookon
        CheckRespawn(); // ������ üũ �� ������
        SetRespawnPosition(); //������ ������ ��ȣ�ۿ�
        currentStamina = Mathf.Clamp(currentStamina + staminaRecoveryRate * Time.deltaTime, 0, maxStamina);
        if (playerHit && !isDead)
        {
            playerHit = false;
            animator.SetTrigger("Hit");
        }
        print(currentStamina);
    }

    void PlayerHitAfter()
    {
        playerHit = false;
        if (isAttack)
        {
            isAttack = false;
        }
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

    void Stamina()
    {
        // ���¹̳� ȸ�� ó�� // ȸ���� �ȵ� -- �ذ�
        currentStamina = Mathf.Clamp(currentStamina + staminaRecoveryRate * Time.deltaTime, 0, maxStamina);
        // ���� �Һ��ҷ��� ���׹̳� ���� ��뷮�� �������
        
    
    }
    void staminaValue(float Value)
    {
        if (currentStamina < Value)
        {
            // �ൿ�� �Ұ����ϰ� ��
            block = false;
        if (currentStamina > Value)
            {
                block = true;
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


        //�̵� ó��
        if (Run)
        {
            Run = true;
            transform.position += moveVec * MoveSpeed * 2.0f * Time.deltaTime; // �޸��� �ӵ��� �̵�
            animator.SetBool("isRun", true); // �޸��� �ִϸ��̼� ����
            
        }
        else
        {
            Run = false;
            transform.position += moveVec * MoveSpeed * Time.deltaTime; // �ȱ� �ӵ��� �̵�
            animator.SetBool("isRun", false); // �޸��� �ִϸ��̼� ����

        }

        animator.SetBool("isWalk", moveVec != Vector3.zero); // �ȱ� �ִϸ��̼� ����
        Run = false;
        

    }

    // ȸ�� ó�� (���� ���콺 �Է� �̱���)
    void Rotate()
    {
        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            transform.LookAt(transform.position + moveVec); // �̵� �������� ȸ��

        }
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

        if (Input.GetKeyDown(KeyCode.U) && BackVec.magnitude == 0)
        {
            Backstep = true;
            Vector3 backVec = -transform.forward * 3.0f;

            transform.Translate(backVec, Space.World);
            animator.SetBool("Backstep", true);
            currentStamina = currentStamina - staminaDrainRateDodge;
            staminaValue(staminaDrainRateDodge);
        }
        else
        {
            animator.SetBool("Backstep", false);
            Backstep = false;
            
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

    void Dodge() // �ִϸ��̼��� true���� ���������� ���� 
    {
        if (Input.GetKeyDown(KeyCode.F))// && isMove && !isJump)
        {
            Vector3 dodgeVec = new Vector3(h, 0, v);  // ������ ���� ����
            transform.Translate(dodgeVec * Time.deltaTime);
            Invoke("DodgeOut", 0.5f); // 0.5�� �� ������ ����
            currentStamina = currentStamina - staminaDrainRateDodge; // ���¹̳� �Ҹ�
            staminaValue(staminaDrainRateDodge);
            animator.SetBool("isDodge", true); // ������ �ִϸ��̼� ����
            isDodge = true; // ������ ���� ����
            return;
        } // �� ���⼭ ������������;;;;
        //if(isDodge)
        //{
        //    return;
        //}
        else
        {
            isDodge = false; // ������ ���� ����
            animator.SetBool("isDodge", false);
            CancelInvoke();
            
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


        if (Input.GetButtonDown("Fire1") && !isDodge && !isSwap && !isAttack)
        {
            
            animator.SetTrigger("isAttack"); // ���� �ִϸ��̼� ����

            currentStamina = currentStamina - staminaDrainRateAttack; // ���¹̳� �Ҹ�

            staminaValue(staminaDrainRateAttack);
            // ȭ�� ���� (�̱���)
            //if (Input.GetButtonUp("Fire2"))
        }
        
    }

    void StartAttack()
    {
        isAttack = true;
        hit = true;
        print("���ݽ���. isAttack�� ����" + isAttack);
    }

    void EndAttack()
    {
        isAttack = false;
        hit = false;
        print("���ݳ�. isAttack�� ����" + isAttack);
    }

    // ȸ�� ������ ��� (R Ű ������ �۵�)
    void potion()
    {
        if (Input.GetKeyDown(KeyCode.R) && portionNum != 0)
        {
            // ü�� ȸ��
            currentHP += healAmount;
            _hpBar.value = currentHP;
            _nextHpBar.value = currentHP;
            portionNum--;
            portionText.text = portionNum.ToString();

            // ü���� �ִ밪���� ����
            if (currentHP >= maxHP)
            {
                currentHP = maxHP;
            }
        }
    }

    void Shield()
    {
        // ShieldCollider = ShieldPrefab.GetComponent<BoxCollider>();
        if (Input.GetMouseButton(1))
        {
            // if (isShieldActive) return; // ���尡 �̹� Ȱ��ȭ �Ǿ��ִٸ� ��ȯ
            Run = false; // �޸��� �Ұ� 
            isShieldActive = true; // ������� Ʈ�� ����
            animator.SetTrigger("isShield"); // �ִϸ��̼� ���

            //isShieldActive�� true�ϋ� �����ǿ��� ������ ���ϴ°ɷ� �Ǿ��ִµ� ... �� ������ ��밡 �����ϴµ� �� �������� �����ϰ� �ѹ� �����ϸ� isShieldActive �� false �Ǵ°ǵ� ��... �������� ����

            print("������");
            if (isShieldActive) //&& Scorpion.isAttackTrue) // ��������϶� ������ ������ Shield hit �ִϸ��̼� ��� 
            {
                isShieldHit = true;
                animator.SetBool("isShieldHit", true);
                print("shield hit!");
                isShieldActive = false;
                return;
            }
        }
        else
        {
            //������� false ���� 
            //���� �ڽ��ݶ��̴� ��Ȱ��ȭ 
            isShieldActive = false;
            animator.SetBool("isShieldHit", false);
        }
        if (Input.GetKeyDown(KeyCode.T)) // ������� ���з� ���� F
        {
            isShieldActive = false;
            animator.SetBool("ShieldAttack", true);
        }
        Run = true;
    }

    // ������ ó�� (������ ������ �Է� ó�� �̱���)
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Respawn"))
        {
            canSetRespawn = true; // ������ ����Ʈ ���� ���� ���·� ����
        }


    }
    void SetRespawnPosition()
    {
        if (canSetRespawn && Input.GetKeyDown(KeyCode.E))
        {
            respawnPosition = transform.position; // ���� ��ġ�� ������ ��ġ�� ����
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

    //// ������ �Ա� ó�� (������ٵ� ����Ͽ� ������ ó�� �̱���)
    //void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    //{
    //    currentHP = Mathf.Clamp(currentHP - atkPower, 0, maxHP); // ü�� ���� �� ����
    //    if (Input.GetMouseButton(1))
    //    {
    //        isShieldHit = true;
    //        animator.SetBool("isShieldHit", true);

    //        return;
    //    }

    //    if (currentHP <= 0 && isDead == false) // ü���� 0 ������ ��
    //    {
    //        currentHP = 0;

    //        animator.SetTrigger("Die"); // ��� �ִϸ��̼� ����
    //        isDead = true;
    //        Death();
    //        GetComponent<CharacterController>().enabled = false; // ĳ���� ��Ʈ�ѷ� ��Ȱ��ȭ
    //        CheckRespawn();
    //    }
    //    else
    //    {
    //        // �ǰ� �ִϸ��̼� ����
    //        animator.SetTrigger("Hit");
    //    }
    //}

    void Death()
    {
        if (currentHP <= 0 && isDead == false)
        {
            isDead = true;
            print("����");
            DiePanel.SetActive(true);
            animator.SetTrigger("Die");
            transform.tag = "Untagged";
            StartCoroutine(IDeath());
        }
    }

    IEnumerator IDeath()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(1);
    }

    void CheckRespawn()
    {
        if (isDead && Respawn != null)
        {
            //���������� �̵�
            transform.position = Respawn.position; //��������ġ�� �̵�
            currentHP = maxHP;
            currentStamina = maxStamina;
            _hpBar.value = currentHP;
            _nextHpBar.value = currentHP;
            isDead = false;
            GetComponent<CharacterController>().enabled = true; // ĳ������Ʈ�ѷ�Ȱ��ȭ
            animator.SetTrigger("Respawn");
        }
        //if (Respawn == null)
        //{
        //    SceneManager.LoadScene(0);
        //    return;
        //}

    }
}
