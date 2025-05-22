using UnityEngine;
using UnityEngine.EventSystems;

public class ExpandOnClick : MonoBehaviour, IPointerClickHandler
{
    public RectTransform scrollViewTransform;  // El mismo Scroll View
    public float collapsedY = 50f;
    public float expandedY = 200f;

    public float collapsedHeight = 120f;
    public float expandedHeight = 300f;

    public AudioClip expandSound;
    private AudioSource audioSource;

    private bool isExpanded = false;

    void Start()
    {
        // Añade AudioSource si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        SetCollapsed();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Si haces clic directamente sobre el fondo del Scroll View
        if (eventData.pointerEnter != gameObject) return;

        isExpanded = !isExpanded;

        if (isExpanded)
            SetExpanded();
        else
            SetCollapsed();

        if (expandSound != null)
        {
            audioSource.PlayOneShot(expandSound);
        }
    }

    private void SetCollapsed()
    {
        scrollViewTransform.anchoredPosition = new Vector2(0, collapsedY);
        scrollViewTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, collapsedHeight);
    }

    private void SetExpanded()
    {
        scrollViewTransform.anchoredPosition = new Vector2(0, expandedY);
        scrollViewTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, expandedHeight);
    }
}
