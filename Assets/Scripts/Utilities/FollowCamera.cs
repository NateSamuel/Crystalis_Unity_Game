using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 4f;
    public float height = 1.5f;
    public float rotationDamping = 20f;
    public float positionDamping = 5f;
    public float sphereCastRadius = 0.3f;
    public float collisionBuffer = 0.2f;
    public LayerMask collisionLayers;

    public Transform startPoint;          // Camera start position & rotation
    public float startMoveDuration = 2f;  // How long to zoom in at level start

    private Vector3 currentVelocity;
    private float startMoveTimer = 0f;
    private bool isStarting = true;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogError("No player found with tag 'Player'.");
        }

        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }
        else
        {
            isStarting = false; // no start point assigned — skip zoom in
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired camera position behind the player
        Quaternion desiredRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
        Vector3 offset = desiredRotation * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = target.position + Vector3.up * height + offset;

        // Collision check with SphereCast
        Vector3 castOrigin = target.position + Vector3.up * height;
        Vector3 directionToCamera = (desiredPosition - castOrigin).normalized;
        float maxDistance = Vector3.Distance(castOrigin, desiredPosition);

        if (Physics.SphereCast(castOrigin, sphereCastRadius, directionToCamera, out RaycastHit hit, maxDistance, collisionLayers))
        {
            desiredPosition = hit.point + hit.normal * collisionBuffer;
        }

        if (isStarting)
        {
            startMoveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(startMoveTimer / startMoveDuration);

            // Smoothly move camera from startPoint to desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 0.1f);

            // Smoothly rotate towards player
            Quaternion targetRotation = Quaternion.LookRotation((target.position + Vector3.up * height) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            if (t >= 1f)
            {
                isStarting = false; // done zooming in
            }
        }
        else
        {
            // Normal follow behavior
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / positionDamping);

            Quaternion targetRotation = Quaternion.LookRotation((target.position + Vector3.up * height) - transform.position);
            transform.rotation = targetRotation; // instant rotation, no delay
        }
    }
}