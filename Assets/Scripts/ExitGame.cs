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
            Debug.Log("���� �����������. ������� � ������� ����...");

        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
            Debug.Log("���� �����������. ������� � ������� ����...");
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();

        }

    }
    public void OnBackToMenuButton()
    {
        // ���� �� ������ � ������������� ����
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // ���� ������ ������ � ������������� ������
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        // ���� ������ ������ � ������������� ������
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }

        // ����� ���������� ��������� ��������� ����� ����
        SceneManager.LoadScene("Menu");
    }
}
