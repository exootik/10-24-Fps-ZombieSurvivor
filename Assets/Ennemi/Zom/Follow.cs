using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Follow : MonoBehaviour
{
    private static readonly int IsAlive = Animator.StringToHash("IsAlive");
    public Transform target;
    private NavMeshAgent agent;
    private Animator anim;
    private Vector3 destination;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        destination = agent.destination;

        TryGetComponent(out anim);
    }

    private void Update()
    {
        //if target move more than X unit change its destination
        //lower => more precise and expensive
        if (anim && !anim.GetBool(IsAlive))
        {
            agent.destination = transform.position;
            return;
        }

        if (Vector3.Distance(destination, target.position) > 1.0f)
        {
            destination = target.position;
            agent.destination = destination;
        }
    }
}