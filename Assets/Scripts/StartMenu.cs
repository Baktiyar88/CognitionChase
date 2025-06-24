using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button singlePlayerButton; // Кнопка для одиночной игры
    public Button hostButton;         // Кнопка для хоста
    public Button joinButton;         // Кнопка для клиента
    public InputField ipInput;        // Поле для ввода IP-адреса

    void Start()
    {
        // Привязываем действия к кнопкам
        singlePlayerButton.onClick.AddListener(StartSinglePlayer);
        hostButton.onClick.AddListener(() => NetworkManager.singleton.StartHost());
        joinButton.onClick.AddListener(() =>
        {
            NetworkManager.singleton.networkAddress = ipInput.text; // Устанавливаем IP
            NetworkManager.singleton.StartClient();                 // Запускаем клиент
        });
    }

    void StartSinglePlayer()
    {
        // Загружаем сцену игры без сети
        SceneManager.LoadScene("Level 1"); // Замените "Level1" на имя вашей игровой сцены
    }
}