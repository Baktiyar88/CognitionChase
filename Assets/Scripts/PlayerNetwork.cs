

using System.Collections.Generic;
using Mirror;
using TMPro;

public class PlayerNetwork : NetworkBehaviour
{
    // Синхронизированное имя игрока
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    // Хук для обновления никнейма на клиентах (например, вывод в UI)
    private TextMeshProUGUI nameText;

    // Статический список занятых ID в диапазоне 1–4
    private static readonly List<int> usedIds = new List<int>();

    void Awake()
    {
        // Предполагается, что в дочернем объекте есть TextMeshProUGUI для отображения имени
        nameText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Вызывается на сервере сразу после спавна игрока
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Найти первый свободный ID в диапазоне [1..4]
        int id = 1;
        while (usedIds.Contains(id) && id <= 4)
            id++;

        if (id <= 4)
        {
            usedIds.Add(id);
            playerName = "P" + id;  // SyncVar: обновит значение у всех клиентов
        }
        else
        {
            // Если все 4 номера заняты, можно отказаться от подключения
            // или присвоить специальное имя
            playerName = "P?"; // пример обработки переполнения
        }
    }

    // Вызывается на сервере при отключении/уничтожении объекта игрока
    public override void OnStopServer()
    {
        base.OnStopServer();

        // Освобождаем ID при выходе игрока
        if (!string.IsNullOrEmpty(playerName) && playerName.Length > 1)
        {
            if (int.TryParse(playerName.Substring(1), out int id) && usedIds.Contains(id))
            {
                usedIds.Remove(id);
            }
        }
    }

    // Хук SyncVar: выполняется на клиентах при изменении playerName
    void OnNameChanged(string oldName, string newName)
    {
        // Обновляем UI-элемент с текстом ника
        if (nameText != null)
            nameText.text = newName;
   
    }
}
