using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;

public class MainMenuScript : MonoBehaviour
{
	[Header("Scene Settings")]
	[Tooltip("Name of the scene to load when Play is pressed. Must be added to Build Settings.")]
	[SerializeField] private UnityTransport transport;
    [SerializeField] private string address = "127.0.0.1";

	public void HostGame()
	{
		NetworkManager.Singleton.StartHost();
		NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
	}

	public void JoinGame()
	{
        transport.ConnectionData.Address = address;
        NetworkManager.Singleton.StartClient();
	}
}
