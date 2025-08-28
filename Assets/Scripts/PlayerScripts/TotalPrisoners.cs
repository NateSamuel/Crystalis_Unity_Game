using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TotalPrisoners : MonoBehaviour
{
    public int prisonersCount = 0;
    [SerializeField] private List<TextMeshProUGUI> prisonerTexts;
    private EnemyTrackerForObjectives enemyTrackerForObjectives;

    void Start()
    {

        FindPrisoners();
        enemyTrackerForObjectives = FindAnyObjectByType<EnemyTrackerForObjectives>();
    }
    public void FindPrisoners()
    {
        Invoke(nameof(CountPrisoners), 0.1f);
    }

    void CountPrisoners()
    {
        prisonersCount = GameObject.FindGameObjectsWithTag("Prisoner").Length;
        UpdatePrisonerUI();
    }


    public void RemovePrisoner(int prisonerAmount)
    {
        prisonersCount -= prisonerAmount;
        UpdatePrisonerUI();
    }
    public void ResetPrisoners(int prisonerAmount)
    {
        prisonersCount = prisonerAmount;

        UpdatePrisonerUI();
    }

    private void UpdatePrisonerUI()
    {
        enemyTrackerForObjectives.UpdateStatusText();
    }
}