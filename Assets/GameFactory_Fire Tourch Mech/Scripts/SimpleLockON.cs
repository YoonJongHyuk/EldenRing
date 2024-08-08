using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLockON : MonoBehaviour
{
    [SerializeField] Transform target;
    void OnEnable()
    {
        if (target == null) target = Camera.main.transform;
        StartCoroutine(LookAtTarget());
    }

    private IEnumerator LookAtTarget()
    {
        while(this.gameObject.activeInHierarchy)
        {
            Vector3 dir = target.position - transform.position;
            // dir.y =0;
            transform.rotation = Quaternion.LookRotation(dir);
            yield return null;
        }
    }
}
