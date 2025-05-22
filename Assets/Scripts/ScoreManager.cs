using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Puntos")]
    public int playerWins = 0;
    public int opponentWins = 0;
    private int totalRounds = 0;

    [Header("Indicadores de rondas (círculos)")]
    public Image[] roundIndicators;
    public Color playerWinColor = Color.green;
    public Color opponentWinColor = Color.red;
    public Color defaultColor = Color.white;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddPlayerPoint()
    {
        if (totalRounds < roundIndicators.Length)
        {
            roundIndicators[totalRounds].color = playerWinColor;
        }
        playerWins++;
        totalRounds++;

        CheckVictory();
    }

    public void AddOpponentPoint()
    {
        if (totalRounds < roundIndicators.Length)
        {
            roundIndicators[totalRounds].color = opponentWinColor;
        }
        opponentWins++;
        totalRounds++;

        CheckVictory();
    }

    private void CheckVictory()
    {
        if (playerWins >= 3)
        {
            if (!GameManager.Instance.segundaPartida)
            {
                // Primera partida ganada
                Invoke(nameof(LoadVictoryScene), 1f);
            }
            else
            {
                // Segunda partida ganada = victoria final
                Invoke(nameof(LoadFinalVictoryScene), 1f);
            }
        }
        else if (opponentWins >= 3)
        {
            Invoke(nameof(LoadDefeatScene), 1f);
        }
    }

    private void LoadVictoryScene()
    {
        SceneManager.LoadScene("VictoryScene");
    }

    private void LoadFinalVictoryScene()
    {
        SceneManager.LoadScene("FinalVictoryScene");
    }

    private void LoadDefeatScene()
    {
        SceneManager.LoadScene("DefeatScene");
    }

    public void ResetAll()
    {
        playerWins = 0;
        opponentWins = 0;
        totalRounds = 0;

        if (roundIndicators != null)
        {
            foreach (var indicator in roundIndicators)
            {
                if (indicator != null)  // Comprobar si la imagen sigue viva
                    indicator.color = defaultColor;
            }
        }
    }

}
