using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using yoon;

public class Sward : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    public CapsuleCollider meleeArea;
    public int attackPower = 10;
    public int attackRange = 3;
    public float rate;
    public GameObject Arrow2;
    public Transform ArrowPos;


    public void use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }

    }

    // 코루틴 -- use + swing -- 골드메탈 유튜브
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = true;

       
        
        
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;


        yield break;
    }
    IEnumerator Shot()
    {
        GameObject intantArrow = Instantiate(Arrow2, ArrowPos.position, ArrowPos.rotation);
        Rigidbody ArrowRigid = intantArrow.GetComponent<Rigidbody>();
        ArrowRigid.velocity = ArrowPos.forward * 50;

        yield return null;

        
    }

    //부딪혔을때 상대방의 콜라이더를 인식한다
    private void OnTriggerEnter(Collider other)
    {
            print(other.gameObject.name);
            
    }
}
