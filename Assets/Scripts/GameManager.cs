using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public OpponentAI opponentAI;

    public bool segundaPartida = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
