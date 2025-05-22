using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatSceneManager : MonoBehaviour
{
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
