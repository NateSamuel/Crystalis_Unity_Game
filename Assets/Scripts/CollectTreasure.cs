using UnityEngine;

public class CollectTreasure : MonoBehaviour
{
    public Transform playerTransform;
    Vector3 spawnPosition;
    public float collectionRange = 3f;
    private CharacterTreasure charTreasureScript;
    public int treasureAmount = 6;

    
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            charTreasureScript = playerTransform.GetComponent<CharacterTreasure>();
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
        charTreasureScript?.ApplyTreasure(treasureAmount);
        Destroy(gameObject);
    }
}
