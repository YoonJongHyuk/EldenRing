using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yoon;
using UnityEngine.UI;

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

    // 백스탭 관련 변수
    bool Backstep = false;

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
    bool isAttack;

    // 쉴드 관련 변수들
    public GameObject ShieldPrefab;
    public bool isShieldActive = false; //방패 상태
    public bool isShieldHit = false; // 맞는 방패상태

    // 화살 관련 변수들
    public Transform ArrowPos; // 화살 위치
    public GameObject Arrow2; // 화살 오브젝트

    // 기타 기능 관련 변수들
    Animator animator; // 애니메이터
    Rigidbody rb; // 리지드바디

    // 스태미나 관련 변수들
    public float maxStamina = 10.0f; // 최대 스태미나
    public float currentStamina = 10.0f; // 현재 스태미나
    public float staminaRecoveryRate = 4.0f; // 스태미나 회복 속도
    public float staminaDrainRateAttack = 2.0f; // 공격 시 스태미나 소모량
    public float staminaDrainRateDodge = 3.0f; // 구르기 시 스태미나 소모량

    [SerializeField]
    private Slider _hpBar;

    [SerializeField]
    private Slider _nextHpBar; // nextHP 용 슬라이더 추가

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
        isShieldActive =ShieldPrefab.GetComponent<BoxCollider>(); // 쉴드 박스콜라이더 가지고옴
    }

    // Update는 매 프레임 호출됩니다.
    void Update()
    {
        Move(); // 이동 처리
        Rotate(); // 회전 처리
        Jump(); // 점프 처리
        //Interation(); // 상호작용 처리
        Attack(); // 공격 처리
        Swap(); // 무기 교체 처리
        Swapout(); // 무기 교체 해제 처리
        potion(); // 포션 사용 처리
        HPBar();
        Shield();
        BackStep();
        Dodge();
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
        if (isDodge)
           // moveVec = dodgeVec; // 구르기 중이라면 구르기 벡터로 이동

        // 이동 처리
        if (Run)
        {
            transform.position += moveVec * MoveSpeed * 2.0f * Time.deltaTime; // 달리기 속도로 이동
            animator.SetBool("isRun", true); // 달리기 애니메이션 설정
        }
        else
        {
            transform.position += moveVec * MoveSpeed * Time.deltaTime; // 걷기 속도로 이동
            animator.SetBool("isRun", false); // 달리기 애니메이션 해제
        }

        animator.SetBool("isWalk", moveVec != Vector3.zero); // 걷기 애니메이션 설정

        // 스태미나 회복 처리
        currentStamina = Mathf.Clamp(currentStamina + staminaRecoveryRate * Time.deltaTime, 0, maxStamina);
    }

    // 회전 처리 (아직 마우스 입력 미구현)
    void Rotate()
    {
        transform.LookAt(transform.position + moveVec); // 이동 방향으로 회전
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

    // 땅에 닿으면 점프 해제
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("isJUmp", false); // 점프 애니메이션 해제
            isJump = false; // 점프 상태 해제
        }
    }

    // 구르기 처리 (애니메이션 없음)
    void Dodge()
    {
        if (Input.GetKeyDown(KeyCode.M) && !isJump)
        {
            dodgeVec = transform.forward * 3.0f;// 구르기 벡터 설정
            transform.Translate(dodgeVec, Space.World);
           // rb.AddForce(Vector3.up * 5, ForceMode.Impulse); // 위로 힘을 가해 구르기
            animator.SetBool("isDodge", true); // 구르기 애니메이션 설정
            isDodge = true; // 구르기 상태 설정
            Invoke("DodgeOut", 0.5f); // 0.5초 후 구르기 해제
            currentStamina -= currentStamina - 3; // 스태미나 소모
        }
        else
        {
            // isDodge = false; // 구르기 상태 해제
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


        if (Input.GetButtonDown("Fire1") && !isDodge && !isSwap)
        {
            isAttack = true;
            animator.SetTrigger("isAttack"); // 공격 애니메이션 설정

            currentStamina -= currentStamina - 4; // 스태미나 소모

            // 화살 공격 (미구현)
            //if (Input.GetButtonUp("Fire2"))
        }
        isAttack = false;
    }

    // 회복 아이템 사용 (K 키 누르면 작동)
    void potion()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            // 체력 회복
            currentHP += healAmount;
            _hpBar.value = currentHP;
            _nextHpBar.value = currentHP;

            // 체력을 최대값으로 제한
            if (currentHP >= maxHP)
            {
                currentHP = maxHP;
            }
        }
    }

    // 무기 교체 처리 (제대로 작동하지 않음, 주운 무기가 없음)
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
                Weapon[weaponIndex].gameObject.SetActive(false); // 현재 무기 비활성화
                equipWeapon = Weapon[weaponIndex].GetComponent<Sward>(); // 무기 교체
                Weapon[weaponIndex].gameObject.SetActive(true); // 새로운 무기 활성화

                animator.SetTrigger("Swap"); // 무기 교체 애니메이션 설정
                isSwap = true; // 무기 교체 상태 설정

                Invoke("SwapOut", 0.4f); // 0.4초 후 무기 교체 해제
            }
        }
    }

    // 무기 교체 해제
    void Swapout()
    {
        isSwap = false; // 무기 교체 상태 해제
    }
    void Shield()
    {
       // ShieldCollider = ShieldPrefab.GetComponent<BoxCollider>();
        if(Input.GetMouseButton(1))
        {
           // if (isShieldActive) return; // 쉴드가 이미 활성화 되어있다면 반환
            
            isShieldActive = true; // 쉴드상태 트루 변경
            animator.SetTrigger("isShield"); // 애니메이션 재생
            
            //isShieldActive가 true일떄 스콜피온이 공격을 안하는걸로 되어있는데 ... 내 생각엔 상대가 공격하는데 그 데미지를 무시하고 한번 무시하면 isShieldActive 가 false 되는건데 어... 쉽지않음 보류

            print("쉴드중");
            if(isShieldActive && Scorpion.isAttackTrue ) // 쉴드상태일때 맞으면 Shield hit 애니메이션 재생 
            {
                isShieldHit = true;
                animator.SetBool("isShieldHit", true);
                print("shield hit!");
                isShieldActive = false;
            }
        }
        else 
        {
            //쉴드상태 false 변경 
            //쉴드 박스콜라이더 비활성화 
            isShieldActive = false;
            animator.SetBool("isShieldHit", false);
        }
        
    }

    // 데미지 처리 (몬스터의 데미지 입력 처리 미구현)
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Scorpion mob = other.GetComponent<Scorpion>();
            if (mob != null && !hitMonster.Contains(mob))
            {
                hitMonster.Add(mob);
                mob.anim.SetTrigger("Hit"); // 피격 애니메이션 설정
            }

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

    // 데미지 입기 처리 (리지드바디를 사용하여 데미지 처리 미구현)
    void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
        currentHP = Mathf.Clamp(currentHP - atkPower, 0, maxHP); // 체력 감소 및 제한

        if (currentHP <= 0) // 체력이 0 이하일 때
        {
            currentHP = 0;

            animator.SetTrigger("Die"); // 사망 애니메이션 설정
            GetComponent<CharacterController>().enabled = false; // 캐릭터 컨트롤러 비활성화
        }
        else
        {
            // 피격 애니메이션 설정
            animator.SetTrigger("Hit");
        }
    }
}
