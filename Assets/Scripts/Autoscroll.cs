using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    private ScrollRect scrollRect;
    private bool autoScrollEnabled = true;
    private bool isDragging = false;
    private float delayAfterRelease = 1f;
    private float releaseTimer = 0f;

    void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    void Update()
    {
        if (scrollRect == null) return;

        // Detectar si está arrastrando
        if (Input.GetMouseButton(0))
        {
            isDragging = true;
            autoScrollEnabled = false;
            releaseTimer = 0f; // Reiniciar temporizador
        }
        else
        {
            if (isDragging)
            {
                releaseTimer += Time.deltaTime;

                if (releaseTimer >= delayAfterRelease)
                {
                    autoScrollEnabled = true;
                    isDragging = false;
                }
            }
        }

        if (autoScrollEnabled)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
