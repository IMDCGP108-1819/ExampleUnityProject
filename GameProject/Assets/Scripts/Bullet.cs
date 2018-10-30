using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Rigidbody2D rigidBody;
    public float LaunchForce;
    public float LifeTime = 3f;
    public float ExplosionRadius = 3f;
    public float ExplosiveForce = 1f;
    private float StartTime;

	// Use this for initialization
	void Start () {
        // Store a reference to the rigidbody component that sits alongside the bullet script.
        rigidBody = GetComponent<Rigidbody2D>();  
	}

    private void OnEnable()
    {
        // check if the rigidbody reference is valid, if not
        if ( rigidBody == null )
            rigidBody = GetComponent<Rigidbody2D>(); // we store a reference - this will only happen if OnEnable is called before Start, which *sometimes* happens.

        // once the bullet is enabled (using SetActive(true)) we want to add force to the bullet.
        rigidBody.AddForce(transform.right * LaunchForce, ForceMode2D.Impulse);

        // we store the time at the moment the bullet is enabled
        // Time.time represents the number of seconds that have elapsed since the game was loaded
        StartTime = Time.time;
    }

    private void Update()
    {
        // if the current time is greater than the start time + the life time of the bullet
        if (Time.time >= StartTime + LifeTime)
        {
            // the bullet is set to inactive, so it vanishes.
            gameObject.SetActive(false);
        }
    }

    // OnCollisionEnter2D is called when an object with a rigidbody and a collider hits another object with a collider (only one of the objects needs a rigidbody, both need a collider).
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // as soon as the bullet hits anything, we deactivate it.
        gameObject.SetActive(false);
        
        // here we make a circlecast - so we're checking for objects in a specific radius around the bullet
        // CircleCastAll returns an array (a list) of all the objects inside the circle.
        // the foreach format lets us work through that list, so we influence all of the objects.
        foreach (var item in Physics2D.CircleCastAll(transform.position, ExplosionRadius, transform.right))
        {
            // check that the item we're looking at has a rigidbody
            if ( item.rigidbody)
            {
                // if it does, add a force to it
                // by subtracting the items position from the position of the bullet, we get a direction away from the bullet.
                item.rigidbody.AddForce((item.transform.position - transform.position) * ExplosiveForce, ForceMode2D.Impulse);
            }
        }
        
    }
}
