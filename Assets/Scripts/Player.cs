using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float tapForce = 100f;
    public float tapImpulse = 1f;
    public float upForce = 1f;
    public float windMagnitude = 0.02f;
    // public float deflateVal = -0.1f;
    // public float deflateInterval = 0.1f;

    public GameObject character;
    public AudioClip[] se;

    InputSystem_Actions input;
    AudioSource audio;
    Rigidbody2D bubbleRb;
    Rigidbody2D charRb;

    List<Rigidbody2D> physicsPoints;
    Coroutine gravityCoroutine;
    // Coroutine deflateCoroutine;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
        audio = GetComponent<AudioSource>();
        bubbleRb = transform.GetComponent<Rigidbody2D>();
        charRb = character.GetComponent<Rigidbody2D>();

        // add all vertices to a list for moving them all at once later
        physicsPoints = new List<Rigidbody2D>();
        physicsPoints.Add(bubbleRb);
        physicsPoints.Add(charRb);
        foreach (Transform obj in transform) {
            if (obj.tag == "BubblePoint") {
                physicsPoints.Add(obj.GetComponent<Rigidbody2D>());
            }
        }
        // deflateCoroutine = StartCoroutine(ChangeBubbleSize(deflateVal, deflateInterval));
    }

    void FixedUpdate()
    {
        bubbleRb.AddForce(new Vector2(0f, upForce));

        if (windMagnitude > 0) {
            float force = -transform.localPosition.x * windMagnitude;
            pushBubble(new Vector2(force, 0f));
        }
        // character.rotation = Quaternion.identity;
    }

    void Update() {

        // input section
        if (!input.Player.Dive.WasPressedThisFrame()) {
            return;
        }
        // Start the coroutine to change gravity
        if (gravityCoroutine != null)
        {
            StopCoroutine(gravityCoroutine); // Stop any existing coroutine to avoid conflicts
        }
        gravityCoroutine = StartCoroutine(ChangeGravityForOneSecond());
        character.GetComponent<Animator>().SetTrigger("Drop");
        
        pushBubble(new Vector2(0f, -tapForce));
        charRb.AddForce(new Vector2(0f, -tapImpulse), ForceMode2D.Impulse);

        audio.resource = se[Random.Range(0, se.Length)];
        audio.Play();
    }

    public void pushBubble(Vector2 dir) {
        foreach (var ver in physicsPoints) {
            ver.AddForce(dir);
        }
    }

    private IEnumerator ChangeBubbleSize(float val, float interval)
    {
        Vector3 scale = new Vector3(val, val, val);
        while(true){
            transform.localScale += scale * Time.deltaTime;
            yield return new WaitForSeconds(interval);
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