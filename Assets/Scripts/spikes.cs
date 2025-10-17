using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        ArcherController player = other.GetComponent<ArcherController>();
        
        if (player != null) 
        {
            player.TakeDamage(1);
        }
    }
}