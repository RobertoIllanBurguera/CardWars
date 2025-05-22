using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSelectable : MonoBehaviour, IPointerUpHandler
{
    private DeckManager deckManager;
    private Outline outline;
    private CardOwner cardOwner;

    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
        outline = GetComponent<Outline>();
        cardOwner = GetComponent<CardOwner>();

        if (outline != null)
        {
            outline.enabled = false; 
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (cardOwner != null && !cardOwner.esDelJugador)
            return;

        Debug.Log("Carta LIBERADA: " + gameObject.name); 

        if (deckManager != null)
        {
            if (deckManager.selectedCard != null && deckManager.selectedCard != gameObject)
            {
                Outline oldOutline = deckManager.selectedCard.GetComponent<Outline>();
                if (oldOutline != null)
                {
                    oldOutline.enabled = false;
                }
            }

            deckManager.selectedCard = gameObject;

            if (outline != null)
            {
                outline.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("DeckManager no encontrado.");
        }
    }
}
