using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public RectTransform handArea;
    public GameObject cardPrefab;
    public int numberOfInitialCards = 5;
    public float cardWidth = 200f;
    public float spacing = 10f;

    public GameObject selectedCard;

    private List<CardData> playerDeck = new List<CardData>();

    void Start()
    {
        playerDeck = new List<CardData>(DeckManagerGlobal.Instance.GetSelectedPlayerDeck());
        ShuffleDeck();

        DrawInitialHand();
        AdjustHandAreaSizeAndPosition();
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            CardData temp = playerDeck[i];
            int randomIndex = Random.Range(i, playerDeck.Count);
            playerDeck[i] = playerDeck[randomIndex];
            playerDeck[randomIndex] = temp;
        }
    }

    void DrawInitialHand()
    {
        for (int i = 0; i < numberOfInitialCards; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        if (playerDeck.Count == 0) return;

        CardData originalData = playerDeck[0];
        playerDeck.RemoveAt(0);

        CardData clonedData = CloneCardData(originalData);

        GameObject newCard = Instantiate(cardPrefab, handArea);
        RectTransform cardRect = newCard.GetComponent<RectTransform>();
        cardRect.localScale = Vector3.one;
        cardRect.anchoredPosition3D = Vector3.zero;
        cardRect.localRotation = Quaternion.identity;

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        if (cardDisplay != null)
        {
            cardDisplay.Setup(clonedData);
        }

        CardDisplayReference refComp = newCard.GetComponent<CardDisplayReference>();
        if (refComp == null)
            refComp = newCard.AddComponent<CardDisplayReference>();

        refComp.cardData = clonedData;

        CardOwner cardOwner = newCard.GetComponent<CardOwner>();
        if (cardOwner != null)
        {
            cardOwner.esDelJugador = true;
        }

        CardSelectable selectable = newCard.GetComponent<CardSelectable>();
        if (selectable == null)
            newCard.AddComponent<CardSelectable>();

        Outline outline = newCard.GetComponent<Outline>();
        if (outline == null)
        {
            outline = newCard.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(5, 5);
        }
        outline.enabled = false;
    }

    void AdjustHandAreaSizeAndPosition()
    {
        float totalWidth = numberOfInitialCards * cardWidth + (numberOfInitialCards - 1) * spacing;
        float height = 350f;

        handArea.sizeDelta = new Vector2(totalWidth, height);

        RectTransform viewport = handArea.parent.GetComponent<RectTransform>();
        float viewportWidth = viewport.rect.width;

        if (totalWidth < viewportWidth)
        {
            float offsetX = (viewportWidth - totalWidth) / 2f;
            handArea.anchoredPosition = new Vector2(offsetX, 0f);
        }
        else
        {
            handArea.anchoredPosition = new Vector2(0f, 0f);
        }
    }

    private CardData CloneCardData(CardData original)
    {
        CardData clone = ScriptableObject.CreateInstance<CardData>();

        clone.cardName = original.cardName;
        clone.attack = original.attack;
        clone.health = original.health;
        clone.speed = original.speed;
        clone.type1 = original.type1;
        clone.type2 = original.type2;
        clone.passiveAbility = original.passiveAbility;
        clone.abilityTarget = original.abilityTarget;
        clone.cost = original.cost;
        clone.abilityDetails = original.abilityDetails;
        clone.artwork = original.artwork;
        clone.isTerrain = original.isTerrain;
        clone.terrainType = original.terrainType;
        clone.buffAttackAmount = original.buffAttackAmount;
        clone.buffHealthAmount = original.buffHealthAmount;
        clone.cartaInvocada = original.cartaInvocada;

        return clone;
    }
}
