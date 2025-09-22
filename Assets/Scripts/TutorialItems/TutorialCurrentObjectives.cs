//Full class is student creation

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class Objective
{
    public string description;
    public bool completed;

    public Objective(string description, bool completed = false)
    {
        this.description = description;
        this.completed = completed;
    }
}

public class TutorialCurrentObjectives : MonoBehaviour
{
    public TextMeshProUGUI tutorialObjectiveText;


    // Each room has own list of objectives
    public List<Objective> roomOneObjectives = new List<Objective>();
    public List<Objective> roomTwoObjectives = new List<Objective>();
    public List<Objective> roomThreeObjectives = new List<Objective>();
    public List<Objective> roomFourObjectives = new List<Objective>();
    public List<Objective> roomFiveObjectives = new List<Objective>();

    // UI GameObjects for each objective in Rooms
    [Header("Room objective UI")]
    public List<GameObject> roomOneObjectiveUI = new List<GameObject>();
    public List<GameObject> roomTwoObjectiveUI = new List<GameObject>();
    public List<GameObject> roomThreeObjectiveUI = new List<GameObject>();
    public List<GameObject> roomFourObjectiveUI = new List<GameObject>();

    [Header("Room Final UI")]
    public GameObject roomOneFinalUI;
    public GameObject roomTwoFinalUI;
    public GameObject roomThreeFinalUI;
    public GameObject roomFourFinalUI;

    // Doors
    public GameObject doorOne;
    public GameObject doorTwo;
    public GameObject doorThree;
    public GameObject doorFour;

    //flags for if room is complete
    private bool allRoomOneComplete = false;
    private bool allRoomTwoComplete = false;
    private bool allRoomThreeComplete = false;
    private bool allRoomFourComplete = false;

    //flags for if door is opened
    private bool roomOneDoorOpened = false;
    private bool roomTwoDoorOpened = false;
    private bool roomThreeDoorOpened = false;
    private bool roomFourDoorOpened = false;


    // index of currently shown UI in roomObjectiveUI's
    private int roomOneCurrentUIIndex = 0;
    private int roomTwoCurrentUIIndex = 0;
    private int roomThreeCurrentUIIndex = 0;
    private int roomFourCurrentUIIndex = 0;

    void Start()
    {
        // Adding objectives for rooms
        roomOneObjectives.Add(new Objective("Use arrow keys/wasd to move around"));
        roomOneObjectives.Add(new Objective("Drag right mouse button to rotate view"));
        roomOneObjectives.Add(new Objective("Scroll on mouse to zoom in and out"));
        roomOneObjectives.Add(new Objective("Click objectives button to find objectives"));

        roomTwoObjectives.Add(new Objective("Use 'Punch' button"));
        roomTwoObjectives.Add(new Objective("Use 'Crystal Spiral' Button"));
        roomTwoObjectives.Add(new Objective("Use 'Heal' Button"));
        roomTwoObjectives.Add(new Objective("Pick Up Key"));
        roomTwoObjectives.Add(new Objective("Free Prisoner"));

        roomThreeObjectives.Add(new Objective("Use 'Force field' button"));
        roomThreeObjectives.Add(new Objective("Use 'Power-Up' button with 'Punch' button"));
        roomThreeObjectives.Add(new Objective("Collect Treasure"));

        roomFourObjectives.Add(new Objective("Use 'Freeze' button with 'Ranged Blast' button"));
        roomFourObjectives.Add(new Objective("Use 'Crystal stab' button"));
        roomFourObjectives.Add(new Objective("Press world button to zoom out"));
        roomFourObjectives.Add(new Objective("Press world button to zoom in"));
        roomFourObjectives.Add(new Objective("Click buy abilities button"));
        roomFourObjectives.Add(new Objective("Click level up abilities button"));

        roomFiveObjectives.Add(new Objective("Exit Tutorial"));

        // Updates Objective UI etc based on if door is removed
        // Room 1
        if (doorOne != null)
        {
            var doorScript = doorOne.GetComponent<TutorialDoorRemove>();
            if (doorScript != null)
            {
                doorScript.OnDoorRemoved += () =>
                {
                    roomOneDoorOpened = true;
                    if (roomOneFinalUI != null)
                        roomOneFinalUI.SetActive(false);
                        UpdateTutorialObjectiveText(roomTwoObjectives);
                        InitFirstObjectiveUI(roomTwoObjectiveUI, roomTwoObjectives, roomTwoCurrentUIIndex);
                };
            }
        }

        // Room 2
        if (doorTwo != null)
        {
            var doorScript = doorTwo.GetComponent<TutorialDoorRemove>();
            if (doorScript != null)
            {
                doorScript.OnDoorRemoved += () =>
                {
                    roomTwoDoorOpened = true;
                    if (roomTwoFinalUI != null)
                        roomTwoFinalUI.SetActive(false);
                        UpdateTutorialObjectiveText(roomThreeObjectives);
                        InitFirstObjectiveUI(roomThreeObjectiveUI, roomThreeObjectives, roomThreeCurrentUIIndex);
                };
            }
        }

        // Room 3
        if (doorThree != null)
        {
            var doorScript = doorThree.GetComponent<TutorialDoorRemove>();
            if (doorScript != null)
            {
                doorScript.OnDoorRemoved += () =>
                {
                    roomThreeDoorOpened = true;
                    if (roomThreeFinalUI != null)
                        roomThreeFinalUI.SetActive(false);
                        UpdateTutorialObjectiveText(roomFourObjectives);
                        InitFirstObjectiveUI(roomFourObjectiveUI, roomFourObjectives, roomFourCurrentUIIndex);
                };
            }
        }

        // Room 4
        if (doorFour != null)
        {
            var doorScript = doorFour.GetComponent<TutorialDoorRemove>();
            if (doorScript != null)
            {
                doorScript.OnDoorRemoved += () =>
                {
                    roomFourDoorOpened = true;
                    if (roomFourFinalUI != null)
                        roomFourFinalUI.SetActive(false);
                };
            }
        }
        // initialize room-one UI first objective and objectives for room one
        InitFirstObjectiveUI(roomOneObjectiveUI, roomOneObjectives, roomOneCurrentUIIndex);

        UpdateTutorialObjectiveText(roomOneObjectives);
    }
    
    // method to initialize UI first objective for any room
    private void InitFirstObjectiveUI(List<GameObject> roomObjectiveUI, List<Objective> objectiveList, int roomCurrentUIIndex)
    {
        // hide all room UI elements if any
        for (int i = 0; i < roomObjectiveUI.Count; i++)
        {
            var go = roomObjectiveUI[i];
            if (go != null)
                go.SetActive(false);
        }

        roomCurrentUIIndex = 0;

        if (roomObjectiveUI.Count > 0)
        {
            if (!objectiveList[0].completed)
                roomObjectiveUI[0]?.SetActive(true);
            else
            {
                ShowNextRoomUIFromIndex(roomObjectiveUI, objectiveList, 0, roomCurrentUIIndex);
            }
        }
    }

    // helper to show next available UI following first objective completion
    private void ShowNextRoomUIFromIndex(List<GameObject> roomObjectiveUI, List<Objective> objectiveList, int startIndex, int roomCurrentUIIndex)
    {
        int idx = startIndex;
        while (idx < objectiveList.Count && idx < roomObjectiveUI.Count && objectiveList[idx].completed)
            idx++;

        roomCurrentUIIndex = idx;
        if (roomCurrentUIIndex >= 0 && roomCurrentUIIndex < roomObjectiveUI.Count)
            roomObjectiveUI[roomCurrentUIIndex]?.SetActive(true);
    }
    //enables and disables all invoked items
    void OnEnable()
    {
        TutorialDirectedAgent.OnPlayerMoved += OnPlayerMoved;
        TutorialInputWatcher.OnCameraDragged += OnCameraDragged;
        TutorialInputWatcher.OnCameraScrolled += OnCameraScrolled;
        TutorialCharacterAttack.OnPunchHit += OnPunchHit;
        TutorialCharacterAttack.OnAOEHit += OnAOEHit;
        TutorialHealthBoost.OnHeal += OnHeal;
        TutorialCollectKey.OnKeyCollect += OnKeyCollect;
        TutorialFreePrisoners.OnFreePrisoner += OnFreePrisoner;
        TutorialCharacterAttack.OnPowerUpWithPunch += OnPowerUpWithPunch;
        TutorialCollectTreasure.OnTreasureCollect += OnTreasureCollect;
        TutorialCharacterAttack.OnFreezeWithRangedBlast += OnFreezeWithRangedBlast;
        TutorialCharacterAttack.OnCrystalStab += OnCrystalStab;
        TutorialFollowCamera.OnToggleZoomOut += OnToggleZoomOut;
        TutorialFollowCamera.OnToggleZoomIn += OnToggleZoomIn;
    }

    void OnDisable()
    {
        TutorialDirectedAgent.OnPlayerMoved -= OnPlayerMoved;
        TutorialInputWatcher.OnCameraDragged -= OnCameraDragged;
        TutorialInputWatcher.OnCameraScrolled -= OnCameraScrolled;
        TutorialCharacterAttack.OnPunchHit -= OnPunchHit;
        TutorialCharacterAttack.OnAOEHit -= OnAOEHit;
        TutorialHealthBoost.OnHeal -= OnHeal;
        TutorialCollectKey.OnKeyCollect -= OnKeyCollect;
        TutorialFreePrisoners.OnFreePrisoner -= OnFreePrisoner;
        TutorialCharacterAttack.OnPowerUpWithPunch -= OnPowerUpWithPunch;
        TutorialCollectTreasure.OnTreasureCollect -= OnTreasureCollect;
        TutorialCharacterAttack.OnFreezeWithRangedBlast -= OnFreezeWithRangedBlast;
        TutorialCharacterAttack.OnCrystalStab -= OnCrystalStab;
        TutorialFollowCamera.OnToggleZoomOut -= OnToggleZoomOut;
        TutorialFollowCamera.OnToggleZoomIn -= OnToggleZoomIn;
    }
    //Orders the objective so that it cannot be completed without previous objective being completed, then shows next UI
    private void CompleteObjectiveOrdered(
        int index,
        List<Objective> objectiveList,
        List<GameObject> roomObjectiveUI,
        ref int roomCurrentUIIndex)
    {
        if (objectiveList == null || index >= objectiveList.Count)
            return;

        if (index != roomCurrentUIIndex)
            return;

        if (objectiveList[index].completed)
            return;

        objectiveList[index].completed = true;

        if (roomObjectiveUI != null && roomObjectiveUI.Count > index)
            roomObjectiveUI[index]?.SetActive(false);

        roomCurrentUIIndex++;

        if (roomObjectiveUI != null && roomCurrentUIIndex < roomObjectiveUI.Count)
            roomObjectiveUI[roomCurrentUIIndex]?.SetActive(true);

        UpdateTutorialObjectiveText(objectiveList);
        CheckFullRoomObjectives();
    }

    //Used if objective is just a simple UI button click, calls CompleteObjectiveOrdered
    public void CompleteObjective(int room, int objectiveIndex)
    {
        switch (room)
        {
            case 1:
                CompleteObjectiveOrdered(objectiveIndex, roomOneObjectives, roomOneObjectiveUI, ref roomOneCurrentUIIndex);
                break;

            case 2:
                CompleteObjectiveOrdered(objectiveIndex, roomTwoObjectives, roomTwoObjectiveUI, ref roomTwoCurrentUIIndex);
                break;

            case 3:
                CompleteObjectiveOrdered(objectiveIndex, roomThreeObjectives, roomThreeObjectiveUI, ref roomThreeCurrentUIIndex);
                break;

            case 4:
                CompleteObjectiveOrdered(objectiveIndex, roomFourObjectives, roomFourObjectiveUI, ref roomFourCurrentUIIndex);
                break;
        }
    }
    //Returns objective index
    public bool IsObjectiveAvailable(int room, int objectiveIndex)
    {
        switch (room)
        {
            case 1:
                return roomOneCurrentUIIndex == objectiveIndex;
            case 2:
                return roomTwoCurrentUIIndex == objectiveIndex;
            case 3:
                return roomThreeCurrentUIIndex == objectiveIndex;
            case 4:
                return roomFourCurrentUIIndex == objectiveIndex;
            default:
                return false;
        }
    }
    //Items called from invokes 
    private void OnPlayerMoved()
    {
        CompleteObjectiveOrdered(
            0,
            roomOneObjectives,
            roomOneObjectiveUI,
            ref roomOneCurrentUIIndex);
    }

    private void OnCameraDragged()
    {
        CompleteObjectiveOrdered(
            1,
            roomOneObjectives,
            roomOneObjectiveUI,
            ref roomOneCurrentUIIndex);
    }

    private void OnCameraScrolled()
    {
        CompleteObjectiveOrdered(
            2,
            roomOneObjectives,
            roomOneObjectiveUI,
            ref roomOneCurrentUIIndex);
    }

    private void OnPunchHit()
    {
        CompleteObjectiveOrdered(
            0,
            roomTwoObjectives,
            roomTwoObjectiveUI,
            ref roomTwoCurrentUIIndex
        );
    }

    private void OnAOEHit()
    {
        CompleteObjectiveOrdered(
            1,
            roomTwoObjectives,
            roomTwoObjectiveUI,
            ref roomTwoCurrentUIIndex
        );
    }

    private void OnHeal()
    {
        CompleteObjectiveOrdered(
            2,
            roomTwoObjectives,
            roomTwoObjectiveUI,
            ref roomTwoCurrentUIIndex
        );
    }

    private void OnKeyCollect()
    {
        CompleteObjectiveOrdered(
            3,
            roomTwoObjectives,
            roomTwoObjectiveUI,
            ref roomTwoCurrentUIIndex
        );
    }
    private void OnFreePrisoner()
    {
        CompleteObjectiveOrdered(
            4,
            roomTwoObjectives,
            roomTwoObjectiveUI,
            ref roomTwoCurrentUIIndex
        );
    }
    private void OnPowerUpWithPunch()
    {
        CompleteObjectiveOrdered(
            1,
            roomThreeObjectives,
            roomThreeObjectiveUI,
            ref roomThreeCurrentUIIndex
        );
    }
    private void OnTreasureCollect()
    {
        CompleteObjectiveOrdered(
            2,
            roomThreeObjectives,
            roomThreeObjectiveUI,
            ref roomThreeCurrentUIIndex
        );
    }
    private void OnFreezeWithRangedBlast()
    {
        CompleteObjectiveOrdered(
            0,
            roomFourObjectives,
            roomFourObjectiveUI,
            ref roomFourCurrentUIIndex
        );
    }
    private void OnCrystalStab()
    {
        CompleteObjectiveOrdered(
            1,
            roomFourObjectives,
            roomFourObjectiveUI,
            ref roomFourCurrentUIIndex
        );
    }
    private void OnToggleZoomOut()
    {
        CompleteObjectiveOrdered(
            2,
            roomFourObjectives,
            roomFourObjectiveUI,
            ref roomFourCurrentUIIndex
        );
    }
    private void OnToggleZoomIn()
    {
        CompleteObjectiveOrdered(
            3,
            roomFourObjectives,
            roomFourObjectiveUI,
            ref roomFourCurrentUIIndex
        );
    }

    //Checks if all room objectives are complete and adds in final UI element if true to 'open the door'
    public void CheckFullRoomObjectives()
    {
        //Room 1
        allRoomOneComplete = roomOneObjectives.TrueForAll(o => o.completed);
        if (allRoomOneComplete && !roomOneDoorOpened)
        {
            doorOne?.GetComponent<TutorialDoorRemove>()?.SetDoorAbleToBeRemoved(true);

            if (roomOneFinalUI != null)
                roomOneFinalUI.SetActive(true);
        }

        //Room 2
        allRoomTwoComplete = roomTwoObjectives.TrueForAll(o => o.completed);
        if (allRoomTwoComplete && !roomTwoDoorOpened)
        {
            doorTwo?.GetComponent<TutorialDoorRemove>()?.SetDoorAbleToBeRemoved(true);

            if (roomTwoFinalUI != null)
                roomTwoFinalUI.SetActive(true);
        }

        //Room 3
        allRoomThreeComplete = roomThreeObjectives.TrueForAll(o => o.completed);
        if (allRoomThreeComplete && !roomThreeDoorOpened)
        {
            doorThree?.GetComponent<TutorialDoorRemove>()?.SetDoorAbleToBeRemoved(true);

            if (roomThreeFinalUI != null)
                roomThreeFinalUI.SetActive(true);
        }

        //Room 4
        allRoomFourComplete = roomFourObjectives.TrueForAll(o => o.completed);
        if (allRoomFourComplete && !roomFourDoorOpened)
        {
            doorFour?.GetComponent<TutorialDoorRemove>()?.SetDoorAbleToBeRemoved(true);

            if (roomFourFinalUI != null)
                roomFourFinalUI.SetActive(true);
        }
    }

    //Updates the Tutorial Objective UI elements which says all objectives in that specific room
    public void UpdateTutorialObjectiveText(List<Objective> objectiveList)
    {
        if (tutorialObjectiveText == null) return;

        string textOutput = "";
        foreach (var obj in objectiveList)
        {
            if (!obj.completed)
            {
                textOutput += obj.description + "\n";
            }
        }

        tutorialObjectiveText.text = textOutput;
    }
}