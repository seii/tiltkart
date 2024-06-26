﻿﻿using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{ 
    public bool IsFinite { get; private set; }
    public float TotalTime { get; private set; }
    public float TimeRemaining { get; private set; }
    public bool IsOver { get; private set; }

    private bool raceStarted;

    private string thisClass = nameof(TimeManager);

    public static Action<float> OnAdjustTime;
    public static Action<int, bool, GameMode> OnSetTime;

    private void Awake()
    {
        IsFinite = false;
        TimeRemaining = TotalTime;
    }


    void OnEnable()
    {
        //print($"{thisClass}: BeginEnable");
        OnAdjustTime += AdjustTime;
        OnSetTime += SetTime;
    }

    private void OnDisable()
    {
        OnAdjustTime -= AdjustTime;
        OnSetTime -= SetTime;
    }

    private void AdjustTime(float delta)
    {
        //print($"{thisClass}: Adjusting time by {delta}");
        TimeRemaining += delta;
    }

    private void SetTime(int time, bool isFinite, GameMode gameMode)
    {
        //print($"{thisClass}: Setting time to {time}, isFinite = {isFinite} for game mode {gameMode}");
        TotalTime = time;
        IsFinite = isFinite;
        TimeRemaining = TotalTime;
    }

    void Update()
    {
        if (!raceStarted) return;

        if (IsFinite && !IsOver)
        {
            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                IsOver = true;
            }
        }
    }

    public void StartRace()
    {
        print($"{thisClass}: Starting race");
        raceStarted = true;
    }

    public void StopRace() {
        print($"{thisClass}: Stopping race");
        raceStarted = false;
    }
}

