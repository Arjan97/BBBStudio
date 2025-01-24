using UnityEngine;

public class Player : MonoBehaviour
{
    InputSystem_Actions input;
    Rigidbody2D rb;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
        rb = transform.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector2(0f, 1f));
    }

    void Update() {
        if (input.Player.Dive.WasPressedThisFrame())
            rb.AddForce(new Vector2(0f, -100f));
    }
}