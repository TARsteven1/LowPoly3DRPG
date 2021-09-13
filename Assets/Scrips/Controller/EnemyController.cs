using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyStates { GUARD,PATROL,CHASE,DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;

    protected CharacterStats characterStats;

    [Header("Basic Setting")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    protected GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;

    private float lastAttackTime;

    private Quaternion guardRotation;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;

    private Vector3 guardPos;

    private Animator anim;
    private Collider coll;

    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;

    bool playerDeath;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }
    void SwitchStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;

        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position!=guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    //SqrMagnitude据说比Distancede消耗性能更低，此处也可使用Distance实现
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation =Quaternion.Lerp(transform.rotation, guardRotation,0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                //判断是否到了随机巡逻点
                if (Vector3.Distance(wayPoint,transform.position)<=agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime>0)  remainLookAtTime -= Time.deltaTime;                  
                    else GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
               
                break;
            case EnemyStates.CHASE:

                //TODO:追击player
               
                //TODO:配合动画

                isWalk = false;
                isChase = true;

                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //拉脱回到上一个状态
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {                       
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard)enemyStates = EnemyStates.GUARD;
                    
                    else enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                //TODO:在攻击范围内攻击
                if (TargetInAttackRange()||TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime<0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击\
                        Attck();
                    }
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
            default:
                break;
        }
    }
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        }
        else return false;
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        }
        else return false;
    }
    void Attck()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
           
        }
    }
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        wayPoint = randomPoint;
        NavMeshHit hit;
        wayPoint= NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, sightRadius);
    }
    void SwitchAnimaton()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Die", isDead);
    }
    //Animation Event
    void Hit()
    {
        if (attackTarget!=null&&transform.isFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        //FIXME:场景切换后修改掉
        GameManager.Instance.AddObserver(this);
    }
    //void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);
    //}
    void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        
            GameManager.Instance.RemoveObserver(this);
        
      
    }
    // Update is called once per frame
    void Update()
    {
        if (characterStats.CurrentHealth<=0)          isDead = true;
        if (!playerDeath)
        {
            SwitchStates();
            SwitchAnimaton();
            lastAttackTime -= Time.deltaTime;
        }       
    }
    public void EndNotify()
    {
        anim.SetBool("Win", true);
        isChase = false;
        isWalk = false;
        attackTarget = null;
        playerDeath = true;

    }
}
