using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {
    private int Score;

    // the Range attribute modifies how this value appears in the Inspector
    // it locks the value to the min / max settings and provides a slider to set the value
    [Range(0.25f, 5.0f)]
    public float MoveSpeedModifier = 0.25f; // MoveSpeedModifier defaults to 0.25
    public Transform jumpCheckPosition; // jumpCheckPosition stores the transform (position, scale, rotation values) of a point in space.

    [Range(0.25f, 1.0f)]
    public float JumpCheckRange = 0.5f; // jumpCheckRange stores how far from jumpCheckPosition we should check for a platform beneath the player.

    public float CurrentHealth; // CurrentHealth is the current health of the player (in theory, 0 - 100)
    public float MaxHealth; // MaxHealth is the maximum possible health value
    public float HealthRegenRate; // HealthRegenRate is the rate at which CurrentHealth changes until it gets to MaxHealth
    public Text PlayerHealthDisplay; // PlayerHealthDisplay is a reference to a text object in the scene that is part of the UI canvas.

    // The SerializeField attribute allows us to make a private property accessible in the editor.
    [SerializeField]
    private Rigidbody2D rigidBody; // rigidBody is a private property that provides us with a reference to the players RigidBody2D. A private property is one that is not accessible outside the owning class.
    public float moveForce = 15; // moveForce is how much force applied each frame when we move using the physics system
    public float bulletImpactForce = 5; // bulletImpactForce is the amount of force applied when we fire using the raycast method.
    public Transform firePosition; // firePosition is the position from which the bullets should be spawned

    public GameObject bulletPrefab; // this is the prefab from the Project view (not in the scene) that we will use as a blueprint for all bullets.

    public GameObject[] bulletList; // bulletList stores the list of instantiated bullets that we will use as our bullet pool.
    public int MaxBullets = 10; // MaxBullets specifies the maximum number of bullets we have in our pool

    private float FireTime; // FireTime is the last point in time we fired
    public float RateOfFire; // RateOfFire represents how quickly the player can fire.

    public bool CanFire = true; // CanFire is the flag that says whether we can fire or not (based on RateOfFire)

    // SetScore is a public function (so it's accessible elsewhere - specifically, it's used in Pickup.cs)
    // it takes a single value; scoreToAdd
    public void SetScore(int scoreToAdd)
    {
        // The += combination is the same as doing Score = Score + scoreToAdd, it's just quicker to write.
        Score += scoreToAdd;
    }

	// Use this for initialization
	void Start () {

        // we need to make sure that FireTime is set to a suitable value as soon as possible
        FireTime = Time.time;

        // We update the player health HUD element. The use of .ToString() here converts the int that CurrentHealth represents into a suitable string version.
        PlayerHealthDisplay.text = CurrentHealth.ToString();

        // we need to set how big the bullet list - the "new" keyword reserves enough space for us to fit MaxBullet number of GameObjects in the list
        bulletList = new GameObject[MaxBullets];

        // we use a for loop here; for loops are made up of 3 elements, separated by semi-colons (;)
        // 1. the "initialization", here we declare a variable called i and assign it a value of 0
        // 2. the "condition" that controls how long the loop will run for; this loop will run while i < MaxBullets is true
        // 3. the "afterthought"; after each time through the loop, this happens - here, we add 1 to the value of i.
        for (int i = 0; i < MaxBullets; i++)
        {
            // we access each entry in the bulletList array with an index (starting from 0, up to 9 (1 less than MaxBullets))
            // here we instantiate a gameobject using the specified prefab and set an initial position and rotation.
            bulletList[i] = Instantiate(bulletPrefab, firePosition.position, transform.rotation);
            // we want the bullets to start inactive
            bulletList[i].SetActive(false);
        }

        // Coroutines are a way of running functions outside of the normal "update" loop in Unity.
        StartCoroutine(RegenerateHealth());
	}

    // GetBullet returns the first available bullet from the bullet pool
    private GameObject GetBullet()
    {
        // loop from 0 to MaxBullets, adding 1 each time we complete a loop
        for (int i = 0; i < MaxBullets; i++)
        {
            // if the bullet in bulletList at position i is not active in the scene
            if (!bulletList[i].activeSelf)
            {
                // then return it.
                // returning will stop any further checks against other bullets
                return bulletList[i];
            }
        }

        // if we reach here, there are no available bullets and we cannot fire.
        return null;
    }

    // Update is run as often as possible - so it varies with the frame rate
    private void Update()
    {
        // if the fire button is down and the Can fire flag is true
        if ( Input.GetButton("Fire1") && CanFire)
        {
            // we start the firing corouting
            StartCoroutine(FireBullet());
        }

        // Raycast fires an invisible line out in a specific direction (and distance) looking for anything with a collider
        // here we are firing a ray downwards (inverse of vector2.up) at a distance of 0.5 units. The start position is specified by the "jumpCheckPosition" variable
        /*RaycastHit2D rayHit = Physics2D.Raycast(jumpCheckPosition.position, -Vector2.up, 0.5f);

        // if the raycast has hit anything at all
        if ( rayHit )
        {
            // then we set the "canjump" flag to true
            canJump = true;
        }*/

        // move by setting the transform position directly.
        /*float newXPosition = transform.position.x + (Input.GetAxis("Horizontal") * MoveSpeedModifier) * Time.deltaTime;
        float newYPosition = transform.position.y + (Input.GetAxis("Vertical") * MoveSpeedModifier) * Time.deltaTime;

        // update the players position directly using the newly built vector.
        transform.position = new Vector3(newXPosition, newYPosition, 0.0f);*/
    }

    // public function that can be called by a HUD button (or other external file)
    public void OnButtonClicked()
    {
        // Debug.Log will print whatever string is provided to the console in the Unity Editor.
        Debug.Log("Clicked!");
    }

    // Coroutines must return something with a type of IEnumerator
    private IEnumerator RegenerateHealth()
    {
        // we enter an infinite loop here; if we did not yield later, this would freeze up our game (and the editor).
        while(true)
        {
            // the yield keyword here makes the computer from running this function for the specified amount of time.
            // in this case, we say it should wait for however many seconds is specified in the HealthRegenRate variable
            yield return new WaitForSeconds(HealthRegenRate);

            // We update the CurrentHealth here - Mathf.Min returns the smaller value of the two provided - so we'll never set CurrentHealth to be greater than MaxHealth.
            CurrentHealth = Mathf.Min(CurrentHealth + 1, MaxHealth);

            // update the HUD element
            PlayerHealthDisplay.text = CurrentHealth.ToString();
        }
    }

    // this is the coroutine function that controls firing
    private IEnumerator FireBullet()
    {
        // unlike above, this does not start an infinite loop

        // we instantly set CanFire to false, to ensure we're not allowed to fire again until this function says so.
        CanFire = false;
        // we get a spare bullet from the pool
        GameObject newBullet = GetBullet();

        // if we got a valid bullet back
        if (newBullet != null)
        {
            // we activate it
            newBullet.SetActive(true);
            // and set it's position and rotation based on the fire position variable.
            newBullet.transform.SetPositionAndRotation(firePosition.position, Quaternion.identity);
        }

        // we then yield this function for our RateOFFire
        yield return new WaitForSeconds(RateOfFire);

        // once that time has elapsed, we say we can fire again.
        CanFire = true;
    }

    public bool canJump = true;

    // FixedUpdate is called a set rate (by default, 60 times per second).
    // if you're modifying physics values, you should do so in FixedUpdate as the physics system runs at the same rate.
    void FixedUpdate () {
        // get the input value from the horizontal axis (a/d/left/right) multiply if by the movespeed modifier value
        float newXPosition = Input.GetAxis("Horizontal") * MoveSpeedModifier;

        // set the velocity of the rigidbody directly, we just retain whatever vertical velocity we already had, otherwise we could interrupt falling or jumping.
        rigidBody.velocity = new Vector3(newXPosition, rigidBody.velocity.y);

        // if the jump button has been pressed *this* frame and we can jump
        if ( Input.GetButtonDown("Jump") && canJump == true)
        {
            // add an impulse force to the rigidbody that throws the player upwards
            rigidBody.AddForce(new Vector2(0.0f, moveForce), ForceMode2D.Impulse);
            // canjump is set back to true in our collision checks.
            canJump = false;
        }

        /*rigidBody.AddForce(
            new Vector2(
                (Input.GetAxis("Horizontal") * moveForce) * Time.deltaTime,
                (Input.GetAxis("Vertical") * moveForce) * Time.deltaTime
                )
            );*/
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if we have collide with anything tagged as "Platform"
        if (collision.collider.CompareTag("Platform"))
        {
            // we can jump again.
            canJump = true;
        }
    }
}
