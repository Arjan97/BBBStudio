using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarningManager : MonoBehaviour
{
    public GameObject warningPrefab;
    public float warningDuration = 1.5f;

    public void ShowWarning(Vector3 enemyPosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(Camera.main.transform.position.x + 8f, enemyPosition.y, 0));
        GameObject warning = Instantiate(warningPrefab, screenPos, Quaternion.identity, FindObjectOfType<Canvas>().transform);
        StartCoroutine(RemoveWarning(warning));
    }

    private IEnumerator RemoveWarning(GameObject warning)
    {
        yield return new WaitForSeconds(warningDuration);
        Destroy(warning);
    }
}
