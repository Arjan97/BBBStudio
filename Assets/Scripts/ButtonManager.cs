using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonManager : MonoBehaviour
{
    public enum ButtonType
    {
        Play,
        Pause,
        Quit,
        Retry,
        Custom
    }

    public ButtonType buttonType;
    public Button button;
    public TMP_Text buttonText;
    public AnimationController animatorController;
    public float loadLevelDelay = 5f;
    public bool hasStarted = false;
    public delegate void CustomButtonAction();
    public CustomButtonAction customAction;

    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(HandleButtonClick);

        UpdateButtonLabel();
    }

    public void HandleButtonClick()
    {
        switch (buttonType)
        {
            case ButtonType.Play:
                PlayAction();
                break;
            case ButtonType.Pause:
                PauseAction();
                break;
            case ButtonType.Quit:
                QuitAction();
                break;
            case ButtonType.Retry:
                RetryAction();
                break;
            case ButtonType.Custom:
                customAction?.Invoke();
                break;
        }
    }

    private void UpdateButtonLabel()
    {
        if (buttonText != null)
        {
            buttonText.text = buttonType.ToString();
        }
    }

    private void PlayAction()
    {
        if (hasStarted)
            return;

        if (animatorController != null)
        {
            animatorController.SetFliesAway();
        }
        hasStarted = true;
        StartCoroutine(LoadLevelAfterDelay("AlphaLevel", loadLevelDelay));
    }

    private void RetryAction()
    {
        ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene("AlphaLevel");

    }
    private void PauseAction()
    {
        Debug.Log("Pause button clicked!");
    }

    private void QuitAction()
    {
        Debug.Log("Quit button clicked!");
        Application.Quit();
    }

    IEnumerator LoadLevelAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}