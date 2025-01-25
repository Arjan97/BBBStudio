using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    public GameObject player;
    public GameObject spawnObject;
    public float spawnDistance = 5f;

    private bool infiniteBubbleBar = false;
    private BubbleBar bubbleBar;
    private InputAction spawnAction;
    private InputAction toggleInfiniteBubbleBarAction;

    private void Awake()
    {
        var inputActions = new InputActionMap("CheatControls");

        spawnAction = inputActions.AddAction("Spawn", binding: "<Keyboard>/e");
        toggleInfiniteBubbleBarAction = inputActions.AddAction("ToggleInfiniteBubbleBar", binding: "<Keyboard>/q");

        spawnAction.performed += _ => SpawnObjectInFrontOfPlayer();
        toggleInfiniteBubbleBarAction.performed += _ => ToggleInfiniteBubbleBar();

        inputActions.Enable();
    }

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        bubbleBar = FindFirstObjectByType<BubbleBar>();

        if (bubbleBar == null)
        {
            Debug.LogError("BubbleBar not found on the player object.");
        }
    }

    private void Update()
    {
        if (infiniteBubbleBar && bubbleBar != null)
        {
            bubbleBar.currentValue = bubbleBar.maxValue;
        }
    }

    private void SpawnObjectInFrontOfPlayer()
    {
        if (player != null && spawnObject != null)
        {
            Vector3 spawnPosition = player.transform.position + player.transform.right * spawnDistance;
            Instantiate(spawnObject, spawnPosition, Quaternion.identity);
            Debug.Log("Spawned object in front of player.");
        }
        else
        {
            Debug.LogError("Player or spawnObject is not assigned.");
        }
    }

    private void ToggleInfiniteBubbleBar()
    {
        infiniteBubbleBar = !infiniteBubbleBar;
        Debug.Log("Infinite BubbleBar toggled: " + infiniteBubbleBar);
    }
}