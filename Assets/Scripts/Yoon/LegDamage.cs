using UnityEngine;
using yoon;

public class LegDamage : MonoBehaviour
{
    public int damageAmount = 20; // �÷��̾�� �� ������

    public Boss boss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && boss.isAttackTrue)
        {
            PlayerContorler player = other.GetComponent<PlayerContorler>();
            player.GetDamage(damageAmount);
            player.playerHit = true;
            boss.isAttackTrue = false;
            print("�ٸ� ����");
        }
    }
}
