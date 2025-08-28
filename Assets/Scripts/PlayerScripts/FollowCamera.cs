using UnityEngine;
using System.Collections;

//FollowCamera follows the player behind them in over the shoulder perspective, and tries to dodge objects
//At the start of the level it zooms from startPoint to give a better view of the overall level
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

    [Header("Start Transition")]
    public Transform startPoint;
    public float startMoveDelay = 2f;

    [Header("Zoom Settings")]
    private Vector3 zoomStartPosition;
    private Quaternion zoomStartRotation;
    public float zoomDuration = 4f;
    private float zoomProgress = 0f;
    private bool isZooming = false;
    private bool waitingToStart = true;

    private Vector3 currentVelocity;
    private Vector3 desiredPosition;

    // Initializes the camera positions and finds the player character
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

        waitingToStart = true;
        isZooming = false;
    }
    // Updates the camera's position and rotation to follow the target,
    // avoids obstacles using SphereCast, and handles the transition zoom at the start.
    void LateUpdate()
    {
        if (target == null) return;

        Quaternion desiredRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
        Vector3 offset = desiredRotation * new Vector3(0, 0, -distance);
        desiredPosition = target.position + Vector3.up * height + offset;

        Vector3 castOrigin = target.position + Vector3.up * height;
        Vector3 directionToCamera = (desiredPosition - castOrigin).normalized;
        float maxDistance = Vector3.Distance(castOrigin, desiredPosition);

        if (Physics.SphereCast(castOrigin, sphereCastRadius, directionToCamera, out RaycastHit hit, maxDistance, collisionLayers))
        {
            desiredPosition = hit.point + hit.normal * collisionBuffer;
        }

        if (waitingToStart)
        {
            Quaternion targetRotation = Quaternion.LookRotation((target.position + Vector3.up * height) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationDamping);
            return;
        }

        if (isZooming)
        {
            zoomProgress += Time.deltaTime / zoomDuration;
            float t = Mathf.SmoothStep(0, 1, zoomProgress);

            transform.position = Vector3.Lerp(zoomStartPosition, desiredPosition, t);

            Quaternion targetRotation = Quaternion.LookRotation((target.position + Vector3.up * height) - transform.position);
            transform.rotation = Quaternion.Slerp(zoomStartRotation, targetRotation, t);

            if (zoomProgress >= 1f)
            {
                isZooming = false;
            }

            return;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / positionDamping);

            Quaternion targetRotation = Quaternion.LookRotation((target.position + Vector3.up * height) - transform.position);
            transform.rotation = targetRotation;
        }
    }
    // Starts the coroutine for WaitThenStartFollow
    public void StartFollowingAfterDelay()
    {
        if (!waitingToStart) return;
        StartCoroutine(WaitThenStartFollow());
    }
    // Waits for a delay, then does the zoom transition from startPoint to the follow position.
    private IEnumerator WaitThenStartFollow()
    {
        yield return new WaitForSeconds(startMoveDelay);

        zoomStartPosition = transform.position;
        zoomStartRotation = transform.rotation;

        zoomProgress = 0f;
        isZooming = true;
        waitingToStart = false;
    }
    // Resets the camera to the startPoint position and rotation.
    public void ResetCameraToStart()
    {
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
            waitingToStart = true;
            isZooming = false;
        }
    }
}