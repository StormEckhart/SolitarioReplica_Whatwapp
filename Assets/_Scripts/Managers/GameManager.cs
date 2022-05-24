using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum e_GameStates
{
    Launching,
    InMainMenu,
    InGame
}
public enum e_LevelStates
{
    Inactive,
    Started,
    Paused,
    Failed,
    Won
}
public enum e_GameplayStates
{
    Inactive,
    DealingDeck,
}

public class GameManager : SingletonMonoBehaviourManager<GameManager>
{
    #region Events

    //An event called at game launch, for all necessary functions to subscribe to
    public static event Action OnGameLaunchEvent;
    //An event called on switching to the main menu, for all necessary functions to subscribe to
    public static event Action OnGameSwitchToMainMenuEvent;
    //An event called on switching to in game, for all necessary functions to subscribe to
    public static event Action OnGameSwitchToInGameEvent;


    //An event called on level start, for all necessary functions to subscribe to
    public static event Action OnLevelStartEvent;
    //An event called on level paused, for all necessary functions to subscribe to
    public static event Action OnLevelPauseEvent;
    //An event called on level failure, for all necessary functions to subscribe to
    public static event Action OnLevelFailureEvent;
    //An event called on level success, for all necessary functions to subscribe to
    public static event Action OnLevelSuccessEvent;


    //An event called when dealing the deck to the player, for all necessary functions to subscribe to
    public static event Action OnGameplayDealingDeckEvent;

    #endregion



    [Header("Useful")]

    [Tooltip("The current game state")]
    [ReadOnly]
    [SerializeField]
    private e_GameStates m_CurrentGameState = e_GameStates.Launching;
    public e_GameStates CurrentGameState => m_CurrentGameState;

    [Tooltip("The current level state")]
    [ReadOnly]
    [SerializeField]
    private e_LevelStates m_CurrentLevelState = e_LevelStates.Inactive;
    public e_LevelStates CurrentLevelState => m_CurrentLevelState;

    [Tooltip("The current gameplay state")]
    [ReadOnly]
    [SerializeField]
    private e_GameplayStates m_CurrentGameplayState = e_GameplayStates.Inactive;
    public e_GameplayStates CurrentGameplayState => m_CurrentGameplayState;



    private void Start()
    {
        //Call the appropriate delegate for the game launch
        OnGameLaunchEvent?.Invoke();
    }


    //Update the current game state and call the appropriate delegate for it
    public static void UpdateGameState(e_GameStates i_WantedGameState)
    {
        if (GameManager.Instance.m_CurrentGameState == i_WantedGameState) return;

        GameManager.Instance.m_CurrentGameState = i_WantedGameState;


        Action delegateToCall = null;

        switch (i_WantedGameState)
        {
            case e_GameStates.InMainMenu:
                delegateToCall = OnGameSwitchToMainMenuEvent;
                break;

            case e_GameStates.InGame:
                delegateToCall = OnGameSwitchToInGameEvent;
                break;
        }


        Debug.LogError(i_WantedGameState);
        delegateToCall?.Invoke();
    }

    //Update the current level state and call the appropriate delegate for it
    public static void UpdateLevelState(e_LevelStates i_WantedLevelState)
    {
        if (GameManager.Instance.m_CurrentLevelState == i_WantedLevelState) return;

        GameManager.Instance.m_CurrentLevelState = i_WantedLevelState;


        Action delegateToCall = null;
    
        switch (i_WantedLevelState)
        {
            case e_LevelStates.Started:
                delegateToCall = OnLevelStartEvent;
                break;

            case e_LevelStates.Paused:
                delegateToCall = OnLevelPauseEvent;
                break;
            case e_LevelStates.Failed:
                delegateToCall = OnLevelFailureEvent;
                break;
            case e_LevelStates.Won:
                delegateToCall = OnLevelSuccessEvent;
                break;
        }

        Debug.LogError(i_WantedLevelState);
        delegateToCall?.Invoke();
    }

    //Update the current gameplay state and call the appropriate delegate for it
    public static void UpdateGameplayState(e_GameplayStates i_WantedGameplayState)
    {
        if (GameManager.Instance.m_CurrentGameplayState == i_WantedGameplayState) return;

        GameManager.Instance.m_CurrentGameplayState = i_WantedGameplayState;


        Action delegateToCall = null;

        switch (i_WantedGameplayState)
        {
            case e_GameplayStates.DealingDeck:
                delegateToCall = OnGameplayDealingDeckEvent;
                break;
        }

        Debug.LogError(i_WantedGameplayState);
        delegateToCall?.Invoke();
    }
}
