using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    [Header("Referencia al panel que se debe mostrar/ocultar")]
    public GameObject panelInfo;

    // Llamado por el botón de "Info"
    public void AbrirPanel()
    {
        panelInfo.SetActive(true);
    }

    // Llamado por el botón de "Cerrar"
    public void CerrarPanel()
    {
        panelInfo.SetActive(false);
    }
}
