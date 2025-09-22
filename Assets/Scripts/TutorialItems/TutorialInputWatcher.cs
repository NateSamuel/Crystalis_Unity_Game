//Full class is student creation
using System;
using UnityEngine;

//Watches inputs from mouse away from camera to not interfer with logic. Sends info to TutoriaCurrentObjectives 
public class TutorialInputWatcher : MonoBehaviour
{
    public static event Action OnCameraDragged;
    public static event Action OnCameraScrolled;

    [Header("Sensitivity / Thresholds")]
    public float dragThreshold = 0.01f;
    public float scrollThreshold = 0.01f;

    void Update()
    {
        CheckDrag();
        CheckScroll();
    }

    void CheckDrag()
    {
        // Right mouse button drag
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(mouseX) > dragThreshold || Mathf.Abs(mouseY) > dragThreshold)
            {
                OnCameraDragged?.Invoke();
            }
        }
    }
    void CheckScroll()
    {
        //Checks mouse scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > scrollThreshold)
        {
            OnCameraScrolled?.Invoke();
        }
    }
    
}