
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(NetworkIdentity))]
public class MelonQuiz : NetworkBehaviour
{
    [Tooltip("������ ������� �� QuizManager.questions")]
    public int questionIndex = 0;

    private QuizManager quizManager;

    private void Start()
    {
        // ������� ������������ � ����� QuizManager
        quizManager = Object.FindFirstObjectByType<QuizManager>();
    }

    // ��������� ������������ ������ � Melon (����������� ������ �� �������)
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D col)
    {
        var playerNet = col.GetComponent<NetworkIdentity>();
        if (playerNet != null && playerNet.connectionToClient != null)
        {
            // ��������� ��������� �� �������, ����� ������������� ��������� ������������ �� ��������� ������
            GetComponent<Collider2D>().enabled = false;
            // ���������� ����������� ������� �������� ��������� (TargetRpc)
            quizManager.TargetShowQuiz(playerNet.connectionToClient, netIdentity, questionIndex);
        }
    }
}
