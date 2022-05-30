using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public enum e_CardValues
{
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}
public enum e_CardSuits
{
    Diamonds,
    Club,
    Hearts,
    Spades
}
[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : SingletonScriptableObjectManager<GameConfig>
{
    public GameplayVariables Gameplay = new GameplayVariables();
    public CardVariables Cards = new CardVariables();
    public BoardVariables Board = new BoardVariables();
    public HUDVariables HUD = new HUDVariables();
    public MenuVariables Menus = new MenuVariables();
    public FeedbackVariables Feedbacks = new FeedbackVariables();
}



[Serializable]
public class CardVariables
{
    //A struct holding a card's data (its value and suit)
    [System.Serializable]
    public struct CardData
    {
        [Tooltip("This card's value")]
        public e_CardValues CardValue;

        [Tooltip("This card's suit")]
        public e_CardSuits CardSuit;



        //Constructor
        public CardData(e_CardValues i_CardValue, e_CardSuits i_CardSuit)
        {
            CardValue = i_CardValue;
            CardSuit = i_CardSuit;
        }
    }

    //A struct holding a card's visual (the sprites associated with its value, figure (if J, Q or K) and suit)
    [System.Serializable]
    public struct CardVisual
    {
        [Tooltip("This card's value visual")]
        public Sprite ValueSprite;

        [Tooltip("This card's figure visual (only has one if it's a J, Q or K)")]
        public Sprite FigureSprite;

        [Tooltip("This card's suit visual")]
        public Sprite SuitSprite;
    }



    [Tooltip("The array holding all card visuals (sprites)")]
    public CardVisual[] CardVisuals = new CardVisual[52];

    [Space]

    [Tooltip("The array holding all value card sprites")]
    [SerializeField]
    private Sprite[] m_ValueVisuals = new Sprite[14];

    [Tooltip("The array holding all black figure card sprites")]
    [SerializeField]
    private Sprite[] m_BlackFigureVisuals = new Sprite[2];
    [Tooltip("The array holding all red figure card sprites")]
    [SerializeField]
    private Sprite[] m_RedFigureVisuals = new Sprite[2];

    [Tooltip("The array holding all suit card sprites")]
    [SerializeField]
    private Sprite[] m_SuitVisuals = new Sprite[4];

    [Space]

    [Tooltip("The red suit's value color")]
    public Color RedSuitValueColor;
    [Tooltip("The black suit's value color")]
    public Color BlackSuitValueColor;

    [Space]

    [Tooltip("The time it takes to flip over a card")]
    public float FlipOverDuration = 0.5f;

    [Tooltip("The time it takes to move a card from a position to another")]
    public float MoveFromToDuration = 0.5f;



    #region Inspector Methods

    [Button]
    private void SetupCardVisuals()
    {
        int l_ValueIndex = 0;
        int l_SuitIndex = 0;


        for (int i = 0; i < CardVisuals.Length; i++)
        {
            CardVisuals[i].ValueSprite = m_ValueVisuals[l_ValueIndex];

            if (l_ValueIndex >= (int)e_CardValues.Jack)
            {
                if (l_SuitIndex % 2 != 0)
                {
                    CardVisuals[i].FigureSprite = m_BlackFigureVisuals[l_ValueIndex - 10];
                }
                else
                {
                    CardVisuals[i].FigureSprite = m_RedFigureVisuals[l_ValueIndex - 10];
                }
            }

            CardVisuals[i].SuitSprite = m_SuitVisuals[l_SuitIndex];


            if ((e_CardValues)l_ValueIndex == e_CardValues.King)
            {
                l_ValueIndex = 0;
                l_SuitIndex++;
            }
            else
            {
                l_ValueIndex++;
            }
        }
    }

    #endregion

    #region Called Methods

    //To return the correct visuals relative to a specific card data (value and suit)
    public CardVisual ReturnCardVisualRelativeToCardData(CardData i_RelatedCardData)
    {
        int l_VisualIndex = (int)i_RelatedCardData.CardValue + (int)i_RelatedCardData.CardSuit * Enum.GetNames(typeof(e_CardValues)).Length;

        return CardVisuals[l_VisualIndex];
    }

    #endregion
}

[Serializable]
public class BoardVariables
{
    [Tooltip("Pause duration between each card being dealt on game start")]
    public float PauseDurationBetweenCardsDealt = 0.25f;

    [Space]

    [Tooltip("The amount of cards that will be dealt onto each main pile on deck dealt")]
    public int[] CardAmountsOnPilesOnDeckDealt = new int[7];

    [Space]
 
    [Tooltip("The offset amount between each card on a same pile")]
    public float CardsOnPileOffsetAmount = 0.5f;
}

[Serializable]
public class GameplayVariables
{
    public enum e_DrawAmounts
    {
        Single = 1,
        Three = 3
    }


    [Tooltip("The game variant type that is currently being played")]
    public e_DrawAmounts DrawAmount = e_DrawAmounts.Single;
}

[Serializable]
public class HUDVariables
{
}

[Serializable]
public class MenuVariables
{
}

[Serializable]
public class FeedbackVariables
{
}