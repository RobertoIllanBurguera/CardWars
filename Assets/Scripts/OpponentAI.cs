using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAI : MonoBehaviour
{
    [Header("Configuración de la IA")]
    public RectTransform opponentHandArea;
    public GameObject cardPrefab;
    public int initialHandSize = 5;
    public int cardsToDrawEachTurn = 3;
    public int maxWeightPerTurn = 3;
    public BoardManager boardManager;

    [Header("Sonido al colocar carta")]
    public AudioSource placementSound;

    private List<CardData> opponentDeck = new List<CardData>();
    private List<GameObject> opponentHandCards = new List<GameObject>();

    void Start()
    {
        opponentDeck = new List<CardData>(DeckManagerGlobal.Instance.GetSelectedOpponentDeck());
        RobarCartasIniciales();
    }

    void RobarCartasIniciales()
    {
        for (int i = 0; i < initialHandSize; i++)
        {
            RobarCarta();
        }
    }

    void RobarCarta()
    {
        if (opponentDeck.Count == 0)
            return;

        CardData originalData = opponentDeck[0];
        opponentDeck.RemoveAt(0);

        CardData clonedData = CloneCardData(originalData);

        GameObject cardGO = Instantiate(cardPrefab, opponentHandArea);

        CardDisplay display = cardGO.GetComponent<CardDisplay>();
        if (display != null)
            display.Setup(clonedData);

        CardDisplayReference refComp = cardGO.GetComponent<CardDisplayReference>();
        if (refComp == null)
            refComp = cardGO.AddComponent<CardDisplayReference>();

        refComp.cardData = clonedData;

        RectTransform cardRect = cardGO.GetComponent<RectTransform>();
        cardRect.localScale = Vector3.one;

        opponentHandCards.Add(cardGO);
    }

    public void DrawCardsForTurn()
    {
        for (int i = 0; i < cardsToDrawEachTurn; i++)
        {
            RobarCarta();
        }
    }

    public void JugarTurno()
    {
        StartCoroutine(JugarTurnoCoroutine());
    }

    IEnumerator JugarTurnoCoroutine()
    {
        int pesoDisponible = maxWeightPerTurn;
        List<GameObject> cartasParaJugar = new List<GameObject>(opponentHandCards);

        foreach (var cartaObj in cartasParaJugar)
        {
            if (pesoDisponible <= 0)
                break;

            if (cartaObj == null) continue;

            CardDisplayReference refComp = cartaObj.GetComponent<CardDisplayReference>();
            if (refComp != null && refComp.cardData != null)
            {
                CardData data = refComp.cardData;
                int coste = data.cost;

                if (coste <= pesoDisponible)
                {
                    bool colocada = false;

                    if (data.isTerrain)
                    {
                        colocada = boardManager.ColocarCartaEnTerrenoOponente(data);
                    }
                    else
                    {
                        colocada = boardManager.ColocarCartaEnCasillaSuperior(data);
                    }

                    if (colocada)
                    {
                        pesoDisponible -= coste;
                        opponentHandCards.Remove(cartaObj);

                        // 🔥 Nueva invocación inmediata
                        if (data.passiveAbility == PassiveAbilityType.InvocarAlEntrar && data.cartaInvocada != null)
                        {
                            BoardSlot slotLibre = boardManager.GetFreeOpponentSlot();
                            if (slotLibre != null)
                            {
                                GameObject invocada = boardManager.CrearCartaEnSlot(data.cartaInvocada, slotLibre.transform);
                                slotLibre.PlaceCardFromAI(invocada);
                            }
                        }

                        Destroy(cartaObj);

                        if (placementSound != null)
                            placementSound.Play();

                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.StartCombat();
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
