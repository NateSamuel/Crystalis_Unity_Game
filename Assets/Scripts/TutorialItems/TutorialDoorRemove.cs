using UnityEngine;
using System;

public class TutorialDoorRemove : MonoBehaviour
{
    private Transform playerTransform;
    private MainScreenManager mainUI;
    private bool doorRemoved = false;

    private bool doorAbleToBeRemoved = false;

    public event Action OnDoorRemoved;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject != null ? playerObject.transform : null;
        mainUI = FindAnyObjectByType<MainScreenManager>();
        mainUI?.HideDoorOpenUI();
    }

    private void OnEnable()
    {
        doorRemoved = false;
        mainUI?.HideDoorOpenUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!doorRemoved && other.CompareTag("Player") && doorAbleToBeRemoved)
        {
            mainUI?.ShowDoorOpenUI(PlayerDestroysDoor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!doorRemoved && other.CompareTag("Player"))
        {
            mainUI?.HideDoorOpenUI();
        }
    }

    public void SetDoorAbleToBeRemoved(bool value)
    {
        doorAbleToBeRemoved = value;
    }

    private void PlayerDestroysDoor()
    {
        if (doorRemoved) return;

        mainUI?.HideDoorOpenUI();
        doorRemoved = true;

        transform.position = new Vector3(0, -1000, 0);
        gameObject.SetActive(false);

        OnDoorRemoved?.Invoke();
    }
}