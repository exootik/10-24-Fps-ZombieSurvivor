using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Attack : MonoBehaviour
{
    private static readonly int IsAlive = Animator.StringToHash("IsAlive");
    private static readonly int Move1 = Animator.StringToHash("move");
    private static readonly int Attack1 = Animator.StringToHash("Attack");
    public float AttackTime;
    public float AttackSpeed;
    public int damage;
    private IDamageable _damageable;
    private NavMeshAgent agent;
    private Animator anim;
    private float attacklength;
    private float currentAttackSpeedTime;
    private float currentAttackTime;

    private GameObject Target;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        Target = GameObject.FindWithTag("Player");
        _damageable = Target.GetComponent<IDamageable>();
        foreach (var VARIABLE in anim.runtimeAnimatorController.animationClips)
            if (VARIABLE.name == "Punch2")
                attacklength = VARIABLE.length;

        currentAttackSpeedTime = attacklength;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!anim.GetBool(IsAlive)) return;
        if (Close())
        {
            Atak();
        }
        else
        {
            currentAttackSpeedTime = attacklength;
            currentAttackTime = 0;
        }
    }

    private bool Close()
    {
        var distance = (new Vector3(agent.transform.position.x, 0, agent.transform.position.z) -
                        new Vector3(Target.transform.position.x, 0, Target.transform.position.z)).magnitude;
        var height = math.abs(agent.transform.position.y - Target.transform.position.y);

        return distance < 1.5 && height < 3;
    }

    private void Atak()
    {
        currentAttackTime += Time.smoothDeltaTime;
        if (currentAttackTime >= AttackTime)
        {
            currentAttackSpeedTime += Time.smoothDeltaTime;
            if (currentAttackSpeedTime >= AttackSpeed + attacklength)
            {
                anim.SetTrigger(Attack1);
                currentAttackSpeedTime = 0;
            }
        }
    }

    private void DealDamage()
    {
        _damageable?.TakeDamage(damage);
    }
}