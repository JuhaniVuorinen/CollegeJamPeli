using System.Collections.Generic;
using UnityEngine;

public class CatDetection : MonoBehaviour
{
    // List to store detected colliders (e.g., the player)
    public List<Collider2D> detectedColliders = new List<Collider2D>();

    // When a collider enters the detection zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!detectedColliders.Contains(other))
        {
            detectedColliders.Add(other);
        }
    }

    // When a collider exits the detection zone
    private void OnTriggerExit2D(Collider2D other)
    {
        if (detectedColliders.Contains(other))
        {
            detectedColliders.Remove(other);
        }
    }
}
