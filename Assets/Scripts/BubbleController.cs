using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BubbleController : MonoBehaviour
{
    public float tapForce = 100f;
    public float tapImpulse = 1f;
    public float upForce = 1f;
    public float windMagnitude = 0.02f;

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
        bubbleCenterRb = GetComponent<AutoBubble>().center.GetComponent<Rigidbody2D>();
        character.GetComponent<SpringJoint2D>().connectedBody = bubbleCenterRb;
        charRb = character.GetComponent<Rigidbody2D>();

        autoBubble = GetComponent<AutoBubble>();
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

            autoBubble.PushBubble(new Vector2(0f, -tapForce));
            charRb.AddForce(new Vector2(0f, -tapImpulse), ForceMode2D.Impulse);
            audio.resource = se[Random.Range(0, se.Length)];
            audio.Play();
        }

        autoBubble.PushBubble(new Vector2(0f, upForce * Time.deltaTime));

        if (windMagnitude > 0)
        {
            float force = -bubbleCenterRb.gameObject.transform.position.x * windMagnitude;
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