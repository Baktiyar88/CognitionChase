using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button singlePlayerButton; // ������ ��� ��������� ����
    public Button hostButton;         // ������ ��� �����
    public Button joinButton;         // ������ ��� �������
    public InputField ipInput;        // ���� ��� ����� IP-������

    void Start()
    {
        // ����������� �������� � �������
        singlePlayerButton.onClick.AddListener(StartSinglePlayer);
        hostButton.onClick.AddListener(() => NetworkManager.singleton.StartHost());
        joinButton.onClick.AddListener(() =>
        {
            NetworkManager.singleton.networkAddress = ipInput.text; // ������������� IP
            NetworkManager.singleton.StartClient();                 // ��������� ������
        });
    }

    void StartSinglePlayer()
    {
        // ��������� ����� ���� ��� ����
        SceneManager.LoadScene("Level 1"); // �������� "Level1" �� ��� ����� ������� �����
    }
}