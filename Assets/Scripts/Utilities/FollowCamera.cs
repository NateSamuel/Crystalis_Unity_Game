using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;           
    public float distance = 4f;        
    public float height = 1.5f;        
    public float rotationDamping = 5f; 
    public float positionDamping = 5f; 

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogError("No player found with tag 'Player'.");
        }
    }
    private void LateUpdate()
    {
        if (target == null) return;

        
        Quaternion desiredRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
        
        
        Vector3 offset = desiredRotation * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = target.position + Vector3.up * height + offset;

        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionDamping);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamping);

        
        transform.LookAt(target.position + Vector3.up * height);
    }
}