using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 40;
    public GameObject rockPrefab;
    public Transform handPos;

    public void KickOut()
    {
        if (attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            //反向击飞  
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //击飞后眩晕动画
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
           // attackTarget.transform.LookAt(this.transform);
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
    public void ThrowRock()
    {
        if (attackTarget!=null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;

        }
    }

    
}
