using UnityEngine;
using yoon;

public class ParticleDamage : MonoBehaviour
{
    public int damageAmount = 10; // 플레이어에게 줄 데미지


    public Boss boss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && boss.isAttackTrue)
        {
            PlayerContorler player = other.GetComponent<PlayerContorler>();
            player.GetDamage(damageAmount);
            player.playerHit = true;
            boss.isAttackTrue = false;
            print("불 맞음");
        }
    }


}
