using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button deck1Button;
    public Button deck2Button;
    public Button deck3Button;
    public Button playButton;
    public AudioSource buttonClickSound; 

    private bool deckSelected = false;

    private void Start()
    {
        playButton.interactable = false;

        deck1Button.onClick.AddListener(() => SelectDeck(1));
        deck2Button.onClick.AddListener(() => SelectDeck(2));
        deck3Button.onClick.AddListener(() => SelectDeck(3));
        playButton.onClick.AddListener(PlayGame);
    }

    private void SelectDeck(int deckNumber)
    {
        if (buttonClickSound != null)
            buttonClickSound.Play(); 

        DeckManagerGlobal.Instance.SelectPlayerDeck(deckNumber);
        DeckManagerGlobal.Instance.SelectOpponentDeck();

        deckSelected = true;
        playButton.interactable = true;

        Debug.Log("Mazo " + deckNumber + " seleccionado.");
    }

    private void PlayGame()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play(); 

        if (deckSelected)
        {
            SceneManager.LoadScene("Battle 1"); 
        }
    }
}
