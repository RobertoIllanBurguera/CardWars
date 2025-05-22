using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public DeckManager playerDeckManager;
    public OpponentAI opponentAI;
    public CombatManager combatManager;

    public Button endTurnButton;
    public bool playerTurn = true;
    private bool combatStarted = false;

    void Start()
    {
        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }
    }

    void OnEndTurnClicked()
    {
        if (playerTurn && !combatStarted)
        {
            playerTurn = false;
            StartOpponentTurn();
        }
    }

    void StartOpponentTurn()
    {
        opponentAI.DrawCardsForTurn();
        opponentAI.JugarTurno();
        combatStarted = true;
    }

    public void StartCombat()
    {
        combatManager.IniciarCombate();
        combatStarted = false;
        Invoke(nameof(StartNextTurn), 2f);
    }

    void StartNextTurn()
    {
        playerTurn = true;
        playerDeckManager.DrawCard();
        playerDeckManager.DrawCard();
        playerDeckManager.DrawCard();
    }

    public void FinDelCombate()
    {
        playerTurn = true;
        combatStarted = false;
    }
}
