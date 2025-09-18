using System;
using UnityEngine;

public class TutorialCollectTreasure : MonoBehaviour
{
    public static event Action OnTreasureCollect;
    public Transform playerTransform;
    Vector3 spawnPosition;
    public float collectionRange = 3f;
    private CharacterTreasure charTreasureScript;
    public int treasureAmount = 6;
    private TutorialMainScreenManager mainUI;

    private bool treasureCollected = false;

    private TutorialCurrentObjectives objectivesManager;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        }

        spawnPosition = transform.position;

        mainUI = FindAnyObjectByType<TutorialMainScreenManager>();

        if (mainUI != null)
        {
            mainUI.HideCollectTreasureUI();
        }

        treasureCollected = false;
        objectivesManager = FindAnyObjectByType<TutorialCurrentObjectives>();
    }

    void Update()
    {
        if (treasureCollected) return;
        if (playerTransform == null) return;

        if (mainUI != null && mainUI.MainUIPanel != null && mainUI.MainUIPanel.activeSelf == false)
        {
            return;
        }

        float dist = Vector3.Distance(spawnPosition, playerTransform.position);
        if (dist < collectionRange)
        {
            if (objectivesManager != null && objectivesManager.IsObjectiveAvailable(3, 2))
            {
                mainUI?.ShowCollectTreasureUI(PlayerCollectsTreasure);
            }
        }
            else
            {
                mainUI?.HideCollectTreasureUI();
            }
    }

    private void PlayerCollectsTreasure()
    {
        if (treasureCollected) return;
        treasureCollected = true;

        mainUI?.HideCollectTreasureUI();

        charTreasureScript?.ApplyTreasure(treasureAmount);
        Destroy(gameObject);
        OnTreasureCollect?.Invoke();

    }
}
