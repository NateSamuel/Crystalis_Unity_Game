//Full class is student creation
using UnityEngine;
//This is for the enemies health sliders so that the player can always see them
public class SliderFaceCamera : MonoBehaviour
{
    void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
