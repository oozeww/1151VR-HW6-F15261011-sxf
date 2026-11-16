
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public float dropSpeed = -0.03f;

    void Update()
    {
        // Make the item fall down
        transform.Translate(0, this.dropSpeed, 0);

        // Destroy item when it falls below the stage
        if (transform.position.y < -1.0f)
        {
            Destroy(gameObject);
        }
    }
}
