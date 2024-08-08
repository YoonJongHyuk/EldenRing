using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefMovement : MonoBehaviour
{
    CharacterController controller; // ĳ���� �̵��� �����ϴ� CharacterController.
    Animator anim; // ĳ���� �ִϸ��̼��� �����ϴ� Animator.
    Transform cam; // ���� ī�޶��� Transform.

    float speedSmoothVelocity; // �ӵ� ������ ���Ǵ� ����.
    float speedSmoothTime; // �ӵ� ���� �ð�.
    float currentSpeed; // ���� �ӵ�.
    float velocityY; // ���� �ӵ� (�߷� ��).
    Vector3 moveInput; // ������� �̵� �Է�.
    Vector3 dir; // �̵� ����.

    [Header("Settings")]
    [SerializeField] float gravity = 25f; // �߷��� ũ��.
    [SerializeField] float moveSpeed = 2f; // �̵� �ӵ�.
    [SerializeField] float rotateSpeed = 3f; // ȸ�� �ӵ�.

    public bool lockMovement; // �̵� ��� ����.

    void Start()
    {
        anim = GetComponent<Animator>(); // Animator ������Ʈ�� �����ɴϴ�.
        controller = GetComponent<CharacterController>(); // CharacterController ������Ʈ�� �����ɴϴ�.
        cam = Camera.main.transform; // ���� ī�޶��� Transform�� �����ɴϴ�.
    }

    void Update()
    {
        GetInput(); // ����� �Է��� �޾ƿɴϴ�.
        PlayerMovement(); // ĳ���� �̵��� ó���մϴ�.
        if (!lockMovement) PlayerRotation(); // �̵� ����� ���� ��� ĳ���� ȸ���� ó���մϴ�.
    }

    private void GetInput()
    {
        // ����� �Է��� �޾ƿɴϴ�.
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // ī�޶��� ���� ����� ������ ������ �����ɴϴ�.
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0; // Y�� ������ �����մϴ�.
        right.y = 0; // Y�� ������ �����մϴ�.
        forward.Normalize(); // ����ȭ�Ͽ� ���⸸ �����ɴϴ�.
        right.Normalize(); // ����ȭ�Ͽ� ���⸸ �����ɴϴ�.

        // ������� �Է¿� ���� �̵� ������ ����մϴ�.
        dir = (forward * moveInput.y + right * moveInput.x).normalized;
    }

    private void PlayerMovement()
    {
        // ���� �ӵ��� �ε巴�� �����մϴ�.
        currentSpeed = Mathf.SmoothDamp(currentSpeed, moveSpeed, ref speedSmoothVelocity, speedSmoothTime * Time.deltaTime);

        // �߷¿� ���� ���� �ӵ��� �����մϴ�.
        if (velocityY > -10) velocityY -= Time.deltaTime * gravity;
        Vector3 velocity = (dir * currentSpeed) + Vector3.up * velocityY;

        // ĳ���͸� �̵���ŵ�ϴ�.
        controller.Move(velocity * Time.deltaTime);

        // �ִϸ��̼� ���¸� ������Ʈ�մϴ�.
        anim.SetFloat("Movement", dir.magnitude, 0.1f, Time.deltaTime);
        anim.SetFloat("Horizontal", moveInput.x, 0.1f, Time.deltaTime);
        anim.SetFloat("Vertical", moveInput.y, 0.1f, Time.deltaTime);
    }

    private void PlayerRotation()
    {
        // �̵� ������ ������ ȸ������ �ʽ��ϴ�.
        if (dir.magnitude == 0) return;

        // �̵� ���⿡ ���� ĳ������ ȸ���� �ε巴�� �����մϴ�.
        Vector3 rotDir = new Vector3(dir.x, dir.y, dir.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotDir), Time.deltaTime * rotateSpeed);
    }
}