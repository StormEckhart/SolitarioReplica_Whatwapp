using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum e_PileTypes
{
    NotSpecified,
    PlayingPile,
    SuitPile,
}
public class BoardManager : SingletonMonoBehaviourManager<BoardManager>
{
    [System.Serializable]
    public struct Row
    {
        [Tooltip("A row holds a row of cards piles in an array")]
        public Pile[] Piles;



        //Constructor
        public Row(int i_StacksInRow)
        {
            Piles = new Pile[i_StacksInRow];
        }
    }
    [System.Serializable]
    public struct Pile
    {
        [Tooltip("This pile's type")]
        public e_PileTypes PileType;

        [Space]

        [Tooltip("All cards on this pile")]
        public List<CardRefs> CardsOnPile;

        [Space]

        [Tooltip("The parent holding all cards")]
        public RectTransform ParentPile;

        [Space]

        [Tooltip("If true, this pile is complete")]
        [HideIf("@PileType != e_PileTypes.SuitPile")]
        [SerializeField]
        private bool m_SuitPileIsComplete;
        public bool SuitPileIsComplete => m_SuitPileIsComplete;
        public void SetSuitPileCompletedState(bool i_PileIsComplete)
        {
            m_SuitPileIsComplete = i_PileIsComplete;

            BoardManager.Instance.CheckIfAllSuitPilesAreComplete();
        }
    }




    [Header("References")]

    [Tooltip("The main row of card piles to hold all dealed out cards on game start")]
    [SerializeField]
    private Row m_MainRow = new Row(7);
    public Row MainRow => m_MainRow;

    [Tooltip("The row of card piles to hold each suit set of cards")]
    [SerializeField]
    private Row m_SuitsRow = new Row(4);
    public Row SuitsRow => m_SuitsRow;

    [Space]

    [Tooltip("The pile that holds three cards maximum at once, on which the player can use the top card if need be")]
    [SerializeField]
    private Pile m_FlippedPile = new Pile();
    public Pile FlippedPile => m_FlippedPile;

    [Tooltip("The pile that holds the entire deck on game start")]
    [SerializeField]
    private Pile m_DeckPile = new Pile();
    public Pile DeckPile => m_DeckPile;

    [Space]

    [Tooltip("The index of how many flipped cards are currently on the front of said pile")]
    [HideInInspector]
    public int FlippedVisibleCardAmount = 0;


    [Tooltip("A reference to all piles")]
    public Pile[] AllPiles
    {
        get
        {
            List<Pile> l_PileToReturn = new List<Pile>();

            for (int i = 0; i < m_MainRow.Piles.Length; i++)
            {
                l_PileToReturn.Add(m_MainRow.Piles[i]);
            }
            for (int i = 0; i < m_SuitsRow.Piles.Length; i++)
            {
                l_PileToReturn.Add(m_SuitsRow.Piles[i]);
            }

            l_PileToReturn.Add(FlippedPile);

            l_PileToReturn.Add(DeckPile);

            return l_PileToReturn.ToArray();
        }
    }
    [Tooltip("A reference to all interactable piles")]
    public Pile[] AllInteractablePiles
    {
        get
        {
            List<Pile> l_PileToReturn = new List<Pile>();

            for (int i = 0; i < m_MainRow.Piles.Length; i++)
            {
                l_PileToReturn.Add(m_MainRow.Piles[i]);
            }
            for (int i = 0; i < m_SuitsRow.Piles.Length; i++)
            {
                l_PileToReturn.Add(m_SuitsRow.Piles[i]);
            }

            return l_PileToReturn.ToArray();
        }
    }


    #region Unity Events

    private void OnEnable()
    {
        GameManager.UpdateLevelState(e_LevelStates.Started);
    }
    private void OnDisable()
    {
    }

    #endregion


    #region Setters

    //Methods to move a card between card piles
    public void SwitchPile(Pile i_PileToSwitchTo, CardRefs i_CardToSwitch, e_CardFaceOptions i_ShouldUpdateCardFace, bool i_SwitchInstantly)
    {
        if (i_PileToSwitchTo.CardsOnPile.Contains(i_CardToSwitch) == false)
        {
            //Switch Pile
            Pile l_PileCardWasOn = i_CardToSwitch.PileCardIsOn;

            l_PileCardWasOn.CardsOnPile.Remove(i_CardToSwitch);
            i_PileToSwitchTo.CardsOnPile.Add(i_CardToSwitch);

            //Reveal the next card on the previous pile if there is a card left
            if (AllInteractablePiles.Contains(l_PileCardWasOn) == true)
            {
                if (l_PileCardWasOn.CardsOnPile.Count > 0) l_PileCardWasOn.CardsOnPile[l_PileCardWasOn.CardsOnPile.Count - 1].SwitchCardFaceShowing(e_CardFaceOptions.FaceShown, i_SwitchInstantly);
            }
        }


        //Update CardRefs parameters and visuals
        i_CardToSwitch.MoveCardToSpecificPile(i_PileToSwitchTo, i_ShouldUpdateCardFace, i_SwitchInstantly);
        i_CardToSwitch.SetPileCardIsOn(i_PileToSwitchTo);


        //Check if a suit pile has just been completed
        if (i_PileToSwitchTo.PileType == e_PileTypes.SuitPile && i_CardToSwitch.CardData.CardValue == e_CardValues.King) i_PileToSwitchTo.SetSuitPileCompletedState(true);
    }


    //Methods to clear all cards from a pile
    public void ClearPile(Pile i_StackToClear, bool i_DestroyAllCardsInPile)
    {
        if (i_DestroyAllCardsInPile == true)
        {
            for (int i = 0; i < i_StackToClear.CardsOnPile.Count; i++)
            {
                if (i_StackToClear.CardsOnPile[i] != null) DestroyImmediate(i_StackToClear.CardsOnPile[i].gameObject);
            }
        }

        i_StackToClear.SetSuitPileCompletedState(false);

        i_StackToClear.CardsOnPile.Clear();
    }

    #endregion


    #region Gameplay
    
    //Shuffle a specific pile
    public void ShufflePile(Pile i_StackToShuffle)
    {
        i_StackToShuffle.CardsOnPile.Shuffle();

        for (int i = 0; i < i_StackToShuffle.CardsOnPile.Count; i++)
        {
            i_StackToShuffle.CardsOnPile[i].transform.SetSiblingIndex(i);
        }
    }

    #endregion


    #region Rules

    //A bool returning true of false depending on if the move the player is trying to do is allowed or not
    public bool CanCardBePutIntoThisPile(Pile i_PileToPutCardInto, CardRefs i_CardToPut)
    {
        bool l_CanCardBePutIntoThisPile = false;


        if (i_PileToPutCardInto.PileType == e_PileTypes.SuitPile && i_CardToPut.InteractedCards.Count > 1) return l_CanCardBePutIntoThisPile;


        CardRefs TopCardOnPile = null;
        if (i_PileToPutCardInto.CardsOnPile.Count > 0)
        {
            TopCardOnPile = i_PileToPutCardInto.CardsOnPile[i_PileToPutCardInto.CardsOnPile.Count - 1];
        }

        switch (i_PileToPutCardInto.PileType)
        {
            case e_PileTypes.PlayingPile:
                if (TopCardOnPile == null)
                {
                    l_CanCardBePutIntoThisPile = true;

                    break;
                }


                int SmallestSuitIndex = Mathf.Min((int)TopCardOnPile.CardData.CardSuit, (int)i_CardToPut.CardData.CardSuit);
                int LargestSuitIndex = Mathf.Max((int)TopCardOnPile.CardData.CardSuit, (int)i_CardToPut.CardData.CardSuit);

                if (SmallestSuitIndex + 2 == LargestSuitIndex) break;

                if (TopCardOnPile.CardData.CardValue == (i_CardToPut.CardData.CardValue + 1))
                {
                    l_CanCardBePutIntoThisPile = true;
                }
                break;

            case e_PileTypes.SuitPile:
                if (i_PileToPutCardInto.ParentPile != m_SuitsRow.Piles[(int)i_CardToPut.CardData.CardSuit].ParentPile) break;


                if (TopCardOnPile == null)
                {
                    if (i_CardToPut.CardData.CardValue == e_CardValues.Ace)
                    {
                        l_CanCardBePutIntoThisPile = true;

                        break;
                    }
                }


                if (TopCardOnPile.CardData.CardSuit == i_CardToPut.CardData.CardSuit 
                    && 
                    TopCardOnPile.CardData.CardValue == i_CardToPut.CardData.CardValue - 1)
                {
                    l_CanCardBePutIntoThisPile = true;
                }
                    break;
        }


        return l_CanCardBePutIntoThisPile;
    }


    public void CheckIfAllSuitPilesAreComplete()
    {
        bool l_PilesAreComplete = true;


        for (int i = 0; i < m_SuitsRow.Piles.Length; i++)
        {
            if (m_SuitsRow.Piles[i].SuitPileIsComplete == false) l_PilesAreComplete = false;
        }


        if (l_PilesAreComplete == true) GameManager.UpdateLevelState(e_LevelStates.Won);
    }

    #endregion
}
