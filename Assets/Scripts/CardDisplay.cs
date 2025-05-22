using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("Referencias de Texto")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI costText;

    [Header("Imagen de la Carta")]
    public Image artworkImage;  // <<<<<< NUEVA referencia para el dibujo

    [Header("Datos de la Carta")]
    public CardData cardDataReference;  // <<<<< NUEVA referencia importante

    // Configura toda la carta
    public void Setup(CardData card)
    {
        cardDataReference = card;  // Guardamos la referencia
        nameText.text = card.cardName;
        attackText.text = "ATK: " + card.attack.ToString();
        healthText.text = "HP: " + card.health.ToString();
        speedText.text = "SPD: " + card.speed.ToString();
        costText.text = "Coste: " + card.cost.ToString();

        if (artworkImage != null && card.artwork != null)
        {
            artworkImage.sprite = card.artwork; // 💥 ESTA LÍNEA ES LA CLAVE
        }
    }

}
