using UnityEngine;

public class CollectTreasure : MonoBehaviour
{
    public Transform playerTransform;
    Vector3 spawnPosition;
    public float collectionRange = 3f;
    private CharacterTreasure charTreasureScript;
    public int treasureAmount = 6;
    private MainScreenManager mainUI;

    private bool treasureCollected = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        }

        spawnPosition = transform.position;

        mainUI = FindAnyObjectByType<MainScreenManager>();

        if (mainUI != null)
        {
            mainUI.HideCollectTreasureUI();
        }

        treasureCollected = false;
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
            mainUI?.ShowCollectTreasureUI(PlayerCollectsTreasure);
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
    }
}