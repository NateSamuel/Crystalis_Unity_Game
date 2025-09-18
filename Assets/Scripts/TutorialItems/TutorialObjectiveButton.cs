using UnityEngine;
using UnityEngine.UI;

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