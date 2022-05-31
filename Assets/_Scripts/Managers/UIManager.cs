using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : SingletonMonoBehaviourManager<UIManager>
{
    public enum e_UIScreens
    {
        None,
        Pause,
        Options,
        Success,
    }


    [Header("Parameters")]

    [ReadOnly]
    [Tooltip("The current UI screen displayed")]
    [SerializeField]
    private e_UIScreens m_CurrentDisplayedUIScreen;


    [Header("References")]

    [Tooltip("A reference to the UI's success screen")]
    [SerializeField]
    private GameObject m_SuccessScreen;

    [Tooltip("A reference to the UI's pause screen")]
    [SerializeField]
    private GameObject m_PauseScreen;

    [Tooltip("A reference to the UI's options screen")]
    [SerializeField]
    private GameObject m_OptionsScreen;

    [Space]

    [Tooltip("A reference to the UI's options draw amount text")]
    [SerializeField]
    private TextMeshProUGUI m_DrawAmountText;
    [Tooltip("A reference to the UI's options timer activated state text")]
    [SerializeField]
    private TextMeshProUGUI m_TimerActivatedText;
    [Tooltip("A reference to the UI's HUD timer area")]
    [SerializeField]
    private GameObject m_InGameTimerArea;
    [Tooltip("A reference to the UI's options warning text")]
    [SerializeField]
    private GameObject m_OptionsWarningText;

    [Space]

    [Tooltip("A reference to the UI's HUD move amount text")]
    [SerializeField]
    private TextMeshProUGUI m_InGameMovesText;
    [Tooltip("A reference to the UI's HUD timer text")]
    [SerializeField]
    private TextMeshProUGUI m_InGameTimerText;
    [Tooltip("A reference to the UI's HUD current score text")]
    [SerializeField]
    private TextMeshProUGUI m_InGameScoreText;



    [Tooltip("The amount of moves the player currently has done")]
    private int m_CurrentGameMoves = 0;
    [Tooltip("The time the current game is lasting")]
    private float m_CurrentGameTime = 0f;
    [Tooltip("The current score the player has")]
    private int m_CurrentGameScore = 0;

    [Tooltip("If true, the timer is activated and counts how long the current game is lasting")]
    private bool m_TimerIsActivated = false;

    [Tooltip("If true, once the options screen closed, the game will restart")]
    private bool m_OptionsHaveBeenModified = false;



    #region Unity Events

    //Subscribing and unsubscribing the appropriate level related functions to the appropriate game manager events
    private void OnEnable()
    {
        GameManager.OnLevelStartEvent += OnLevelStarted;
        GameManager.OnLevelSuccessEvent += OnLevelSuccess;
        GameManager.OnLevelPauseEvent += OnLevelPause;
    }
    private void OnDisable()
    {
        GameManager.OnLevelStartEvent -= OnLevelStarted;
        GameManager.OnLevelSuccessEvent -= OnLevelSuccess;
        GameManager.OnLevelPauseEvent -= OnLevelPause;
    }

    private void Update()
    {
        if (m_CurrentDisplayedUIScreen != e_UIScreens.None || GameManager.Instance.CurrentGameplayState != e_GameplayStates.Playing || m_InGameTimerArea.activeSelf == false) return;

        //Keep track of current game time and update timer text component
        m_CurrentGameTime += Time.deltaTime;
        m_InGameTimerText.text = m_CurrentGameTime.ToString("00.00");
    }

    #region Custom Events

    private void OnLevelStarted()
    {
        CloseCurrentUIScreen();


        //Reset game variables
        m_CurrentGameTime = 0f;
        m_InGameTimerText.text = m_CurrentGameTime.ToString("00.00");

        m_CurrentGameMoves = 0;
        m_InGameMovesText.text = m_CurrentGameMoves.ToString();

        m_CurrentGameScore = 0;
        m_InGameScoreText.text = m_CurrentGameScore.ToString();


        //Show correct draw amount in options
        m_DrawAmountText.text = GameConfig.Instance.Gameplay.DrawAmount.ToString();


        //Reset options elements
        m_OptionsHaveBeenModified = false;
        m_OptionsWarningText.SetActive(false);
    }
    private void OnLevelSuccess()
    {
        ShowSpecificUIScreen(e_UIScreens.Success);
    }
    private void OnLevelPause()
    {
        ShowSpecificUIScreen(e_UIScreens.Options);
    }

    #endregion

    #endregion


    #region UI

    //Show the wanted UI screen whilst closing the others
    public void ShowSpecificUIScreen(e_UIScreens i_WantedUIScreen)
    {
        if (m_CurrentDisplayedUIScreen == i_WantedUIScreen) return;


        m_PauseScreen.SetActive(false);
        m_OptionsScreen.SetActive(false);
        m_SuccessScreen.SetActive(false);

        switch (i_WantedUIScreen)
        {
            case e_UIScreens.Pause:
                m_PauseScreen.SetActive(true);
                break;

            case e_UIScreens.Options:
                m_OptionsScreen.SetActive(true);
                break;

            case e_UIScreens.Success:
                m_SuccessScreen.SetActive(true);
                break;
        }

        m_CurrentDisplayedUIScreen = i_WantedUIScreen;
    }
    public void CloseCurrentUIScreen()
    {
        if (m_CurrentDisplayedUIScreen == e_UIScreens.None) return;

        switch (m_CurrentDisplayedUIScreen)
        {
            case e_UIScreens.Pause:
                m_PauseScreen.SetActive(false);
                break;

            case e_UIScreens.Options:
                m_OptionsScreen.SetActive(false);
                break;

            case e_UIScreens.Success:
                m_SuccessScreen.SetActive(false);
                break;
        }

        m_CurrentDisplayedUIScreen = e_UIScreens.None;


        if (m_OptionsHaveBeenModified == true)
        {
            RestartGame();
        }
    }


    #region InGame Methods

    //Update the move amount in the moves text component
    public void UpdateInGameMovesText(int i_ValueToAdd)
    {
        m_CurrentGameMoves += i_ValueToAdd;

        m_InGameMovesText.text = m_CurrentGameMoves.ToString();
    }
    //Update the current score in the score text component
    public void UpdateInGameScoreText(int i_ValueToAdd)
    {
        m_CurrentGameScore += i_ValueToAdd;

        m_InGameScoreText.text = m_CurrentGameScore.ToString();
    }

    #endregion

    #region Screen Methods

    public void OpenOptionsScreen()
    {
        ShowSpecificUIScreen(e_UIScreens.Options);
    }
    public void OpenPauseScreen()
    {
        ShowSpecificUIScreen(e_UIScreens.Pause);
    }


    public void RestartGame()
    {
        GameManager.UpdateLevelState(e_LevelStates.Started, true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }


    public void SwitchDrawAmount()
    {
        if (GameConfig.Instance.Gameplay.DrawAmount == GameplayVariables.e_DrawAmounts.Single)
        {
            GameConfig.Instance.Gameplay.DrawAmount = GameplayVariables.e_DrawAmounts.Three;
        }
        else
        {
            GameConfig.Instance.Gameplay.DrawAmount = GameplayVariables.e_DrawAmounts.Single;
        }

        m_DrawAmountText.text = GameConfig.Instance.Gameplay.DrawAmount.ToString();


        m_OptionsHaveBeenModified = !m_OptionsHaveBeenModified;
        m_OptionsWarningText.SetActive(m_OptionsHaveBeenModified);
    }
    public void SwitchTimerEnabled()
    {
        m_TimerIsActivated = !m_TimerIsActivated;

        m_TimerActivatedText.text = m_TimerIsActivated.ToString();

        m_InGameTimerArea.SetActive(m_TimerIsActivated);


        m_OptionsHaveBeenModified = !m_OptionsHaveBeenModified;
        m_OptionsWarningText.SetActive(m_OptionsHaveBeenModified);
    }

    #endregion

    #endregion
}
