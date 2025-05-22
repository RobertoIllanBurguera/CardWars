using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySceneManager : MonoBehaviour
{
    public void ContinuarCampa�a()
    {
        // Aumentar progreso (cu�ntas partidas ha ganado el jugador)
        DeckManagerGlobal.Instance.AumentarProgresoCampa�a();

        // Si ya ha ganado dos partidas, mostrar victoria final
        if (DeckManagerGlobal.Instance.partidasGanadas >= 2)
        {
            SceneManager.LoadScene("FinalVictoryScene");
        }
        else
        {
            // Indicar que empieza la segunda partida
            GameManager.Instance.segundaPartida = true;

            // Resetear el marcador
            ScoreManager.Instance.ResetAll();

            // Elegir nuevo mazo de la IA (el que quede)
            DeckManagerGlobal.Instance.SelectOpponentDeck();

            // Cargar de nuevo la escena del juego
            SceneManager.LoadScene("Battle 1");
        }
    }

}
