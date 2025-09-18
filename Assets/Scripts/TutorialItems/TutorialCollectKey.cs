using System;
using UnityEngine;

public class TutorialCollectKey : MonoBehaviour
{
    public static event Action OnKeyCollect;
    private CharacterKeys charKeyScript;
    private int keyAmount = 1;
    private bool keyIsPickedUp = false;
    private MainScreenManager mainUI;
    private TutorialCurrentObjectives objectivesManager;

    // Find the player with key script and MainScreenManager
    void Start()
    {

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            charKeyScript = playerObject.GetComponent<CharacterKeys>();
        }

        mainUI = FindAnyObjectByType<MainScreenManager>();
        mainUI?.HideCollectKeyUI();
        objectivesManager = FindAnyObjectByType<TutorialCurrentObjectives>();
    }

    // Player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (!keyIsPickedUp && other.CompareTag("Player"))
        {
            if (objectivesManager != null && objectivesManager.IsObjectiveAvailable(2, 3))
            {
                mainUI?.ShowCollectKeyUI(PlayerCollectedKey);
            }
        }
    }

    // Player leaves the trigger
    private void OnTriggerExit(Collider other)
    {
        if (!keyIsPickedUp && other.CompareTag("Player"))
        {
            mainUI?.HideCollectKeyUI();
        }
    }

    // Called when player presses the button
    private void PlayerCollectedKey()
    {
        if (keyIsPickedUp) return;

        keyIsPickedUp = true;
        charKeyScript?.ApplyKey(keyAmount);
        mainUI?.HideCollectKeyUI();
        Destroy(gameObject);
        OnKeyCollect?.Invoke();
    }
}