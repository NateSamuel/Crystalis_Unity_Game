//Full class is student creation
using UnityEngine;

//collects treasure from treasure chest if character is within collider trigger zone
public class CollectTreasure : MonoBehaviour
{
    private CharacterTreasure charTreasureScript;
    public int treasureAmount = 6;
    private MainScreenManager mainUI;
    private bool treasureCollected = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            charTreasureScript = playerObject.GetComponent<CharacterTreasure>();
        }

        mainUI = FindAnyObjectByType<MainScreenManager>();
        mainUI?.HideCollectTreasureUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!treasureCollected && other.CompareTag("Player"))
        {
            mainUI?.ShowCollectTreasureUI(PlayerCollectsTreasure);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!treasureCollected && other.CompareTag("Player"))
        {
            mainUI?.HideCollectTreasureUI();
        }
    }

    private void PlayerCollectsTreasure()
    {
        if (treasureCollected) return;

        treasureCollected = true;
        charTreasureScript?.ApplyTreasure(treasureAmount);
        mainUI?.HideCollectTreasureUI();
        Destroy(gameObject);
    }
}