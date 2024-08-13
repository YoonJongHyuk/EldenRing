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
    // 이동 관련 변수들
    public float MoveSpeed; // 이동 속도
    float h, v; // 수평 및 수직 입력값
    bool isMove;
    bool Run; // 달리기 여부
    Vector3 moveVec; // 이동 벡터
    public Transform cameraTransform; // 카메라 Transform

    // 점프 관련 변수들
    bool isJump = false; // 점프 상태
    Vector3 gravityPower; // 중력 값

    // 구르기 관련 변수들
    bool isDodge; // 구르기 상태
    Vector3 dodgeVec; // 구르기 벡터
    Transform pivot;

    // 백스탭 관련 변수
    bool Backstep = false;
    Vector3 BackVec;

    // 무기 관련 변수들
    //public GameObject nearObject; // 근처 오브젝트
    public GameObject[] Weapon; // 무기 배열
    public Sward equipWeapon; // 장착된 무기
    public bool[] hasWeapone; // 무기 보유 여부
    public bool hiding; // 플레이어의 숨은 상태 여부
    bool iDown; // 상호작용 키 입력 상태
    bool Swap1, Swap2, Swap3; // 무기 교체 키 입력 상태
    bool isSwap; // 무기 교체 중인지 여부

    // 회복 아이템 관련 변수들
    public GameObject potionprefab; // 포션 프리팹
    public int portionNum = 5;
    public TMP_Text portionText;

    public float healAmount = 50f; // 회복량
    public float maxHP = 100; // 최대 체력
    public float currentHP = 10; // 현재 체력 (테스트용)
    float nextHP;
    float currentTime = 0;
    private float lerpDuration = 1.5f; // 체력이 천천히 빠질 시간 (1.5초)
    private float delayDuration = 1.5f; // 딜레이
    private int totalDamageTaken = 0; // 누적 데미지

    // 공격 관련 변수들
    bool MeleeAttack; // 근접 공격 여부
    float AttackDelay; // 공격 지연 시간
    private List<Scorpion> scorps; // 충돌한 Monster 객체
    private List<Scorpion> hitMonster;
    public bool isAttack;
    public bool hit;

    public bool playerHit = false;

    // 쉴드 관련 변수들
    public GameObject ShieldPrefab;
    public bool isShieldActive = false; //방패 상태
    public bool isShieldHit = false; // 맞는 방패상태

    // 화살 관련 변수들
    public Transform ArrowPos; // 화살 위치
    public GameObject Arrow2; // 화살 오브젝트

    // 기타 기능 관련 변수들
    public Animator animator; // 애니메이터
    Rigidbody rb; // 리지드바디

    // 스태미나 관련 변수들
    public float maxStamina = 1.0f; // 최대 스태미나
    public float currentStamina; // 현재 스태미나
    public float staminaRecoveryRate = 0.004f; // 스태미나 회복 속도
    public float staminaDrainRateAttack = 0.2f; // 공격 시 스태미나 소모량
    public float staminaDrainRateDodge = 0.3f; // 구르기 시 스태미나 소모량
    bool block = true;

    //사망
    public GameObject DiePanel;

    [SerializeField]
    private Slider _hpBar;

    [SerializeField]
    private Slider _nextHpBar; // nextHP 용 슬라이더 추가

    // 시네머신 카메라 
    public CinemachineFreeLook Vcamera;
    public Transform L_target;
    public bool targetLocked = false;


    //사망 (리스폰)
    public bool isDead = false;
    Transform Respawn;
    private Vector3 respawnPosition;
    private bool canSetRespawn = false; // 리스폰 포인트 설정 가능 여부


    Scorpion Scorpion;

    // Start는 첫 프레임 업데이트 전에 호출됩니다.
    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // 리지드바디 컴포넌트 가져오기
        animator = GetComponentInChildren<Animator>(); // 자식 객체에서 애니메이터 컴포넌트 가져오기
        hiding = true;
        scorps = new List<Scorpion>(); // Scorpion 리스트 초기화
        hitMonster = new List<Scorpion>(); // hitMonster 리스트 초기화
    }

    void Start()
    {
        //시네머신카메라
        Vcamera = FindObjectOfType<CinemachineFreeLook>();
        // currentHP = maxHP; // 회복을 보이기 위해 초기화 생략
        currentStamina = maxStamina; // 현재 스태미나를 최대값으로 설정
        equipWeapon = Weapon[0].GetComponent<Sward>();
        Cursor.lockState = CursorLockMode.Locked;
        currentHP = maxHP;
        nextHP = currentHP;
        _hpBar.maxValue = currentHP;
        _hpBar.value = currentHP;
        _nextHpBar.maxValue = currentHP; // nextHP 슬라이더 초기화
        _nextHpBar.value = currentHP;
        isShieldActive = ShieldPrefab.GetComponent<BoxCollider>(); // 쉴드 박스콜라이더 가지고옴
        portionText.text = portionNum.ToString();
    }

    // Update는 매 프레임 호출됩니다.
    void Update()
    {
        if (!playerHit && !isDead && block)
        {
            Attack(); // 공격 처리
            //Shield(); // 쉴드
            BackStep(); // 빽스탭
            Stamina(); // 스테미나 처리 
            Dodge(); // 구르기
        }
        if(!playerHit && !isDead)
        {
            Move(); // 이동 처리
            Rotate(); // 회전 처리
            Jump(); // 점프 처리
            Death(); 
            potion(); // 포션 사용 처리
        }
        HPBar(); // 체력바
        
        //Cam(); // Lookon
        CheckRespawn(); // 리스폰 체크 및 리스폰
        SetRespawnPosition(); //리스폰 프리팹 상호작용
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
        if (_nextHpBar == null) return; // nextHP 슬라이더가 할당되지 않았으면 리턴

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
        // 스태미나 회복 처리 // 회복이 안됨 -- 해결
        currentStamina = Mathf.Clamp(currentStamina + staminaRecoveryRate * Time.deltaTime, 0, maxStamina);
        // 현재 소비할려는 스테미너 보다 사용량이 적을경우
        
    
    }
    void staminaValue(float Value)
    {
        if (currentStamina < Value)
        {
            // 행동을 불가능하게 함
            block = false;
            return;
            if (currentStamina > Value)
            {
                block = true;
            }
        }


    }

    // 기본 움직임 처리
    void Move()
    {
        h = Input.GetAxisRaw("Horizontal"); // 수평 입력 값 가져오기
        v = Input.GetAxisRaw("Vertical"); // 수직 입력 값 가져오기
        Run = Input.GetButton("shift"); // 달리기 입력 값 가져오기

        // 카메라의 forward와 right 방향 가져오기
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // y축 값 0으로 설정하여 수평 이동만 고려
        cameraForward.y = 0;
        cameraRight.y = 0;

        // 벡터 정규화
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 이동 벡터 계산
        moveVec = (cameraForward * v + cameraRight * h).normalized;


        //이동 처리
        if (Run)
        {
            Run = true;
            transform.position += moveVec * MoveSpeed * 2.0f * Time.deltaTime; // 달리기 속도로 이동
            animator.SetBool("isRun", true); // 달리기 애니메이션 설정
            
        }
        else
        {
            Run = false;
            transform.position += moveVec * MoveSpeed * Time.deltaTime; // 걷기 속도로 이동
            animator.SetBool("isRun", false); // 달리기 애니메이션 해제

        }

        animator.SetBool("isWalk", moveVec != Vector3.zero); // 걷기 애니메이션 설정
        Run = false;
        

    }

    // 회전 처리 (아직 마우스 입력 미구현)
    void Rotate()
    {
        if (moveVec != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            transform.LookAt(transform.position + moveVec); // 이동 방향으로 회전

        }
    }

    // 점프 처리 (애니메이션 exit 문제)
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse); // 위로 힘을 가해 점프
            animator.SetTrigger("isJump"); // 점프 애니메이션 설정
            isJump = true; // 점프 상태 설정
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

    // 땅에 닿으면 점프 해제
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("isJUmp", false); // 점프 애니메이션 해제
            isJump = false; // 점프 상태 해제
        }
    }

    void Dodge() // 애니메이션이 true에서 빠져나가지 못함 
    {
            
        if (Input.GetKeyDown(KeyCode.F))// && isMove && !isJump)
        {
            Vector3 dodgeVec = new Vector3(h, 0, v);  // 구르기 벡터 설정
            transform.Translate(dodgeVec * Time.deltaTime);
            currentStamina = currentStamina - staminaDrainRateDodge; // 스태미나 소모
            staminaValue(staminaDrainRateDodge);
            animator.SetBool("isDodge", true); // 구르기 애니메이션 설정
            isDodge = true; // 구르기 상태 설정
            //return;
        } 
        else
        {
            isDodge = false; // 구르기 상태 해제
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
            currentTime = 0.0f; // 새 데미지를 받을 때마다 currentTime을 초기화
            totalDamageTaken += damage; // 누적 데미지 업데이트
        }
    }

    // 공격 처리
    void Attack()
    {
        if (equipWeapon == null)
            return; // 무기가 없다면 공격 중지


        if (Input.GetButtonDown("Fire1") && !isDodge && !isSwap && !isAttack)
        {
            
            animator.SetTrigger("isAttack"); // 공격 애니메이션 설정

            currentStamina = currentStamina - staminaDrainRateAttack; // 스태미나 소모

            staminaValue(staminaDrainRateAttack);
            // 화살 공격 (미구현)
            //if (Input.GetButtonUp("Fire2"))
        }
        
    }

    void StartAttack()
    {
        isAttack = true;
        hit = true;
        print("공격시작. isAttack의 값은" + isAttack);
    }

    void EndAttack()
    {
        isAttack = false;
        hit = false;
        print("공격끝. isAttack의 값은" + isAttack);
    }

    // 회복 아이템 사용 (R 키 누르면 작동)
    void potion()
    {
        if (Input.GetKeyDown(KeyCode.R) && portionNum != 0)
        {
            // 체력 회복
            currentHP += healAmount;
            _hpBar.value = currentHP;
            _nextHpBar.value = currentHP;
            portionNum--;
            portionText.text = portionNum.ToString();

            // 체력을 최대값으로 제한
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
            // if (isShieldActive) return; // 쉴드가 이미 활성화 되어있다면 반환
            Run = false; // 달리기 불가 
            isShieldActive = true; // 쉴드상태 트루 변경
            animator.SetTrigger("isShield"); // 애니메이션 재생

            //isShieldActive가 true일떄 스콜피온이 공격을 안하는걸로 되어있는데 ... 내 생각엔 상대가 공격하는데 그 데미지를 무시하고 한번 무시하면 isShieldActive 가 false 되는건데 어... 쉽지않음 보류

            print("쉴드중");
            if (isShieldActive) //&& Scorpion.isAttackTrue) // 쉴드상태일때 적한테 맞으면 Shield hit 애니메이션 재생 
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
            //쉴드상태 false 변경 
            //쉴드 박스콜라이더 비활성화 
            isShieldActive = false;
            animator.SetBool("isShieldHit", false);
        }
        if (Input.GetKeyDown(KeyCode.T)) // 쉴드어택 방패로 때림 F
        {
            isShieldActive = false;
            animator.SetBool("ShieldAttack", true);
        }
        Run = true;
    }

    // 데미지 처리 (몬스터의 데미지 입력 처리 미구현)
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Respawn"))
        {
            canSetRespawn = true; // 리스폰 포인트 설정 가능 상태로 변경
        }


    }
    void SetRespawnPosition()
    {
        if (canSetRespawn && Input.GetKeyDown(KeyCode.E))
        {
            respawnPosition = transform.position; // 현재 위치를 리스폰 위치로 설정
        }
    }

    // 애니메이션 이벤트가 호출할 메서드
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
            hitMonster.Clear(); // 데미지를 입힌 후 리스트 초기화
        }
    }

    //// 데미지 입기 처리 (리지드바디를 사용하여 데미지 처리 미구현)
    //void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    //{
    //    currentHP = Mathf.Clamp(currentHP - atkPower, 0, maxHP); // 체력 감소 및 제한
    //    if (Input.GetMouseButton(1))
    //    {
    //        isShieldHit = true;
    //        animator.SetBool("isShieldHit", true);

    //        return;
    //    }

    //    if (currentHP <= 0 && isDead == false) // 체력이 0 이하일 때
    //    {
    //        currentHP = 0;

    //        animator.SetTrigger("Die"); // 사망 애니메이션 설정
    //        isDead = true;
    //        Death();
    //        GetComponent<CharacterController>().enabled = false; // 캐릭터 컨트롤러 비활성화
    //        CheckRespawn();
    //    }
    //    else
    //    {
    //        // 피격 애니메이션 설정
    //        animator.SetTrigger("Hit");
    //    }
    //}

    void Death()
    {
        if (currentHP <= 0 && isDead == false)
        {
            isDead = true;
            print("죽음");
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
            //리스폰으로 이동
            transform.position = Respawn.position; //리스폰위치로 이동
            currentHP = maxHP;
            currentStamina = maxStamina;
            _hpBar.value = currentHP;
            _nextHpBar.value = currentHP;
            isDead = false;
            GetComponent<CharacterController>().enabled = true; // 캐릭터컨트롤러활성화
            animator.SetTrigger("Respawn");
        }
        //if (Respawn == null)
        //{
        //    SceneManager.LoadScene(0);
        //    return;
        //}

    }
}
