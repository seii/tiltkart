using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using KartGame.KartSystems;

public enum GameState{Play, Won, Lost}

public class GameFlowManager : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("Duration of the fade-to-black at the end of the game")]
    public float endSceneLoadDelay = 3f;
    [Tooltip("The canvas group of the fade-to-black screen")]
    public List<CanvasGroup> endGameFadeCanvasGroup;

    [Header("Win")]
    [Tooltip("This string has to be the name of the scene you want to load when winning")]
    public string winSceneName = "WinScene";
    [Tooltip("Duration of delay before the fade-to-black, if winning")]
    public float delayBeforeFadeToBlack = 4f;
    [Tooltip("Duration of delay before the win message")]
    public float delayBeforeWinMessage = 2f;
    [Tooltip("Sound played on win")]
    public AudioClip victorySound;

    [Tooltip("Prefab for the win game message")]
    public List<DisplayMessage> winDisplayMessage;

    public List<PlayableDirector> raceCountdownTrigger;

    [Header("Lose")]
    [Tooltip("This string has to be the name of the scene you want to load when losing")]
    public string loseSceneName = "LoseScene";
    [Tooltip("Prefab for the lose game message")]
    public List<DisplayMessage> loseDisplayMessage;

    private string thisClass = nameof(GameFlowManager);


    public GameState gameState { get; private set; }

    public bool autoFindKarts = true;
    public ArcadeKart playerKart;

    ArcadeKart[] karts;
    ObjectiveManager m_ObjectiveManager;
    TimeManager m_TimeManager;
    float m_TimeLoadEndGameScene;
    string m_SceneToLoad;
    float elapsedTimeBeforeEndScene = 0;

    private bool isStarted;

    void Start()
    {
        ObjectiveManager.allObjectivesComplete += AllObjectivesComplete;

        if (!isStarted)
        {
            isStarted = true;

            karts = FindObjectsOfType<ArcadeKart>();

            if (autoFindKarts)
            {
                if (karts.Length > 0)
                {
                    if (!playerKart) playerKart = karts[0];
                }
                DebugUtility.HandleErrorIfNullFindObject<ArcadeKart, GameFlowManager>(playerKart, this);
            }

            m_ObjectiveManager = FindObjectOfType<ObjectiveManager>();
            DebugUtility.HandleErrorIfNullFindObject<ObjectiveManager, GameFlowManager>(m_ObjectiveManager, this);

            m_TimeManager = FindObjectOfType<TimeManager>();
            DebugUtility.HandleErrorIfNullFindObject<TimeManager, GameFlowManager>(m_TimeManager, this);

            // Audio seems very loud, reduce it from default value of "1"
            AudioUtility.SetMasterVolume(0.8f);

            foreach (DisplayMessage msg in winDisplayMessage)
            {
                msg.gameObject.SetActive(false);
            }
            //winDisplayMessage.gameObject.SetActive(false);

            foreach (DisplayMessage msg in loseDisplayMessage)
            {
                msg.gameObject.SetActive(false);
            }
            //loseDisplayMessage.gameObject.SetActive(false);

            m_TimeManager.StopRace();
            foreach (ArcadeKart k in karts)
            {
                k.SetCanMove(false);
            }

            //run race countdown animation
            ShowRaceCountdownAnimation();
            StartCoroutine(ShowObjectivesRoutine());

            StartCoroutine(CountdownThenStartRaceRoutine());
        }
    }

    private void OnDestroy()
    {
        ObjectiveManager.allObjectivesComplete -= AllObjectivesComplete;
    }

    IEnumerator CountdownThenStartRaceRoutine() {
        yield return new WaitForSeconds(3f);
        StartRace();
    }

    void StartRace() {
        print($"{thisClass}: Starting race");
        foreach (ArcadeKart k in karts)
        {
            k.SetCanMove(true);
        }

        m_TimeManager.StartRace();
    }

    void ShowRaceCountdownAnimation() {
        foreach(PlayableDirector dir in raceCountdownTrigger)
        {
            dir.Play();
        }
        //raceCountdownTrigger.Play();
    }

    IEnumerator ShowObjectivesRoutine() {
        while (m_ObjectiveManager.Objectives.Count == 0)
            yield return null;
        yield return new WaitForSecondsRealtime(0.2f);
        for (int i = 0; i < m_ObjectiveManager.Objectives.Count; i++)
        {
           if (m_ObjectiveManager.Objectives[i].displayMessage)m_ObjectiveManager.Objectives[i].displayMessage.Display();
           yield return new WaitForSecondsRealtime(1f);
        }
    }


    void Update()
    {

        if (gameState != GameState.Play)
        {
            //print($"{thisClass}: Entering {gameState} mode");
            elapsedTimeBeforeEndScene += Time.deltaTime;
            if(elapsedTimeBeforeEndScene >= endSceneLoadDelay)
            {

                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / endSceneLoadDelay;
                foreach(CanvasGroup group in endGameFadeCanvasGroup)
                {
                    group.alpha = timeRatio;
                }
                //endGameFadeCanvasGroup.alpha = timeRatio;

                float volumeRatio = Mathf.Abs(timeRatio);
                float volume = Mathf.Clamp(1 - volumeRatio, 0, 1);
                AudioUtility.SetMasterVolume(volume);

                // See if it's time to load the end scene (after the delay)
                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    print($"{thisClass}: Loading next level");
                    SceneSwitcher.Instance.BeginLoad(m_SceneToLoad, true, true);
                    gameState = GameState.Play;
                }
            }
        }
        /*else
        {
            if (m_ObjectiveManager.AreAllObjectivesCompleted())
            {
                print($"{thisClass}: Objectives complete, ending game");
                EndGame(true);
            }

            if (m_TimeManager.IsFinite && m_TimeManager.IsOver)
            {
                print($"{thisClass}: Time is up");
                EndGame(false);
            }
        }*/
    }

    void EndGame(bool win)
    {
        print($"{thisClass}: Attempting to end game");

        // unlocks the cursor before leaving the scene, to be able to click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_TimeManager.StopRace();

        // Remember that we need to load the appropriate end scene after a delay
        gameState = win ? GameState.Won : GameState.Lost;
        foreach(CanvasGroup group in endGameFadeCanvasGroup)
        {
            group.gameObject.SetActive(true);
        }
        //endGameFadeCanvasGroup.gameObject.SetActive(true);
        if (win)
        {
            m_SceneToLoad = winSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;

            // play a sound on win
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = victorySound;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);
            audioSource.PlayScheduled(AudioSettings.dspTime + delayBeforeWinMessage);

            // create a game message
            foreach(DisplayMessage msg in winDisplayMessage)
            {
                msg.delayBeforeShowing = delayBeforeWinMessage;
                msg.gameObject.SetActive(true);
            }
            /*winDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            winDisplayMessage.gameObject.SetActive(true);*/
        }
        else
        {
            m_SceneToLoad = loseSceneName;
            m_TimeLoadEndGameScene = Time.time + endSceneLoadDelay + delayBeforeFadeToBlack;

            // create a game message
            foreach(DisplayMessage msg in loseDisplayMessage)
            {
                msg.delayBeforeShowing = delayBeforeWinMessage;
                msg.gameObject.SetActive(true);
            }
            /*loseDisplayMessage.delayBeforeShowing = delayBeforeWinMessage;
            loseDisplayMessage.gameObject.SetActive(true);*/
        }
    }

    private void AllObjectivesComplete(bool win)
    {
        print($"{thisClass}: Received AllObjectivesComplete signal");
        EndGame(win);
    }
}
