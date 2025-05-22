using System.Collections.Generic;
using UnityEngine;

public class DeckManagerGlobal : MonoBehaviour
{
    public static DeckManagerGlobal Instance;

    [Header("Mazos disponibles")]
    public List<CardData> deck1 = new List<CardData>();
    public List<CardData> deck2 = new List<CardData>();
    public List<CardData> deck3 = new List<CardData>();

    [Header("Prefab de carta")]
    public GameObject cardPrefab;

    private List<CardData> selectedPlayerDeck;
    private List<CardData> selectedOpponentDeck;
    private List<CardData> lastOpponentDeck;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SelectPlayerDeck(int deckNumber)
    {
        switch (deckNumber)
        {
            case 1:
                selectedPlayerDeck = new List<CardData>(deck1);
                break;
            case 2:
                selectedPlayerDeck = new List<CardData>(deck2);
                break;
            case 3:
                selectedPlayerDeck = new List<CardData>(deck3);
                break;
            default:
                Debug.LogError("Número de mazo inválido: " + deckNumber);
                break;
        }
    }

    public void SelectOpponentDeck()
    {
        List<List<CardData>> availableDecks = new List<List<CardData>>() { deck1, deck2, deck3 };

        if (selectedPlayerDeck != null)
        {
            if (EsMismoMazo(selectedPlayerDeck, deck1))
                availableDecks.Remove(deck1);
            else if (EsMismoMazo(selectedPlayerDeck, deck2))
                availableDecks.Remove(deck2);
            else if (EsMismoMazo(selectedPlayerDeck, deck3))
                availableDecks.Remove(deck3);
        }

        if (lastOpponentDeck != null)
        {
            availableDecks.Remove(lastOpponentDeck);
        }

        if (availableDecks.Count > 0)
        {
            int randomIndex = Random.Range(0, availableDecks.Count);
            selectedOpponentDeck = new List<CardData>(availableDecks[randomIndex]);
            lastOpponentDeck = selectedOpponentDeck;
        }
        else
        {
            Debug.LogWarning("No quedan mazos disponibles para la IA.");
        }
    }

    private bool EsMismoMazo(List<CardData> mazoA, List<CardData> mazoB)
    {
        if (mazoA.Count != mazoB.Count) return false;
        for (int i = 0; i < mazoA.Count; i++)
        {
            if (mazoA[i] != mazoB[i]) return false;
        }
        return true;
    }

    public List<CardData> GetSelectedPlayerDeck()
    {
        return selectedPlayerDeck;
    }

    public List<CardData> GetSelectedOpponentDeck()
    {
        return selectedOpponentDeck;
    }

    public void ResetMazos()
    {
        lastOpponentDeck = null;
    }
    public int partidasGanadas = 0;

    public void AumentarProgresoCampaña()
    {
        partidasGanadas++;
    }

}
