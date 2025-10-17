using UnityEngine;

public class CaveEntranceTrigger : MonoBehaviour
{
    public GameObject caveOverlay;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Solo lo activamos si aún no está activo
            if (caveOverlay != null && !caveOverlay.activeSelf)
            {
                caveOverlay.SetActive(true);
                // Opcional: desactivar parallax para rendimiento/coherencia
                // ParallaxManager.Instance.Disable();
            }
        }
    }

    // ¡NO HAY OnTriggerExit2D!
    // No queremos desactivar al salir del trigger de entrada.
}