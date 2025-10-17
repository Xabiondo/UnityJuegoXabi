using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitCave : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject caveOverlay;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            caveOverlay.SetActive(false);
        }

    }




}
