using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{

    public void StopGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
            Debug.Log("Игра остановлена. Возврат в главное меню...");

        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
            Debug.Log("Игра остановлена. Возврат в главное меню...");
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();

        }

    }
    public void OnBackToMenuButton()
    {
        // Если мы хостим — останавливаем хост
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // Если только клиент — останавливаем клиент
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        // Если только сервер — останавливаем сервер
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }

        // После отключения загружаем локальную сцену меню
        SceneManager.LoadScene("Menu");
    }
}
