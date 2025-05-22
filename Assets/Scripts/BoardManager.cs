using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Casillas de la IA")]
    public BoardSlot[] opponentSlots;
    public BoardSlot terrenoOponenteSlot;

    [Header("Casillas del Jugador")]
    public BoardSlot[] playerSlots;
    public BoardSlot terrenoJugadorSlot;

    private bool terrenoOcupadoOponente = false;
    private bool terrenoOcupadoJugador = false;

    public CardData terrenoActivoJugador = null;
    public CardData terrenoActivoOponente = null;

    public bool CasillaTerrenoOcupadaOponente()
    {
        return terrenoOcupadoOponente;
    }

    public bool CasillaTerrenoOcupadaJugador()
    {
        return terrenoOcupadoJugador;
    }

    public GameObject CrearCartaEnSlot(CardData cartaData, Transform parentSlot)
    {
        GameObject nuevaCarta = Instantiate(DeckManagerGlobal.Instance.cardPrefab, parentSlot, false);

        RectTransform rt = nuevaCarta.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        nuevaCarta.transform.localScale = Vector3.one;

        CardDisplay display = nuevaCarta.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.Setup(cartaData);
        }

        CardDisplayReference refComp = nuevaCarta.GetComponent<CardDisplayReference>();
        if (refComp == null)
            refComp = nuevaCarta.AddComponent<CardDisplayReference>();

        refComp.cardData = cartaData;

        return nuevaCarta;
    }

    public bool ColocarCartaEnCasillaSuperior(CardData carta)
    {
        foreach (BoardSlot slot in opponentSlots)
        {
            if (!slot.HasCard())
            {
                GameObject cartaCreada = CrearCartaEnSlot(carta, slot.transform);
                slot.PlaceCardFromAI(cartaCreada);
                return true;
            }
        }
        return false;
    }

    public bool ColocarCartaEnTerrenoOponente(CardData carta)
    {
        if (!terrenoOcupadoOponente && terrenoOponenteSlot != null && !terrenoOponenteSlot.HasCard())
        {
            GameObject cartaCreada = CrearCartaEnSlot(carta, terrenoOponenteSlot.transform);
            terrenoOponenteSlot.PlaceCardFromAI(cartaCreada);
            terrenoOcupadoOponente = true;
            terrenoActivoOponente = carta; // Guardar el terreno activo
            return true;
        }
        return false;
    }

    public bool ColocarCartaEnTerrenoJugador(CardData carta)
    {
        if (!terrenoOcupadoJugador && terrenoJugadorSlot != null && !terrenoJugadorSlot.HasCard())
        {
            GameObject cartaCreada = CrearCartaEnSlot(carta, terrenoJugadorSlot.transform);
            terrenoJugadorSlot.PlaceCardFromAI(cartaCreada);
            terrenoOcupadoJugador = true;
            terrenoActivoJugador = carta; // Guardar el terreno activo
            return true;
        }
        return false;
    }

    public BoardSlot GetFreeOpponentSlot()
    {
        foreach (BoardSlot slot in opponentSlots)
        {
            if (!slot.HasCard())
                return slot;
        }
        return null;
    }

    public BoardSlot GetFreePlayerSlot()
    {
        foreach (BoardSlot slot in playerSlots)
        {
            if (!slot.HasCard())
                return slot;
        }
        return null;
    }
}
