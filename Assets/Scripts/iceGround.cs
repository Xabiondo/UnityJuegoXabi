using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iceGround : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Assign the Ice PhysicsMaterial2D to the Collider2D for low friction
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            PhysicsMaterial2D iceMaterial = Resources.Load<PhysicsMaterial2D>("Ice");
            if (iceMaterial != null)
            {
                collider.sharedMaterial = iceMaterial;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
