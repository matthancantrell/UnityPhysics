using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_ColliderThing : MonoBehaviour
{
    // Start is called before the first frame update

        public bool OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Player has entered the trigger area
            return true;
        }
        return false;
    }
}
