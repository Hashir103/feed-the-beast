using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;  // Singleton

    public TextMeshProUGUI interactionPrompt;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Optional if you load multiple scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowPrompt(string text)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.text = text;
            interactionPrompt.enabled = true;
        }
    }

    public void HidePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.enabled = false;
        }
    }
}
