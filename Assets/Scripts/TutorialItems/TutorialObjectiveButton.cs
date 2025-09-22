//Full class is student creation
using UnityEngine;
using UnityEngine.UI;
//Stores info on the room and objective index for that specific button for easier button objectives
public class TutorialObjectiveButton : MonoBehaviour
{
    public Button button;
    public TutorialCurrentObjectives tutorial;
    public int room = 1;
    public int objectiveIndex = 4;

    void Start()
    {
        button.onClick.AddListener(() => 
            tutorial.CompleteObjective(room, objectiveIndex));
    }
}