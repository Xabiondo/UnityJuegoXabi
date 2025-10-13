using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El arquero
    public float smoothSpeed = 0.125f;
    public Vector3 baseOffset = new Vector3(0, 5, -10); // Offset base (arriba y atrás)
    
    [Range(0, 1)]
    public float heightInfluence = 0.365f; // Cuánto afecta la altura del jugador al offset

    void LateUpdate()
    {
        if (target == null) return;

        // Ajustar el offset Y según la altura del jugador
        float adjustedOffsetY = baseOffset.y - (target.position.y * heightInfluence);
        Vector3 dynamicOffset = new Vector3(baseOffset.x, adjustedOffsetY, baseOffset.z);

        Vector3 desiredPosition = target.position + dynamicOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}