
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(NetworkIdentity))]
public class MelonQuiz : NetworkBehaviour
{
    [Tooltip("Индекс вопроса из QuizManager.questions")]
    public int questionIndex = 0;

    private QuizManager quizManager;

    private void Start()
    {
        // Находим единственный в сцене QuizManager
        quizManager = Object.FindFirstObjectByType<QuizManager>();
    }

    // Обработка столкновения игрока с Melon (выполняется только на сервере)
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D col)
    {
        var playerNet = col.GetComponent<NetworkIdentity>();
        if (playerNet != null && playerNet.connectionToClient != null)
        {
            // Отключаем коллайдер на сервере, чтобы предотвратить повторные срабатывания до обработки ответа
            GetComponent<Collider2D>().enabled = false;
            // Отправляем конкретному клиенту показать викторину (TargetRpc)
            quizManager.TargetShowQuiz(playerNet.connectionToClient, netIdentity, questionIndex);
        }
    }
}
