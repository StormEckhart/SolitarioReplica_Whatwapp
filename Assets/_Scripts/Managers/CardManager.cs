using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : SingletonMonoBehaviourManager<CardManager>
{
    [System.Serializable]
    public class ColliderScriptDictionary : UnitySerializedDictionary<CardVariables.CardData, CardRefs> { }



    [Header("References")]

    [Tooltip("The array holding all pooled cards (the whole 52 card by default)")]
    [SerializeField]
    private CardRefs[] m_Deck = new CardRefs[52];
    public CardRefs[] Deck => m_Deck;

    [Tooltip("The parent holding all pooled cards")]
    [SerializeField]
    private Transform m_DeckParent;

    [Space]

    [Tooltip("The card prefab")]
    [SerializeField]
    private CardRefs m_CardPrefab;



    //Shortcut by code to create all cards in deck (instead of manually)
    [Button]
    [SerializeField]
    private void SetupCards()
    {
        int l_ValueIndex = (int)e_CardValues.Ace;
        int l_SuitIndex = (int)e_CardSuits.Diamonds;


        for (int i = 0; i < m_Deck.Length; i++)
        {
            if (m_Deck[i] != null) DestroyImmediate(m_Deck[i].gameObject);



            CardRefs l_CurrentCardRefs = Instantiate(m_CardPrefab, m_DeckParent);


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


            l_CurrentCardRefs.SwitchCardFaceShowing(false);


            m_Deck[i] = l_CurrentCardRefs;
        }
    }

    //Shuffling the deck using the Fisher-Yates shuffle
    private void ShuffleDeck()
    {
        System.Random l_Random = new System.Random();

        for (int i = 0; i < m_Deck.Length; i++)
        {
            int j = l_Random.Next(i, m_Deck.Length);

            CardRefs l_TempCard = m_Deck[i];

            m_Deck[i] = m_Deck[j];
            m_Deck[j] = l_TempCard;
        }
    }

    public void DealDeck()
    {
        ShuffleDeck();

        GameManager.UpdateGameplayState(e_GameplayStates.DealingDeck);
    }
}
