using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float tapForce = 100f;

    public GameObject character;

    InputSystem_Actions input;
    Rigidbody2D bubbleRb;
    Rigidbody2D charRb;

    List<Rigidbody2D> physicsPoints;
    Coroutine gravityCoroutine;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
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
    }

    void FixedUpdate()
    {
        bubbleRb.AddForce(new Vector2(0f, 1f));

        // character.rotation = Quaternion.identity;
    }

    void Update() {
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
        
        foreach (var ver in physicsPoints) {
            ver.AddForce(new Vector2(0f, -tapForce));
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