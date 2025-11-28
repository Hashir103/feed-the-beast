using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
	[Header("Scene Settings")]
	[Tooltip("Name of the scene to load when Play is pressed. Must be added to Build Settings.")]
	[SerializeField]
	private string gameSceneName = "GameScene";

	// Called by the Play button (hook this method to the Button OnClick in the Inspector)
	public void PlayGame()
	{

		SceneManager.LoadScene(gameSceneName);
	}
}
