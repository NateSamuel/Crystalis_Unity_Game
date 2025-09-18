using UnityEngine;
using UnityEngine.AI;

public class DirectedAgent : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    public float moveSpeed = 3.5f;
    public float rotationSpeed = 400f;
    public bool canMove = true;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updatePosition = false;
    }

    void Update()
    {
        if (!canMove) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, horizontal * rotationSpeed * Time.deltaTime);

        Vector3 direction = transform.forward * vertical;
        Vector3 movement = direction * moveSpeed * Time.deltaTime;

        agent.Move(movement);
        transform.position = agent.nextPosition;

        float inputMagnitude = new Vector2(horizontal, vertical).magnitude;
        animator.SetFloat("forwardSpeed", inputMagnitude);
    }
}