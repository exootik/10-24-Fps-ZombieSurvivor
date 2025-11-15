using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Move : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private bool IsBusy;
    private Vector2 smoothDeltaPosition = Vector2.zero;
    private Vector2 velocity = Vector2.zero;

    private void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Don’t update position automatically
        agent.updatePosition = false;
    }

    private void Update()
    {
        var worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        var dx = Vector3.Dot(transform.right, worldDeltaPosition);
        var dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        var deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        var smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;
        velocity.Normalize();
        var shouldMove = velocity.magnitude > 0.5f &&
                         agent.remainingDistance > agent.radius + agent.stoppingDistance / 2;

        // Update animation parameters
        anim.SetBool("move", shouldMove);
        if (shouldMove)
        {
            anim.SetFloat("velx", velocity.x);
            anim.SetFloat("vely", velocity.y);
        }

        GetComponent<LookAt>().lookAtTargetPosition = agent.steeringTarget + transform.forward;

        if (worldDeltaPosition.magnitude > agent.radius)
            agent.nextPosition = transform.position + 0.9f * worldDeltaPosition;
    }

    private void OnAnimatorMove()
    {
        // Update position to agent position
        if (!IsBusy)
            transform.position = agent.nextPosition;
    }

    public void BecomeBusy()
    {
        IsBusy = true;
    }

    public void BecomeNotBusy()
    {
        IsBusy = false;
    }
}