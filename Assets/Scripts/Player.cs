using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float tapForce = 100f;

    // public Transform character;

    InputSystem_Actions input;
    Rigidbody2D rb;

    List<Rigidbody2D> verticies;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
        rb = transform.GetComponent<Rigidbody2D>();

        // add all vertices to a list for moving them all at once later
        verticies = new List<Rigidbody2D>();
        verticies.Add(rb);
        foreach (Transform obj in transform) {
            if (obj.tag == "BubblePoint") {
                verticies.Add(obj.GetComponent<Rigidbody2D>());
            }
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector2(0f, 1f));

        // character.rotation = Quaternion.identity;
    }

    void Update() {
        if (!input.Player.Dive.WasPressedThisFrame()) {
            return;
        }
        foreach (var ver in verticies) {
            ver.AddForce(new Vector2(0f, -tapForce));
        }
    }
}