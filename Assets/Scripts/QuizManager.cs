
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : NetworkBehaviour
{
    [System.Serializable]
    public struct Question
    {
        public string question;
        public string[] answers;   // ����� = 4
        public int correctIndex;   // �� 0 �� 3
    }

    [Header("������ ��������")]
    private Question[] questions = new Question[]
{
    new Question
    {
        question = "����� ������� ������������� ������� ������������ �������� 'O'?",
        answers = new string[] { "������", "��������", "�������", "�������" },
        correctIndex = 1
    },
    new Question
    {
        question = "����� ����� ����� ������� �� �����?",
        answers = new string[] { "�������������", "���������", "�����", "�������� ���������" },
        correctIndex = 2
    },
     new Question
{
    question = "������� ������ ��������� � ��������� ������?",
    answers = new string[] { "80", "80", "80", "100" },
    correctIndex = 3
    },

    new Question
    {
        question = "� ����� ���� ������� ������� ��������� �� ����?",
        answers = new string[] { "1959", "1969", "1979", "1989" },
        correctIndex = 1
    },
    new Question
    {
        question = "������� ��������� �� �����?",
        answers = new string[] { "5", "6", "7", "8" },
        correctIndex = 2
    },
    new Question
    {
        question = "��� ���������� ������� ������?",
        answers = new string[] { "�����", "�����", "�����", "��������" },
        correctIndex = 1
    },
    new Question
    {
        question = "������� ����� � ������?",
        answers = new string[] { "12", "24", "48", "60" },
        correctIndex = 1
    },
    new Question
    {
        question = "����� ��� ��������� �������� ��� �������?",
        answers = new string[] { "����", "���������� ���", "��������", "�������" },
        correctIndex = 2
    }
};

    // UI-������ (��������� � ����������)
    public GameObject quizPanel;
    public Text questionText;
    public Button[] answerButtons; // ����� = 4

    // ���������, ����� ����� � ��� �����
    private NetworkIdentity currentMelon;
    private NetworkConnection currentClient;
    private int currentQIndex;

    private void Awake()
    {
        quizPanel.SetActive(false);
    }

    // ���������� �� ������� ��� ����������� ������� � �������� ������ ���������
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

        // ������������ �������� ���������� ������
        NetworkClient.localPlayer.GetComponent<PlayerMovement>()?.SetFrozen(true);
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        quizPanel.SetActive(false);
        bool isCorrect = (selectedIndex == questions[currentQIndex].correctIndex);
        // ���������� ����� �� ������ �� ������� (����� ������ ����� ������� ���� Cmd)
        CmdSubmitAnswer(currentMelon, isCorrect);
    }

    [Command(requiresAuthority = false)]
    private void CmdSubmitAnswer(NetworkIdentity melonNetId, bool correct, NetworkConnectionToClient conn = null)
    {
        var playerObj = conn.identity.gameObject;

        if (correct)
        {
            // ���������� ����� � ��������� ���� ������ � ���������� ����
            var pMove = playerObj.GetComponent<PlayerMovement>();
            if (pMove != null) pMove.melon += 1;
            var gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.globalMelons += 1;

            // **������ ���������:** ������ �� ������� Melon (NetworkServer.Destroy ���������������)
            // ������ Melon ������� �� ����� ��� ��������� ���������.

            // ����������� ���������� ������ ����� ���������
            TargetFinishQuiz(conn);
        }
        else
        {
            // ������������ ����� � ������� ������ �� �������
            var life = playerObj.GetComponent<PlayerLife>();
           // if (life != null) life.CmdDie();
            if (life != null) life.TargetDieOnClient(conn);

            // �������� ��������� Melon �� �������, ����� ��������� ��������� ���������
            if (melonNetId != null)
            {
                var col = melonNetId.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;

                // ���������� ���� ��������, ����� ��� �������� ��������� � ���������������� Melon
                RpcEnableCollider(melonNetId);
            }
        }
    }

    // ClientRpc � ���������� �� ���� �������� ��� ������������� ��������� ���������� Melon
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
        // ���������� ���������� ������ ����� ����������� ������
        NetworkClient.localPlayer.GetComponent<PlayerMovement>()?.SetFrozen(false);
    }
}
