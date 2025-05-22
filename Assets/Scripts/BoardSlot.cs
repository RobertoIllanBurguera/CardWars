using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardSlot : MonoBehaviour, IPointerClickHandler
{
    public enum SlotOwner { None, Player, Opponent, Terrain }
    public SlotOwner slotOwner = SlotOwner.None;

    private GameObject placedCard = null;
    public AudioSource placementSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HasCard()) return;

        DeckManager deckManager = FindObjectOfType<DeckManager>();
        BoardManager boardManager = FindObjectOfType<BoardManager>();
        CombatManager combatManager = FindObjectOfType<CombatManager>();

        if (deckManager != null && deckManager.selectedCard != null)
        {
            CardDisplay cardDisplay = deckManager.selectedCard.GetComponent<CardDisplay>();

            if (cardDisplay != null && cardDisplay.cardDataReference != null)
            {
                bool cartaEsTerreno = cardDisplay.cardDataReference.isTerrain;
                bool slotEsTerreno = slotOwner == SlotOwner.Terrain;

                if (cartaEsTerreno && !slotEsTerreno)
                {
                    Debug.LogWarning("Esta carta es un terreno, solo puede ir en una casilla de terreno.");
                    return;
                }
                else if (!cartaEsTerreno && slotEsTerreno)
                {
                    Debug.LogWarning("Esta carta NO es un terreno, no puede ir en una casilla de terreno.");
                    return;
                }

                if (!cartaEsTerreno && combatManager != null)
                {
                    if (combatManager.manaActual < cardDisplay.cardDataReference.cost)
                    {
                        CombatLogUI logUI = FindObjectOfType<CombatLogUI>();
                        if (logUI != null)
                        {
                            logUI.AddLog("No tienes suficiente peso para jugar esta carta.");
                        }
                        return;
                    }
                    combatManager.manaActual -= cardDisplay.cardDataReference.cost;
                }

                GameObject originalCard = deckManager.selectedCard;
                GameObject cardToPlace = Instantiate(originalCard, this.transform);

                CardDisplay originalDisplay = originalCard.GetComponent<CardDisplay>();
                CardDisplay newDisplay = cardToPlace.GetComponent<CardDisplay>();

                if (originalDisplay != null && newDisplay != null)
                {
                    CardData clonedData = CloneCardData(originalDisplay.cardDataReference);
                    clonedData.haResucitado = false; // Resetear resurrección si es una carta nueva
                    newDisplay.Setup(clonedData);

                    if (clonedData.isTerrain)
                    {
                        CombatLogUI logUI = FindObjectOfType<CombatLogUI>();
                        if (logUI != null)
                        {
                            logUI.AddLog("Terreno activado: " + clonedData.terrainType.ToString());
                        }
                        if (slotOwner == SlotOwner.Terrain)
                        {
                            boardManager.terrenoActivoJugador = clonedData;
                        }
                    }
                    else
                    {
                        CardData terreno = boardManager.terrenoActivoJugador;
                        if (terreno != null && (clonedData.type1 == terreno.type1 || clonedData.type2 == terreno.type1))
                        {
                            clonedData.attack += terreno.buffAttackAmount;
                            clonedData.health += terreno.buffHealthAmount;
                            CombatLogUI logUI = FindObjectOfType<CombatLogUI>();
                            if (logUI != null)
                            {
                                logUI.AddLog(clonedData.cardName + " gana +" + terreno.buffAttackAmount + " ATK y +" + terreno.buffHealthAmount + " HP por " + terreno.cardName);
                            }
                        }
                    }

                    if (clonedData.passiveAbility == PassiveAbilityType.DestruirTerreno)
                    {
                        if (boardManager.terrenoOponenteSlot != null && boardManager.terrenoOponenteSlot.HasCard())
                        {
                            GameObject terrenoEnemy = boardManager.terrenoOponenteSlot.GetPlacedCard();
                            if (terrenoEnemy != null)
                            {
                                Destroy(terrenoEnemy);
                                boardManager.terrenoOponenteSlot.ClearSlot();
                                boardManager.terrenoActivoOponente = null;

                                CombatLogUI logUI = FindObjectOfType<CombatLogUI>();
                                if (logUI != null)
                                {
                                    logUI.AddLog("El terreno enemigo ha sido destruido.");
                                }
                            }
                        }
                    }
                }

                RectTransform cardRect = cardToPlace.GetComponent<RectTransform>();
                cardRect.SetParent(this.transform, false);
                cardRect.anchorMin = new Vector2(0.5f, 0.5f);
                cardRect.anchorMax = new Vector2(0.5f, 0.5f);
                cardRect.pivot = new Vector2(0.5f, 0.5f);
                cardRect.anchoredPosition = Vector2.zero;

                RectTransform slotRect = GetComponent<RectTransform>();
                float scaleFactor = Mathf.Min(slotRect.rect.width / cardRect.rect.width, slotRect.rect.height / cardRect.rect.height);
                cardRect.localScale = Vector3.one * scaleFactor;

                placedCard = cardToPlace;
                deckManager.selectedCard = null;

                var outline = placedCard.GetComponent<Outline>();
                if (outline != null) outline.enabled = false;

                if (placementSound != null)
                    placementSound.Play();

                Destroy(originalCard);

                if (cardDisplay.cardDataReference.passiveAbility == PassiveAbilityType.InvocarAlEntrar && cardDisplay.cardDataReference.cartaInvocada != null)
                {
                    InvocarCartaAdicional(cardDisplay.cardDataReference.cartaInvocada);
                }
            }
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

    public GameObject GetPlacedCard() => placedCard;

    public void ClearSlot()
    {
        if (placedCard != null)
        {
            Destroy(placedCard);
            placedCard = null;
        }
    }

    public bool HasCard() => placedCard != null;
    public bool EsTerrenoSlot() => slotOwner == SlotOwner.Terrain;

    public void PlaceCardFromAI(GameObject carta)
    {
        placedCard = carta;
        RectTransform cardRect = carta.GetComponent<RectTransform>();
        cardRect.SetParent(this.transform, false);
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        RectTransform slotRect = GetComponent<RectTransform>();
        float scaleFactor = Mathf.Min(slotRect.rect.width / cardRect.rect.width, slotRect.rect.height / cardRect.rect.height);
        carta.transform.localScale = Vector3.one * scaleFactor;
    }

    private void InvocarCartaAdicional(CardData cartaInvocada)
    {
        BoardManager boardManager = FindObjectOfType<BoardManager>();
        BoardSlot slotLibre = slotOwner == SlotOwner.Player
            ? boardManager.GetFreePlayerSlot()
            : boardManager.GetFreeOpponentSlot();

        if (slotLibre != null)
        {
            GameObject cartaCreada = boardManager.CrearCartaEnSlot(cartaInvocada, slotLibre.transform);
            slotLibre.PlaceCardFromAI(cartaCreada);

            CombatLogUI logUI = FindObjectOfType<CombatLogUI>();
            if (logUI != null)
            {
                logUI.AddLog(cartaInvocada.cardName + " ha sido invocado.");
            }
        }
    }
}

