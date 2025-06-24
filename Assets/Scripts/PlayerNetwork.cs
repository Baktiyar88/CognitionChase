

using System.Collections.Generic;
using Mirror;
using TMPro;

public class PlayerNetwork : NetworkBehaviour
{
    // ������������������ ��� ������
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    // ��� ��� ���������� �������� �� �������� (��������, ����� � UI)
    private TextMeshProUGUI nameText;

    // ����������� ������ ������� ID � ��������� 1�4
    private static readonly List<int> usedIds = new List<int>();

    void Awake()
    {
        // ��������������, ��� � �������� ������� ���� TextMeshProUGUI ��� ����������� �����
        nameText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // ���������� �� ������� ����� ����� ������ ������
    public override void OnStartServer()
    {
        base.OnStartServer();

        // ����� ������ ��������� ID � ��������� [1..4]
        int id = 1;
        while (usedIds.Contains(id) && id <= 4)
            id++;

        if (id <= 4)
        {
            usedIds.Add(id);
            playerName = "P" + id;  // SyncVar: ������� �������� � ���� ��������
        }
        else
        {
            // ���� ��� 4 ������ ������, ����� ���������� �� �����������
            // ��� ��������� ����������� ���
            playerName = "P?"; // ������ ��������� ������������
        }
    }

    // ���������� �� ������� ��� ����������/����������� ������� ������
    public override void OnStopServer()
    {
        base.OnStopServer();

        // ����������� ID ��� ������ ������
        if (!string.IsNullOrEmpty(playerName) && playerName.Length > 1)
        {
            if (int.TryParse(playerName.Substring(1), out int id) && usedIds.Contains(id))
            {
                usedIds.Remove(id);
            }
        }
    }

    // ��� SyncVar: ����������� �� �������� ��� ��������� playerName
    void OnNameChanged(string oldName, string newName)
    {
        // ��������� UI-������� � ������� ����
        if (nameText != null)
            nameText.text = newName;
   
    }
}
