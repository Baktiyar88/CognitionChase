
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : NetworkBehaviour
{
    [System.Serializable]
    public struct Question
    {
        public string question;
        public string[] answers;   // длина = 4
        public int correctIndex;   // от 0 до 3
    }

    [Header("Список вопросов")]
    private Question[] questions = new Question[]
{
    new Question
    {
        question = "Какой элемент периодической таблицы обозначается символом 'O'?",
        answers = new string[] { "Золото", "Кислород", "Серебро", "Углерод" },
        correctIndex = 1
    },
    new Question
    {
        question = "Какой океан самый большой на Земле?",
        answers = new string[] { "Атлантический", "Индийский", "Тихий", "Северный Ледовитый" },
        correctIndex = 2
    },
     new Question
{
    question = "Сколько баллов поставите к дипломной работе?",
    answers = new string[] { "80", "80", "80", "100" },
    correctIndex = 3
    },

    new Question
    {
        question = "В каком году человек впервые высадился на Луну?",
        answers = new string[] { "1959", "1969", "1979", "1989" },
        correctIndex = 1
    },
    new Question
    {
        question = "Сколько материков на Земле?",
        answers = new string[] { "5", "6", "7", "8" },
        correctIndex = 2
    },
    new Question
    {
        question = "Как называется столица Японии?",
        answers = new string[] { "Осака", "Токио", "Киото", "Нагасаки" },
        correctIndex = 1
    },
    new Question
    {
        question = "Сколько часов в сутках?",
        answers = new string[] { "12", "24", "48", "60" },
        correctIndex = 1
    },
    new Question
    {
        question = "Какой газ необходим человеку для дыхания?",
        answers = new string[] { "Азот", "Углекислый газ", "Кислород", "Водород" },
        correctIndex = 2
    }
};

    // UI-ссылки (присвоить в инспекторе)
    public GameObject quizPanel;
    public Text questionText;
    public Button[] answerButtons; // длина = 4

    // Сохраняем, какой мелон и чей игрок
    private NetworkIdentity currentMelon;
    private NetworkConnection currentClient;
    private int currentQIndex;

    private void Awake()
    {
        quizPanel.SetActive(false);
    }

    // Вызывается на сервере для конкретного клиента – показать панель викторины
    [TargetRpc]
    public void TargetShowQuiz(NetworkConnection target, NetworkIdentity melonNetId, int qIndex)
    {
        currentMelon = melonNetId;
        currentQIndex = qIndex;

        var q = questions[qIndex];
        questionText.text = q.question;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int idx = i;
            answerButtons[i].GetComponentInChildren<Text>().text = q.answers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(idx));
        }
        quizPanel.SetActive(true);

        // Замораживаем движение локального игрока
        NetworkClient.localPlayer.GetComponent<PlayerMovement>()?.SetFrozen(true);
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        quizPanel.SetActive(false);
        bool isCorrect = (selectedIndex == questions[currentQIndex].correctIndex);
        // Отправляем ответ на сервер от клиента (любой клиент может вызвать этот Cmd)
        CmdSubmitAnswer(currentMelon, isCorrect);
    }

    [Command(requiresAuthority = false)]
    private void CmdSubmitAnswer(NetworkIdentity melonNetId, bool correct, NetworkConnectionToClient conn = null)
    {
        var playerObj = conn.identity.gameObject;

        if (correct)
        {
            // Правильный ответ — начисляем очки игроку и глобальный счёт
            var pMove = playerObj.GetComponent<PlayerMovement>();
            if (pMove != null) pMove.melon += 1;
            var gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.globalMelons += 1;

            // **Важное изменение:** больше не удаляем Melon (NetworkServer.Destroy закомментирован)
            // Теперь Melon остаётся на сцене для повторных активаций.

            // Разморозить локального игрока после викторины
            TargetFinishQuiz(conn);
        }
        else
        {
            // Неправильный ответ — убиваем игрока на сервере
            var life = playerObj.GetComponent<PlayerLife>();
           // if (life != null) life.CmdDie();
            if (life != null) life.TargetDieOnClient(conn);

            // Включаем коллайдер Melon на сервере, чтобы разрешить повторную активацию
            if (melonNetId != null)
            {
                var col = melonNetId.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;

                // Уведомляем всех клиентов, чтобы они включили коллайдер у соответствующего Melon
                RpcEnableCollider(melonNetId);
            }
        }
    }

    // ClientRpc — вызывается на всех клиентах для синхронизации включения коллайдера Melon
    [ClientRpc]
    private void RpcEnableCollider(NetworkIdentity melonNetId)
    {
        var melonObject = melonNetId.gameObject;
        if (melonObject != null)
        {
            var col = melonObject.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
        }
    }

    [TargetRpc]
    private void TargetFinishQuiz(NetworkConnection target)
    {
        // Разморозка локального игрока после правильного ответа
        NetworkClient.localPlayer.GetComponent<PlayerMovement>()?.SetFrozen(false);
    }
}
