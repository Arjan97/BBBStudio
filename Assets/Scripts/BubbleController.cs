using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BubbleController : MonoBehaviour
{
    public float tapBubbleForce = 0f;
    public float tapBirdImpulse = 2f;
    public float buoyancyMultiplier = 5f;
    public float windMagnitude = 1f;

    public GameObject character;
    public AudioClip[] se;

    InputSystem_Actions input;
    AudioSource audio;
    Rigidbody2D bubbleCenterRb;
    Rigidbody2D charRb;

    Coroutine gravityCoroutine;
    AutoBubble autoBubble;

    private void Start()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
        audio = GetComponent<AudioSource>();
        
        autoBubble = GetComponent<AutoBubble>();
        bubbleCenterRb = autoBubble.center.GetComponent<Rigidbody2D>();
        character.GetComponent<SpringJoint2D>().connectedBody = bubbleCenterRb;
        charRb = character.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // keep character within limits
        Vector2 offset = charRb.position - bubbleCenterRb.position;
        var r = autoBubble.radius;
        // var r = 0.3f;
        if (offset.magnitude > r) {
            charRb.position = bubbleCenterRb.position + offset.normalized * r;
            // charRb.linearVelocity = Vector2.zero;
            charRb.linearVelocity = -charRb.linearVelocity/2;
        }
    }

    // private void OnEnable()
    // {
    //     // Enable input only in gameplay scenes
    //     string currentScene = SceneManager.GetActiveScene().name;
    //     if (currentScene != "StartScreen" && currentScene != "GameOver")
    //     {
    //         input.Player.Enable();
    //     }
    // }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    // void FixedUpdate()
    // {
    //     if (SceneManager.GetActiveScene().name == "StartScreen" || SceneManager.GetActiveScene().name == "GameOver")
    //         return;


    // }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "StartScreen" || SceneManager.GetActiveScene().name == "GameOver")
            return;

        if (input.Player.Dive.WasPressedThisFrame())
        {
            // Start the coroutine to change gravity
            if (gravityCoroutine != null)
            {
                StopCoroutine(gravityCoroutine); // Stop any existing coroutine to avoid conflicts
            }
            gravityCoroutine = StartCoroutine(ChangeGravityForOneSecond());
            character.GetComponent<Animator>().SetTrigger("Drop");

            autoBubble.PushBubble(new Vector2(0f, -tapBubbleForce));
            charRb.AddForce(new Vector2(0f, -tapBirdImpulse), ForceMode2D.Impulse);
            audio.resource = se[Random.Range(0, se.Length)];
            audio.Play();
        }

        // bouyancy
        // var vol = Mathf.Pow(autoBubble.radius, 2);
        var vol = autoBubble.radius;
        autoBubble.PushBubble(new Vector2(0f, vol * buoyancyMultiplier * Time.deltaTime));

        if (windMagnitude > 0)
        {
            float force = -bubbleCenterRb.linearVelocity.x * windMagnitude;
            autoBubble.PushBubble(new Vector2(force * Time.deltaTime, 0f));
        }
    }

    private IEnumerator ChangeGravityForOneSecond()
    {
        charRb.gravityScale = 1f;

        yield return new WaitForSeconds(1f);

        // Revert gravity back to 0
        charRb.gravityScale = 0f;

        // Optional: Nullify the coroutine reference (good practice)
        gravityCoroutine = null;
    }
}