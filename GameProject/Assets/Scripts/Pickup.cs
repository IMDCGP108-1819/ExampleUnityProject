using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    // OnTriggerEnter works similarly to OnCollisionEnter (see Bullet.cs or PlayerControl.cs)
    // but triggers do not cause a physical response - they are not solid.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the object that has entered the trigger is not tagged as "Player"
        if (!collision.gameObject.CompareTag("Player"))
            return; // we return from the function early, which will stop the rest of the code running

        // we disable the pickup
        gameObject.SetActive(false);

        // grab a reference to the PlayerControl script
        PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();

        // make sure that reference is valid
        if ( player )
        {
            // if it is, we call the SetScore function with a value of 1.
            player.SetScore(1);
        }
    }
}
