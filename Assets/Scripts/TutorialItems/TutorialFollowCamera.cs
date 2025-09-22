//Full class is student creation
//Minimal updates from base class
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//FollowCamera follows the player behind them in over the shoulder perspective, and tries to dodge objects
//At the start of the level it zooms from startPoint to give a better view of the overall level
//Player can rotate camera on mouse drag then it snaps back
//Zoom in and out toggle button means player camera zooms far out to see the full level
//Scroll on mouse means player can zoom in and out locally

public class TutorialFollowCamera : MonoBehaviour
{
    public static event Action OnToggleZoomOut;
    public static event Action OnToggleZoomIn;
    public Transform target;
    public float distance = 4f;
    public float height = 1.5f;
    public float positionDamping = 5f;
    public float sphereCastRadius = 0.3f;
    public float collisionBuffer = 0.2f;
    public LayerMask collisionLayers;

    [Header("Start Transition")]
    public Transform startPoint;
    public float startMoveDelay = 1f;

    [Header("Zoom Settings")]
    private Vector3 zoomStartPosition;
    private Quaternion zoomStartRotation;
    public float zoomDuration = 4f;
    private float zoomProgress = 0f;
    private bool isZooming = false;
    private bool waitingToStart = true;

    private Vector3 currentVelocity;
    private Vector3 desiredPosition;

    [Header("Mouse Orbit Control")]
    public float mouseSensitivity = 2f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    private float yaw = 0f;
    private float pitch = 20f;
    private bool isDragging = false;

    [Header("Scroll Zoom Control")]
    public float minDistance = 0f;
    public float maxDistance = 15f;
    public float zoomSpeed = 4f;

    [Header("Zoom-Out Toggle")]
    public float zoomOutDistance = 40f;
    public bool zoomOutIgnoresMax = true;
    public float zoomTransitionDuration = 0.5f;
    public Button zoomToggleButton;

    [Header("Zoom Button Sprites")]
    public Sprite zoomOutSprite;
    public Sprite zoomNormalSprite;
    private Image zoomButtonImage;

    [Header("Zoom-Out Collision Behavior")]
    public bool ignoreCollisionWhileZoomedOut = true;

    private bool isZoomedOut = false;
    private float previousDistance = 4f;
    private Coroutine zoomCoroutine = null;

    [Header("Yaw Follow")]
    public float yawFollowSpeed = 5f;
    public float backwardDeadzone = -0.1f;

    // Initializes the camera positions, zoom toggle listener, and finds the player character
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

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        previousDistance = distance;

        if (zoomToggleButton != null)
        {
            zoomToggleButton.onClick.AddListener(ToggleZoomOut);
            zoomButtonImage = zoomToggleButton.GetComponent<Image>();
            if (zoomButtonImage != null && zoomNormalSprite != null)
            {
                zoomButtonImage.sprite = zoomNormalSprite;
            }
        }


    }
    // Updates the camera's position and rotation to follow the target,
    // avoids obstacles using SphereCast, and handles the transition zoom at the start.
    void LateUpdate()
    {
        if (target == null) return;

        HandleMouseInput();
        HandleScrollZoom();

        float effectiveYaw;
        if (isDragging) effectiveYaw = yaw;
        else
        {
            effectiveYaw = target.eulerAngles.y;
            yaw = effectiveYaw;
        }

        
        if (isDragging)
        {
            effectiveYaw = yaw;
        }
        else
        {
            // detect if player is moving backwards (S / down arrow)
            float verticalInput = Input.GetAxis("Vertical");

            if (verticalInput < backwardDeadzone)
            {
                effectiveYaw = yaw;
            }
            else
            {
                effectiveYaw = target.eulerAngles.y;
                yaw = effectiveYaw;
            }
        }

        Quaternion desiredRotation = Quaternion.Euler(pitch, effectiveYaw, 0f);
        Vector3 offset = desiredRotation * new Vector3(0f, 0f, -distance);
        desiredPosition = target.position + Vector3.up * height + offset;

        // Skip the SphereCast when zoomed out and ignoreCollisionWhileZoomedOut is true.
        bool doCollisionCheck = true;
        if (isZoomedOut && ignoreCollisionWhileZoomedOut)
            doCollisionCheck = false;

        if (doCollisionCheck)
        {
            Vector3 castOrigin = target.position + Vector3.up * height;
            Vector3 directionToCamera = (desiredPosition - castOrigin).normalized;
            float maxCheckDistance = Vector3.Distance(castOrigin, desiredPosition);

            if (Physics.SphereCast(castOrigin, sphereCastRadius, directionToCamera, out RaycastHit hit, maxCheckDistance, collisionLayers))
            {
                desiredPosition = hit.point + hit.normal * collisionBuffer;
            }
        }
        // otherwise, when collision check is skipped, desiredPosition stays at the target distance

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

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / positionDamping);

        Quaternion lookAtPlayer = Quaternion.LookRotation((target.position + Vector3.up * height) - transform.position);
        transform.rotation = lookAtPlayer;
    }

    //When right mouse drags, rotation of camera begins
    void HandleMouseInput()
    {
        if (Input.GetMouseButton(1))
        {
            isDragging = true;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        else isDragging = false;
    }

    //gets scroll info as input from player mouse, and changes distance camera zooms in and out
    //deals with if camera is already zoomed out
    void HandleScrollZoom()
    {
        if (isZoomedOut) 
            return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            if (isZoomedOut)
            {
                isZoomedOut = false;
                if (zoomCoroutine != null)
                {
                    StopCoroutine(zoomCoroutine);
                    zoomCoroutine = null;
                }
            }

            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            previousDistance = distance; // update previousDistance so toggle back returns here
        }
    }

    // Toggle zoom-out button. Call from inspector.
    public void ToggleZoomOut()
    {
        SetZoomState(!isZoomedOut);
    }

    //Changes the button sprite based on when it has/not been toggled
    //Starts coroutine for zoom in/ zoom out
    private void SetZoomState(bool zoomOut)
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
            zoomCoroutine = null;
        }

        if (zoomOut)
        {
            OnToggleZoomOut?.Invoke();
            previousDistance = distance;
            float target = zoomOutIgnoresMax ? zoomOutDistance : Mathf.Clamp(zoomOutDistance, minDistance, maxDistance);
            zoomCoroutine = StartCoroutine(SmoothZoomTo(target, zoomTransitionDuration));
            isZoomedOut = true;

            if (zoomButtonImage != null && zoomOutSprite != null)
            {
                zoomButtonImage.sprite = zoomOutSprite;
            }
        }
        else
        {
            OnToggleZoomIn?.Invoke();
            float target = Mathf.Clamp(previousDistance, minDistance, maxDistance);
            zoomCoroutine = StartCoroutine(SmoothZoomTo(target, zoomTransitionDuration));
            isZoomedOut = false;

            if (zoomButtonImage != null && zoomNormalSprite != null)
            {
                zoomButtonImage.sprite = zoomNormalSprite;
            }
        }
    }

    // While the zoom animation is running isZooming is set so other start/zoom logic doesn't interfere.
    private IEnumerator SmoothZoomTo(float targetDistance, float duration)
    {
        float start = distance;
        float elapsed = 0f;

        if (duration <= 0f)
        {
            distance = targetDistance;
            yield break;
        }


        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            distance = Mathf.Lerp(start, targetDistance, t);
            yield return null;
        }

        distance = targetDistance;
        zoomCoroutine = null;
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