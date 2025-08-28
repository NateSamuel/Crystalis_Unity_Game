using UnityEngine;

public class FreePrisoners : MonoBehaviour
{
    private CharacterKeys charKeyScript;
    private MainScreenManager mainUI;
    private TotalPrisoners totalPrisoners;
    private bool prisonerFreed = false;


    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            charKeyScript = playerObject.GetComponent<CharacterKeys>();
        }

        mainUI = FindAnyObjectByType<MainScreenManager>();
        totalPrisoners = FindAnyObjectByType<TotalPrisoners>();
        mainUI?.HideFreePrisonerUI();
    }
    
    private void OnEnable()
    {
        prisonerFreed = false;
        mainUI?.HideFreePrisonerUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!prisonerFreed && other.CompareTag("Player") && charKeyScript != null && charKeyScript.keys > 0)
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
            charKeyScript.RemoveKey(1);
            totalPrisoners?.RemovePrisoner(1);
            prisonerFreed = true;

            mainUI?.HideFreePrisonerUI();

            transform.position = new Vector3(0, -1000, 0);

            gameObject.SetActive(false);
        }
    }
}