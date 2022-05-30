using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardManager : SingletonMonoBehaviourManager<CardManager>
{
    [Header("References")]

    [Tooltip("The array holding all pooled cards (the whole 52 card by default)")]
    //[ReadOnly]
    [SerializeField]
    private CardRefs[] m_Deck = new CardRefs[52];

    [Tooltip("The parent holding all pooled cards")]
    [SerializeField]
    private RectTransform m_PooledParent;

    [Space]

    [Tooltip("The card prefab")]
    [SerializeField]
    private CardRefs m_CardPrefab;

    [Space]

    [Tooltip("The list holding all cards that are currently face up (being showed)")]
    [ReadOnly]
    [SerializeField]
    private List<CardRefs> m_CardsShowing = new List<CardRefs>();
    public CardRefs[] CardsShowing => m_CardsShowing.ToArray();

    [Space]

    [Tooltip("A variable for the FollowInput coroutine in CardRefs script, to start and stop it")]
    public Coroutine FollowInputCoroutine;



    //Shortcut by code to create all cards in deck (instead of manually)
    [Button]
    private void SetupCards()
    {
        //Make sure to destroy all remaining cards before spawning new ones
        CardRefs[] l_AllCardRemnants = FindObjectsOfType<CardRefs>(true);

        for (int i = 0; i < l_AllCardRemnants.Length; i++)
        {
            DestroyImmediate(l_AllCardRemnants[i].gameObject);
        }


        //Reset the deck array count
        m_Deck = new CardRefs[52];
        //Reset all board piles too
        for (int i = 0; i < BoardManager.Instance.AllPiles.Length; i++)
        {
            BoardManager.Instance.ClearPile(BoardManager.Instance.AllPiles[i], true);
        }


        //Indexes used to know which value and which suit needs to be created next
        int l_ValueIndex = (int)e_CardValues.Ace;
        int l_SuitIndex = (int)e_CardSuits.Diamonds;


        //Create each card and add each one to the array,
        //then add it to the correct card pile (in BoardManager)
        for (int i = 0; i < m_Deck.Length; i++)
        {
            if (m_Deck[i] != null) DestroyImmediate(m_Deck[i].gameObject);


            CardRefs l_CurrentCardRefs = PrefabUtility.InstantiatePrefab(m_CardPrefab, m_PooledParent) as CardRefs;


            l_CurrentCardRefs.SetCardValueAndSuite((e_CardValues)l_ValueIndex, (e_CardSuits)l_SuitIndex);

            if ((e_CardValues)l_ValueIndex == e_CardValues.King)
            {
                l_ValueIndex = (int)e_CardValues.Ace;
                l_SuitIndex++;
            }
            else
            {
                l_ValueIndex++;
            }


            l_CurrentCardRefs.gameObject.name = StaticStrings.Names.Card + " (" + l_CurrentCardRefs.CardData.CardValue + " " + l_CurrentCardRefs.CardData.CardSuit + ")";


            m_Deck[i] = l_CurrentCardRefs;
            
            
            //BoardManager.Instance.SwitchPile(BoardManager.Instance.DeckPile, l_CurrentCardRefs, e_CardFaceOptions.FaceHidden);
        }
    }


    #region Unity Events

    private void OnEnable()
    {
        GameManager.OnLevelStartEvent += DealDeck;
    }
    private void OnDisable()
    {
        GameManager.OnLevelStartEvent -= DealDeck;
    }

    #endregion


    #region Setters

    //Used to add or remove any cards that have switched face
    public void UpdateCardsShowing(CardRefs i_CardToUpdate)
    {
        if (i_CardToUpdate.CardIsShown == true)
        {
            if (m_CardsShowing.Contains(i_CardToUpdate) == false) m_CardsShowing.Add(i_CardToUpdate);
        }
        else
        {
            if (m_CardsShowing.Contains(i_CardToUpdate) == true) m_CardsShowing.Remove(i_CardToUpdate);
        }
    }

    #endregion


    #region Gameplay

    //Adds all cards into the deck pile
    private void AddDeckToDeckPile()
    {
        for (int i = 0; i < BoardManager.Instance.AllPiles.Length; i++)
        {
            BoardManager.Instance.ClearPile(BoardManager.Instance.AllPiles[i], false);
        }

        for (int i = 0; i < m_Deck.Length; i++)
        {
            BoardManager.Instance.SwitchPile(BoardManager.Instance.DeckPile, m_Deck[i], e_CardFaceOptions.FaceHidden, true);
        }
    }
    //Shuffle the deck pile, and can or not first put back all flipped cards into said pile
    public void ShuffleDeckPile()
    {
        BoardManager.Instance.ShufflePile(BoardManager.Instance.DeckPile);
    }


    //Deal the deck into all main playing piles
    private void DealDeck()
    {
        AddDeckToDeckPile();

        ShuffleDeckPile();

        GameManager.UpdateGameplayState(e_GameplayStates.DealingDeck);

        GameManager.Instance.StartCoroutine(DealCardsCoroutine());
    }
    //A coroutine used to deal out all the necessary cards into the main playing piles
    private IEnumerator DealCardsCoroutine()
    {
        for (int i = 0; i < BoardManager.Instance.MainRow.Piles.Length; i++)
        {
            for (int j = 0; j < GameConfig.Instance.Board.CardAmountsOnPilesOnDeckDealt[i]; j++)
            {
                yield return new WaitForSeconds(GameConfig.Instance.Board.PauseDurationBetweenCardsDealt);

                Debug.LogError(BoardManager.Instance.DeckPile.CardsOnPile.Count - 1);

                BoardManager.Instance.SwitchPile(BoardManager.Instance.MainRow.Piles[i], BoardManager.Instance.DeckPile.CardsOnPile[BoardManager.Instance.DeckPile.CardsOnPile.Count - 1], j == GameConfig.Instance.Board.CardAmountsOnPilesOnDeckDealt[i] - 1 ? e_CardFaceOptions.FaceShown : e_CardFaceOptions.FaceHidden, false);
            }
        }

        yield return new WaitForSeconds(GameConfig.Instance.Cards.MoveFromToDuration);

        GameManager.UpdateGameplayState(e_GameplayStates.Playing);
    }


    //Simply to start the PutBackFlippedCardsIntoDeckCoroutine coroutine
    public void PutBackFlippedCards()
    {
        GameManager.Instance.StartCoroutine(PutBackFlippedCardsIntoDeckCoroutine());
    }
    //A coroutine used to put back all flipped cards into the deck pile
    private IEnumerator PutBackFlippedCardsIntoDeckCoroutine()
    {
        while (BoardManager.Instance.FlippedPile.CardsOnPile.Count != 0)
        {
            BoardManager.Instance.SwitchPile(BoardManager.Instance.DeckPile, BoardManager.Instance.FlippedPile.CardsOnPile[0], e_CardFaceOptions.FaceHidden, false);
        }

        yield return new WaitForSeconds(GameConfig.Instance.Cards.MoveFromToDuration);

        ShuffleDeckPile();
    }
    //Flip over the necessary amount of cards from the deck pile into the flipped pile
    public void FlipOverCards()
    {
        for (int i = 0; i < (int)GameConfig.Instance.Gameplay.DrawAmount; i++)
        {
            BoardManager.Instance.SwitchPile(BoardManager.Instance.FlippedPile, BoardManager.Instance.DeckPile.CardsOnPile[BoardManager.Instance.DeckPile.CardsOnPile.Count - 1], e_CardFaceOptions.FaceShown, false);
        }
    }

    #endregion
}
