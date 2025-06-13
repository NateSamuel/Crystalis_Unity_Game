using UnityEngine;

public class CollectTreasure : MonoBehaviour
{
    public Transform playerTransform;
    Vector3 spawnPosition;
    public float collectionRange = 3f;
    private CharacterTreasure charTreasureScript;

    
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure Player GameObject is tagged 'Player'.");
        }

        spawnPosition = transform.position;
    }

    
    void Update()
    {
        if (Vector3.Distance(spawnPosition, playerTransform.position) < collectionRange)
        {
            PlayerCanCollect();
        }
    }
    void PlayerCanCollect()
    {
        charTreasureScript?.ApplyTreasure();
        Destroy(gameObject);
    }
}
