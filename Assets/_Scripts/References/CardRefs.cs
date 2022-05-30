using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum e_CardFaceOptions
{
    FaceHidden,
    FaceShown,
    LeaveUntouched,
    FlipOver
}
public class CardRefs : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Parameters")]

    [Tooltip("This card's value")]
    [ReadOnly]
    [SerializeField]
    private CardVariables.CardData m_CardData = new CardVariables.CardData();
    public CardVariables.CardData CardData => m_CardData;

    [Space]

    [Tooltip("If true, this card is front side up")]
    [ReadOnly]
    [SerializeField]
    private bool m_CardIsShown = false;
    public bool CardIsShown => m_CardIsShown;

    [Tooltip("If true, the player can interact with this card")]
    [ReadOnly]
    [SerializeField]
    private bool m_CardIsInteractable = false;
    public bool CardIsInteractable => m_CardIsInteractable;

    [Space]

    [Tooltip("The pile this card is currently on")]
    [ReadOnly]
    [SerializeField]
    private BoardManager.Pile m_PileCardIsOn;
    public BoardManager.Pile PileCardIsOn => m_PileCardIsOn;


    [Header("References")]

    [Tooltip("A reference to the card's RectTransform component")]
    [SerializeField]
    private RectTransform m_RectTransform;
    public RectTransform RectTransform => m_RectTransform;

    [Tooltip("A reference to the card's override Canvas component")]
    [SerializeField]
    private Canvas m_OverrideCanvas;
    public Canvas OverrideCanvas => m_OverrideCanvas;

    [Space]

    [Tooltip("A reference to this card's render Transform component")]
    [SerializeField]
    private RectTransform m_Render;

    [Space]

    [Tooltip("A reference to this card's render front face Transform component")]
    [SerializeField]
    private RectTransform m_FrontFace;

    [Tooltip("A reference to this card's render back face Transform component")]
    [SerializeField]
    private RectTransform m_BackFace;

    [Space]

    [Tooltip("A reference to this card's value Image component")]
    [SerializeField]
    private Image m_ValueRenderer;


    [Tooltip("A reference to this card's suit (small) Image component")]
    [SerializeField]
    private Image m_SuitSmallRenderer;

    [Tooltip("A reference to this card's suit (large) Image component")]
    [SerializeField]
    private Image m_SuitLargeRenderer;

    [Space]

    [Tooltip("A reference to all cards currently being interacted with")]
    [SerializeField]
    private List<CardRefs> m_InteractedCards = new List<CardRefs>();
    public List<CardRefs> InteractedCards => m_InteractedCards;



    //A shortcut for the card variables
    private CardVariables m_CardVars => GameConfig.Instance.Cards;



    #region Setters

    //Set this card's data
    public void SetCardValueAndSuite(e_CardValues i_WantedValue, e_CardSuits i_WantedSuit)
    {
        m_CardData.CardValue = i_WantedValue;
        m_CardData.CardSuit = i_WantedSuit;

        UpdateCardAppearanceRelativeToData();
    }


    //Set the pile this card is currently on
    public void SetPileCardIsOn(BoardManager.Pile i_PileCardIsNowOn)
    {
        m_PileCardIsOn = i_PileCardIsNowOn;
    }

    #endregion


    #region Visuals

    //Changes this card's appearance
    public void UpdateCardAppearanceRelativeToData()
    {
        CardVariables.CardVisual l_CardVisualToUse = m_CardVars.ReturnCardVisualRelativeToCardData(m_CardData);


        m_ValueRenderer.sprite = l_CardVisualToUse.ValueSprite;
        m_ValueRenderer.color = (int)m_CardData.CardSuit % 2 == 0 ? m_CardVars.RedSuitValueColor : m_CardVars.BlackSuitValueColor;
        m_ValueRenderer.SetNativeSize();

        m_SuitSmallRenderer.sprite = l_CardVisualToUse.SuitSprite;
        
        m_SuitLargeRenderer.sprite = l_CardVisualToUse.FigureSprite != null ? l_CardVisualToUse.FigureSprite : l_CardVisualToUse.SuitSprite;
    }


    //Flips over this card (front side up or down depending on the parameter boolean)
    [Button]
    public void SwitchCardFaceShowing(e_CardFaceOptions i_HowToUpdateCardFace, bool i_SwitchInstantly)
    {
        if (i_HowToUpdateCardFace == e_CardFaceOptions.LeaveUntouched) return;


        bool l_SetFrontSideUp = true;

        switch (i_HowToUpdateCardFace)
        {
            case e_CardFaceOptions.FaceHidden:
                if (m_CardIsShown == false) return;
                else l_SetFrontSideUp = false;
                break;

            case e_CardFaceOptions.FaceShown:
                if (m_CardIsShown == true) return;
                break;

            case e_CardFaceOptions.FlipOver:
                if (m_CardIsShown == true) l_SetFrontSideUp = false;
                break;
        }


        m_CardIsInteractable = false;

        if (i_SwitchInstantly == false)
        {
            Sequence l_SwitchCardFaceSequence = DOTween.Sequence();

            l_SwitchCardFaceSequence.Append(m_Render.DOScaleX(0f, m_CardVars.FlipOverDuration / 2f))
                                    .AppendCallback(() =>
                                    {
                                        m_FrontFace.gameObject.SetActive(l_SetFrontSideUp == true ? true : false);
                                        m_BackFace.gameObject.SetActive(l_SetFrontSideUp == true ? false : true);
                                    })
                                    .Append(m_Render.DOScaleX(1f, m_CardVars.FlipOverDuration / 2f))
                                    .OnComplete(() =>
                                    {
                                        m_CardIsShown = m_CardIsInteractable = l_SetFrontSideUp;

                                        CardManager.Instance.UpdateCardsShowing(this);
                                    });
        }
        else
        {
            m_FrontFace.gameObject.SetActive(l_SetFrontSideUp == true ? true : false);
            m_BackFace.gameObject.SetActive(l_SetFrontSideUp == true ? false : true);

            m_CardIsShown = m_CardIsInteractable = l_SetFrontSideUp;

            CardManager.Instance.UpdateCardsShowing(this);
        }
    }

    //Move this card to a specified pile
    public void MoveCardToSpecificPile(BoardManager.Pile i_PileToSetParentAsAndMoveTo, e_CardFaceOptions i_ShouldUpdateCardFace, bool i_MoveInstantly)
    {
        transform.SetParent(i_PileToSetParentAsAndMoveTo.ParentPile);


        Vector2 l_CardOffset = Vector2.zero;
        Vector2 l_PositionToGoTo = (Vector2)i_PileToSetParentAsAndMoveTo.ParentPile.position;

        if (i_PileToSetParentAsAndMoveTo.ParentPile == BoardManager.Instance.FlippedPile.ParentPile)
        {
            l_CardOffset = Vector2.left;

            l_PositionToGoTo += l_CardOffset * GameConfig.Instance.Board.CardsOnPileOffsetAmount * BoardManager.Instance.FlippedVisibleCardAmount;


            if (BoardManager.Instance.FlippedVisibleCardAmount == 2)
                BoardManager.Instance.FlippedVisibleCardAmount = 0;
            else
                BoardManager.Instance.FlippedVisibleCardAmount++;
        }
        else
        {
            if (i_PileToSetParentAsAndMoveTo.ParentPile != BoardManager.Instance.DeckPile.ParentPile)
            {
                l_CardOffset = Vector2.down;
            }

            l_PositionToGoTo += l_CardOffset * GameConfig.Instance.Board.CardsOnPileOffsetAmount * (i_PileToSetParentAsAndMoveTo.CardsOnPile.Count - 1);
        }


        if ((Vector2)transform.position == l_PositionToGoTo) return;


        m_OverrideCanvas.sortingOrder = 100 + i_PileToSetParentAsAndMoveTo.CardsOnPile.Count;

        m_CardIsInteractable = false;


        if (i_MoveInstantly == false)
        {
            Tween l_CardMovingTween = transform.DOMove(l_PositionToGoTo, m_CardVars.MoveFromToDuration)
                                   .OnComplete(() =>
                                   {
                                       if (m_CardIsShown == true) m_CardIsInteractable = true;

                                       if (i_ShouldUpdateCardFace != e_CardFaceOptions.LeaveUntouched)
                                       {
                                           SwitchCardFaceShowing(i_ShouldUpdateCardFace, i_MoveInstantly);
                                       }
                                   });
        }
        else
        {
            transform.position = l_PositionToGoTo;

            if (m_CardIsShown == true) m_CardIsInteractable = true;

            if (i_ShouldUpdateCardFace != e_CardFaceOptions.LeaveUntouched)
            {
                SwitchCardFaceShowing(i_ShouldUpdateCardFace, i_MoveInstantly);
            }
        }
    }

    #endregion


    #region Interacting

    //Make this card follow the player's input (finger)
    private IEnumerator FollowInputCoroutine()
    {
        //Set the sorting order higher than all other cards
        for (int i = 0; i < m_InteractedCards.Count; i++)
        {
            m_InteractedCards[i].OverrideCanvas.sortingOrder = 200 * (i + 1);
        }


        //int l = 0;

        while (true /*&& l < 10000*/)
        {
            for (int i = 0; i < m_InteractedCards.Count; i++)
            {
                m_InteractedCards[i].transform.position = InputManager.Instance.ReturnPrimaryCurrentPosition(false, true) + Vector2.down * GameConfig.Instance.Board.CardsOnPileOffsetAmount * i;
            }
            
            yield return null;

            //l++;
        }

        /*
        if (l >= 1000)
        {
            Debug.LogError("Stopped an infinite coroutine!");
        }
        */
    }

    #region Collision

    //Once the player's input released, check if the released card is overlapping with another, if so, switch its pile, if not, put it back into its original pile
    private void DetectCollisionAndUpdateCardIfAccordingly()
    {
        for (int i = 0; i < CardManager.Instance.CardsShowing.Length; i++)
        {
            if (CardManager.Instance.CardsShowing[i] == this) continue;


            //Shortcut for better readability
            var s_RectTranform2 = CardManager.Instance.CardsShowing[i].RectTransform;

            if (CustomReturnMethods.AreRectTransformsColliding(m_RectTransform, s_RectTranform2) == true)
            {
                Debug.LogError("Collision detected!");

                if (BoardManager.Instance.CanCardBePutIntoThisPile(CardManager.Instance.CardsShowing[i].PileCardIsOn, this) == true)
                {
                    for (int a = 0; a < m_InteractedCards.Count; a++)
                    {
                        BoardManager.Instance.SwitchPile(CardManager.Instance.CardsShowing[i].PileCardIsOn, m_InteractedCards[a], e_CardFaceOptions.FaceShown, false);
                    }

                    UIManager.Instance.UpdateInGameMovesText(1);
                    if (CardManager.Instance.CardsShowing[i].PileCardIsOn.PileType == e_PileTypes.SuitPile) UIManager.Instance.UpdateInGameScoreText(GameConfig.Instance.Gameplay.OnSuitPileCardAddedScoreAmount);

                    return;
                }
            }
        }

        for (int i = 0; i < BoardManager.Instance.AllInteractablePiles.Length; i++)
        {
            //Shortcut for better readability
            var s_RectTranform2 = BoardManager.Instance.AllInteractablePiles[i].ParentPile;

            if (CustomReturnMethods.AreRectTransformsColliding(m_RectTransform, s_RectTranform2) == true)
            {
                Debug.LogError("Collision detected!");

                if (BoardManager.Instance.CanCardBePutIntoThisPile(BoardManager.Instance.AllInteractablePiles[i], this) == true)
                {
                    for (int a = 0; a < m_InteractedCards.Count; a++)
                    {
                        BoardManager.Instance.SwitchPile(BoardManager.Instance.AllInteractablePiles[i], m_InteractedCards[a], e_CardFaceOptions.FaceShown, false);
                    }

                    UIManager.Instance.UpdateInGameMovesText(1);
                    if (CardManager.Instance.CardsShowing[i].PileCardIsOn.PileType == e_PileTypes.SuitPile) UIManager.Instance.UpdateInGameScoreText(GameConfig.Instance.Gameplay.OnSuitPileCardAddedScoreAmount);

                    return;
                }
            }
        }


        Debug.LogError("No collision");

        for (int a = 0; a < m_InteractedCards.Count; a++)
        {
            BoardManager.Instance.SwitchPile(m_PileCardIsOn, m_InteractedCards[a], e_CardFaceOptions.FaceShown, false);
        }
    }

    #endregion

    #endregion


    #region Interfaces

    //Detect current clicks on the GameObject (the one with the script attached)
    public void OnPointerDown(PointerEventData l_PointerEventData)
    {
        //Cards can only be interacted with once the game has properly started
        if (GameManager.Instance.CurrentGameplayState == e_GameplayStates.DealingDeck) return;


        //If the player tapped on the deck pile, then flip over the necessary amount of cards into the flipped pile
        if (m_PileCardIsOn.ParentPile == BoardManager.Instance.DeckPile.ParentPile) CardManager.Instance.FlipOverCards();


        //If the card isn't interactable right now, ignore it
        if (m_CardIsInteractable == false) return;


        m_InteractedCards.Clear();

        for (int i = 0; i < m_PileCardIsOn.CardsOnPile.Count - m_PileCardIsOn.CardsOnPile.IndexOf(this); i++)
        {
            m_InteractedCards.Add(m_PileCardIsOn.CardsOnPile[m_PileCardIsOn.CardsOnPile.IndexOf(this) + i]);
        }


        if (CardManager.Instance.FollowInputCoroutine != null) GameManager.Instance.StopCoroutine(CardManager.Instance.FollowInputCoroutine);

        CardManager.Instance.FollowInputCoroutine = GameManager.Instance.StartCoroutine(FollowInputCoroutine());
    }

    //Detect if click on the GameObject has been released (the one with the script attached)
    public void OnPointerUp(PointerEventData l_PointerEventData)
    {
        //Cards can only be interacted with once the game has properly started
        //OR
        //If the card isn't interactable right now, ignore it
        if (m_CardIsInteractable == false || GameManager.Instance.CurrentGameplayState == e_GameplayStates.DealingDeck) return;


        GameManager.Instance.StopCoroutine(CardManager.Instance.FollowInputCoroutine);

        DetectCollisionAndUpdateCardIfAccordingly();
    }

    #endregion
}
