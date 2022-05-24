using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRefs : MonoBehaviour
{
    [Header("Parameters")]

    [Tooltip("This card's value")]
    [SerializeField]
    private CardVariables.CardData m_CardData = new CardVariables.CardData();
    public CardVariables.CardData CardData => m_CardData;

    [Space]

    [Tooltip("If true, this card is front side up")]
    [SerializeField]
    private bool m_CardIsShowed = false;
    public bool CardIsShowed => CardIsShowed;


    [Header("References")]

    [Tooltip("A reference to this card's render Transform component")]
    [SerializeField]
    private Transform m_Render;

    [Space]

    [Tooltip("A reference to this card's render front face Transform component")]
    [SerializeField]
    private Transform m_FrontFace;

    [Tooltip("A reference to this card's render back face Transform component")]
    [SerializeField]
    private Transform m_BackFace;

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
    public void SwitchCardFaceShowing(bool i_SetFrontSideUp)
    {
        if (i_SetFrontSideUp == true)
        {
            m_Render.DOScaleX(1f, m_CardVars.FlipOverDuration).
                OnComplete(() => 
                {
                    m_CardIsShowed = true; 
                });

            DOVirtual.DelayedCall(m_CardVars.FlipOverDuration / 2f, () =>
            {
                m_FrontFace.gameObject.SetActive(true);
                m_BackFace.gameObject.SetActive(false);
            });
        }
        else
        {
            Debug.LogError("a");


            m_Render.DOScale(new Vector2(-1f, 1f), m_CardVars.FlipOverDuration).
                OnComplete(() =>
                {
                    Debug.LogError("b");

                    m_CardIsShowed = false;
                });

            DOVirtual.DelayedCall(m_CardVars.FlipOverDuration / 2f, () =>
            {
                Debug.LogError("c");

                m_FrontFace.gameObject.SetActive(false);
                m_BackFace.gameObject.SetActive(true);
            });
        }
    }

    #endregion
}
