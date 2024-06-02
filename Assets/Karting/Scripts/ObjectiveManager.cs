using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    List<Objective> m_Objectives = new List<Objective>();

    public List<Objective> Objectives => m_Objectives;

    public static Action<Objective> RegisterObjective;

    public static event Action<bool> allObjectivesComplete;

    private bool isCompleted;

    private string thisClass = nameof(ObjectiveManager);

    public void OnEnable()
    {
        //print($"{thisClass}: BeginEnable");
        RegisterObjective += OnRegisterObjective;
        isCompleted = false;
        print($"Enabled {thisClass}");
    }

    private void Update()
    {
        if(!isCompleted && m_Objectives.Count > 0 && AreAllObjectivesCompleted())
        {
            isCompleted = true;
            allObjectivesComplete?.Invoke(true);
        }
    }

    public bool AreAllObjectivesCompleted()
    {
        if (m_Objectives.Count == 0)
            return false;

        for (int i = 0; i < m_Objectives.Count; i++)
        {
            // pass every objectives to check if they have been completed
            if (m_Objectives[i].isBlocking())
            {
                // break the loop as soon as we find one uncompleted objective
                return false;
            }
        }

        // found no uncompleted objective
        return true;
    }

    public void OnRegisterObjective(Objective objective)
    {
        //print($"{thisClass}: Received objective");
        m_Objectives.Add(objective);
    }
}
