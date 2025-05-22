using UnityEngine;
using TMPro;

public class CombatLogUI : MonoBehaviour
{
    public TextMeshProUGUI logText;

    public void AddLog(string message)
    {
        logText.text += message + "\n";
    }


}
