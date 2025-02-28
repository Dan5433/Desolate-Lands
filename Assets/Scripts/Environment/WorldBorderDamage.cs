using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBorderDamage : MonoBehaviour
{
    [SerializeField] float minDamageAmount;
    [SerializeField] float maxDamageAmount;
    LinkedList<GameObject> insideTrigger = new();

    private void Start()
    {
        StartCoroutine(DealDamage());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        insideTrigger.AddLast(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        insideTrigger.Remove(collision.gameObject);
    }
    
    IEnumerator DealDamage()
    {
        while (!DeathMenu.IsDead)
        {
            foreach (GameObject obj in insideTrigger)
            {
                if (!obj.TryGetComponent<LivingBase>(out var living) &&
                    !obj.transform.parent.TryGetComponent(out living))
                    continue;

                var distanceToBorder = Mathf.Abs(transform.rotation.eulerAngles.z) == 90
                    ? transform.position.x - obj.transform.position.x
                    : transform.position.y - obj.transform.position.y;

                var proximityToBorder = 
                    (WorldBorderManager.Instance.EffectRange - distanceToBorder) / 
                    WorldBorderManager.Instance.EffectRange;

                living.Damage(
                    Mathf.Lerp(minDamageAmount,maxDamageAmount, proximityToBorder));
            }
            yield return new WaitForSeconds(1);
        }
    }
}
