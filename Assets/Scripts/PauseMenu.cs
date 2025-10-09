using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if TMP_PRESENT
using TMPro;
#endif

public class PauseMenu : MonoBehaviour
{
	[Header("UI References")]
	[Tooltip("Root panel of the pause overlay. Enable/disable to show or hide the pause UI.")]
	[SerializeField]
	private GameObject pausePanel;

	[Tooltip("Slider that controls rotation speed")]
	[SerializeField]
	private Slider rotationSlider;

	[SerializeField]
	private UnityEngine.Component rotationValueTMP; 

	[Header("Gameplay")]
	[Tooltip("PlayerController to update. If empty the script will try to find one in the scene.")]
	[SerializeField]
	private PlayerController playerController;

	[Tooltip("Min rotation speed for the slider")]
	[SerializeField]
	private float minRotationSpeed = 0.1f;

	[Tooltip("Max rotation speed for the slider")]
	[SerializeField]
	private float maxRotationSpeed = 20f;

	[Header("Scenes")]
	[Tooltip("Name of the main menu scene to load when quitting to menu. Must be added to Build Settings.")]
	[SerializeField]
	private string mainMenuSceneName = "MainMenu";

	private bool isOpen = false;
	private CursorLockMode previousCursorState;
	private bool previousCursorVisible;
	private bool previousAcceptInput = true;

	void Start()
	{
		if (playerController == null)
			playerController = FindObjectOfType<PlayerController>();

		if (pausePanel != null)
			pausePanel.SetActive(false);

		if (rotationSlider != null)
		{
			rotationSlider.minValue = minRotationSpeed;
			rotationSlider.maxValue = maxRotationSpeed;
			if (playerController != null)
				rotationSlider.value = playerController.RotationSpeed;
			rotationSlider.onValueChanged.AddListener(OnRotationSliderChanged);
		}

		UpdateRotationText();
	}

	void OnDestroy()
	{
		if (rotationSlider != null)
			rotationSlider.onValueChanged.RemoveListener(OnRotationSliderChanged);
	}

	void Update()
	{
		var keyboard = Keyboard.current;
		if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
		{
			TogglePause();
		}
	}

	public void TogglePause()
	{
		if (isOpen) ClosePause(); else OpenPause();
	}

	public void OpenPause()
	{
		if (isOpen) return;
		isOpen = true;

		// Show UI overlay
		if (pausePanel != null) pausePanel.SetActive(true);

		// Unlock cursor so player can interact with UI
		previousCursorState = Cursor.lockState;
		previousCursorVisible = Cursor.visible;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// Disable player input so mouse/camera input stops while UI is open
		if (playerController != null)
		{
			previousAcceptInput = playerController.AcceptInput;
			playerController.AcceptInput = false;
		}
	}

	public void ClosePause()
	{
		if (!isOpen) return;
		isOpen = false;

		if (pausePanel != null) pausePanel.SetActive(false);

		// Restore cursor state
		Cursor.lockState = previousCursorState;
		Cursor.visible = previousCursorVisible;

		// Restore player input enabled state
		if (playerController != null)
		{
			playerController.AcceptInput = previousAcceptInput;
		}
	}

	private void OnRotationSliderChanged(float value)
	{
		if (playerController != null)
		{
			playerController.RotationSpeed = value;
		}
		UpdateRotationText();
	}

	private void UpdateRotationText()
	{
		string s = rotationSlider != null ? rotationSlider.value.ToString("0.00") : (playerController != null ? playerController.RotationSpeed.ToString("0.00") : "-");

		if (rotationValueTMP != null)
		{
			// Try to update TextMeshProUGUI if the user assigned one
			var tmp = rotationValueTMP as UnityEngine.Component;
			if (tmp != null)
			{
				var tpro = tmp.GetComponent("TextMeshProUGUI");
				if (tpro != null)
				{
					var prop = tpro.GetType().GetProperty("text");
					if (prop != null) prop.SetValue(tpro, s, null);
				}
			}
		}
	}

	// Public helper so UI button can call Resume
	public void Resume()
	{
		ClosePause();
	}

	// Public helper to quit to the main menu scene (does not quit the application)
	public void QuitToMainMenu()
	{
		// make sure cursor is visible and unlocked when we load the menu
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		if (string.IsNullOrEmpty(mainMenuSceneName))
		{
			Debug.LogError("PauseMenu: mainMenuSceneName not set. Set it in the Inspector or add the main menu to Build Settings.");
			return;
		}

		SceneManager.LoadScene(mainMenuSceneName);
	}
}
