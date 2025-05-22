using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardZoom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GameObject zoomCard;
    private Image zoomImage;
    private float holdTime = 0.3f; // Tiempo para mantener pulsado
    private float holdCounter = 0f;
    private bool isHolding = false;
    private bool zoomStarted = false;

    void Update()
    {
        if (isHolding)
        {
            holdCounter += Time.deltaTime;
            if (holdCounter >= holdTime && !zoomStarted)
            {
                StartZoom();
                zoomStarted = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdCounter = 0f;
        zoomStarted = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        holdCounter = 0f;

        if (zoomCard != null)
        {
            Destroy(zoomCard);
            zoomCard = null;
        }
    }

    private void StartZoom()
    {
        if (zoomCard != null) return;

        Canvas canvas = GetComponentInParent<Canvas>();
        zoomCard = new GameObject("ZoomCard");
        zoomCard.transform.SetParent(canvas.transform, false);

        // Obtener el artwork desde el CardData asociado
        CardDisplayReference refComp = GetComponent<CardDisplayReference>();
        if (refComp == null)
            refComp = GetComponentInParent<CardDisplayReference>();

        Sprite artworkSprite = refComp != null ? refComp.cardData.artwork : null;

        zoomImage = zoomCard.AddComponent<Image>();
        zoomImage.sprite = artworkSprite;
        zoomImage.preserveAspect = true;

        RectTransform zoomRect = zoomCard.GetComponent<RectTransform>();
        zoomRect.anchorMin = new Vector2(0.5f, 0.5f);
        zoomRect.anchorMax = new Vector2(0.5f, 0.5f);
        zoomRect.pivot = new Vector2(0.5f, 0.5f);
        zoomRect.anchoredPosition = Vector2.zero;

        RectTransform originalRect = GetComponent<RectTransform>();
        Vector2 originalSize = originalRect.sizeDelta;
        zoomRect.sizeDelta = originalSize * 1.5f;
    }


}
