using System;
using UnityEngine;

public class TutorialFreePrisoners : MonoBehaviour
{
    public static event Action OnFreePrisoner;
    private Transform playerTransform;
    private CharacterKeys charKeyScript;
    private MainScreenManager mainUI;
    private TotalPrisoners totalPrisoners;
    private bool prisonerFreed = false;
    private CharacterTreasure charTreasureScript;


    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;

        if (playerObject != null)
        {
            charKeyScript = playerObject.GetComponent<CharacterKeys>();
        }

        mainUI = FindAnyObjectByType<MainScreenManager>();
        totalPrisoners = FindAnyObjectByType<TotalPrisoners>();
        mainUI?.HideFreePrisonerUI();
        charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
    }
    
    private void OnEnable()
    {
        prisonerFreed = false;
        mainUI?.HideFreePrisonerUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!prisonerFreed && other.CompareTag("Player") && charKeyScript != null && charKeyScript.keys > 0 && totalPrisoners?.prisonersCount > 0)
        {
            mainUI?.ShowFreePrisonerUI(PlayerFreesPrisoner);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!prisonerFreed && other.CompareTag("Player"))
        {
            mainUI?.HideFreePrisonerUI();
        }
    }

    private void PlayerFreesPrisoner()
    {
        if (prisonerFreed) return;

        if (charKeyScript != null && charKeyScript.keys > 0)
        {
            OnFreePrisoner?.Invoke();
            charKeyScript.RemoveKey(1);
            totalPrisoners?.RemovePrisoner(1);
            prisonerFreed = true;

            mainUI?.HideFreePrisonerUI();

            transform.position = new Vector3(0, -1000, 0);

            gameObject.SetActive(false);
            charTreasureScript?.ApplyTreasure(5);
        }
    }
}