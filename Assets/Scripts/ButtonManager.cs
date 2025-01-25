using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public enum ButtonType
    {
        Play,
        Pause,
        Quit,
        Custom
    }

    public ButtonType buttonType;
    public Button button;
    public TMP_Text buttonText;

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
        SceneManager.LoadScene("TestScene");
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
}
