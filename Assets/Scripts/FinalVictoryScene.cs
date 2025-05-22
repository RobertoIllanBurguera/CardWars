using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalVictorySceneManager : MonoBehaviour
{
    public void VolverAlMenu()
    {
        {
            DeckManagerGlobal.Instance.partidasGanadas = 0;
            SceneManager.LoadScene("MainMenu");
        }
        
        GameManager.Instance.segundaPartida = false;
        ScoreManager.Instance.ResetAll();

        
        SceneManager.LoadScene("MainMenu");
    }
}
