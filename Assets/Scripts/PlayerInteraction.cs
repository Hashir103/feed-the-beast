using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldItem;
    public TextMeshProUGUI promptUI;

    private void FindPromptUI()
    {
        if (promptUI == null)
        {
            GameObject promptObject = GameObject.FindGameObjectWithTag("InteractionPrompt");
            if (promptObject != null)
            {
                promptUI = promptObject.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    public bool IsHoldingPreparableItem()
    {
        if (heldItem == null)
        {
            Debug.Log("No item is held.");
            return false;
        }

        if (heldItem.GetComponent<IPreparable>() != null)
        {
            Debug.Log("Held item is preparable.");
            return true;
        }

        Debug.Log("Held item is not preparable.");
        return false;
    }

    public void ShowPrompt(string text)
    {
        UIManager.Instance.ShowPrompt(text);
        Debug.Log(text);
    }

    public void HidePrompt()
    {
        UIManager.Instance.HidePrompt();
        Debug.Log("hidden");
    }
}
