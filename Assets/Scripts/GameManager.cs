using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public int globalMelons;

    public Text globalMelonText;
    public Text melonText;
    public GameObject melonPrefab;


    [SyncVar] private bool levelCompleted = false;

    public void CompleteLevel()
    {
        if (!isServer) return;

        if (!levelCompleted)
        {
            levelCompleted = true;
            RpcCompleteLevel();
            Invoke(nameof(LoadNextLevel), 2f); // Переход через 2 секунды
        }
    }

    [ClientRpc]
    private void RpcCompleteLevel()
    {
        Debug.Log("Уровень завершён! Переход через 2 секунды...");
    }

    private void LoadNextLevel()
    {
        // Текущий индекс и общее число сцен в билде
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        Debug.Log($"[GameManager] currentIndex = {currentIndex}, totalScenes = {totalScenes}");

        if (currentIndex < totalScenes - 1)
        {
            // Есть следующий уровень — просто инкрементируем индекс
            string nextSceneName = SceneUtility.GetScenePathByBuildIndex(currentIndex + 1);
            NetworkManager.singleton.ServerChangeScene(nextSceneName);
        }
        else
        {
            // Это последний элемент в списке → идём на EndGame
            NetworkManager.singleton.ServerChangeScene("EndGame");
        }
    }
}
