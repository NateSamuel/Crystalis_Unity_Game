//Full class is student creation
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
//deals with teleportation logic for enemy when the enemy's ranged blast ability is called to teleport them further away in the room
public class EnemyTeleportation
{
    private Transform enemyTransform;
    private Transform castPoint;
    private Texture2D levelTexture;
    private Transform playerTransform;
    private GameObject teleportEffect;
    private NavMeshAgent agent;
    private Animator animator;
    private MonoBehaviour coroutineHost;
    private int scale;
    public System.Action OnTeleportFinished;
    private bool isTripleSpell = false;
    private EnemyAttack attackScript;

    public EnemyTeleportation(
        Transform enemy, 
        Transform castPoint, 
        Texture2D texture, 
        Transform player, 
        GameObject effect, 
        NavMeshAgent agent, 
        Animator animator, 
        MonoBehaviour host,
        EnemyAttack attackScript)
    {
        this.enemyTransform = enemy;
        this.castPoint = castPoint;
        this.levelTexture = texture;
        this.playerTransform = player;
        this.teleportEffect = effect;
        this.agent = agent;
        this.animator = animator;
        this.coroutineHost = host;
        this.scale = SharedLevelData.Instance.Scale;
        this.attackScript = attackScript;
    }
    //starts coroutine
    public void StartTeleport(bool spellFromBoss)
    {
        isTripleSpell = spellFromBoss;
        coroutineHost.StartCoroutine(TeleportRoutineAfterAnimation());
    }

    private IEnumerator TeleportRoutineAfterAnimation()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Standing 2H Cast Spell 01"))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.25f)
            yield return null;

        if (teleportEffect && castPoint)
        {
            GameObject effectInstance = GameObject.Instantiate(teleportEffect, castPoint.position, castPoint.rotation);
            GameObject.Destroy(effectInstance, 1f);
        }

        TeleportEnemyWithinSameRoom();

        OnTeleportFinished?.Invoke();
    }
    //Finds available position for enemy to teleport within room between distance of 7f and 20f
    public void TeleportEnemyWithinSameRoom()
    {
        Vector2 currentPixel = LevelUtility.WorldPositionToTexturePixel(enemyTransform.position, scale);
        Color currentRoomColor = levelTexture.GetPixel((int)currentPixel.x, (int)currentPixel.y);

        const int maxAttempts = 1000;

        for (int i = 0; i < maxAttempts; i++)
        {
            int x = Random.Range(0, levelTexture.width);
            int y = Random.Range(0, levelTexture.height);
            Color candidate = levelTexture.GetPixel(x, y);

            if (LevelUtility.ColorsApproximatelyEqual(candidate, currentRoomColor) && !SurroundedByBlackPixels(x, y, 2))
            {
                Vector3 worldTarget = LevelUtility.LevelPositionToWorldPosition(new Vector2(x, y), levelTexture, scale);
                float dist = Vector3.Distance(worldTarget, playerTransform.position);

                if (dist >= 7f && dist <= 20f)
                {
                    if (NavMesh.SamplePosition(worldTarget, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    {
                        if (isTripleSpell)
                        {
                            attackScript?.ShootTripleSpellAtPlayer();
                        }
                        else
                        {
                            attackScript?.ShootSpellAtPlayer();
                        }
                        enemyTransform.position = hit.position;
                        
                        return;
                    }
                }
            }
        }
    }
    //Checks if surrounded by black pixels (i.e. still within confines of room)
    private bool SurroundedByBlackPixels(int x, int y, int radius)
    {
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < levelTexture.width && ny >= 0 && ny < levelTexture.height)
                {
                    if (!LevelUtility.ColorsApproximatelyEqual(levelTexture.GetPixel(nx, ny), Color.black))
                        return false;
                }
            }
        }
        return true;
    }
}
