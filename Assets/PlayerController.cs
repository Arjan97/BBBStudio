using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private bool canBeHit = true;
    public float hitCooldown = 2f;
    private bool isDead = false;
    private AnimationController animationController;
    private Player p;

    private void Awake()
    {
        animationController = GetComponent<AnimationController>();
        p = FindFirstObjectByType<Player>();
        if (animationController == null)
        {
            Debug.LogError("AnimationController not found on the player.");
        }
    }

    public bool CanTakeHit()
    {
        return canBeHit && !isDead;
    }

    public void RegisterHit()
    {
        if (CanTakeHit())
        {
            StartCoroutine(HitCooldown());
        }
    }

    public void TriggerDeath()
    {
        if (!isDead && animationController != null)
        {
            animationController.SetIsDead(true);
            isDead = true;
        }
    }

    private IEnumerator HitCooldown()
    {
        canBeHit = false;
        animationController.SetIsHit(true);

        yield return new WaitForSeconds(0.5f); 
        animationController.SetIsHit(false);

        yield return new WaitForSeconds(hitCooldown - 0.5f); 
        canBeHit = true;
    }

    public void PushPlayer(float tapForce)
    {
        if (p != null)
        {
            p.pushBubble(new Vector2(tapForce, 0f));
        }
    }

    public void PushPlayerBackward()
    {
        if (p != null)
        {
            p.pushBubble(new Vector2(0f, -p.tapForce));
        }
    }
}
