using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SkinManager : NetworkBehaviour
{
    public static SkinManager Instance;

    [Header("Заполните в инспекторе 4 скина")]
    public RuntimeAnimatorController[] skins = new RuntimeAnimatorController[4];

    private readonly List<int> availableSkinIndices = new List<int> { 0, 1, 2, 3 };
    private readonly Dictionary<int, int> playerSkinMap = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server]
    public int GetOrAssignSkin(NetworkConnectionToClient conn)
    {
        if (playerSkinMap.TryGetValue(conn.connectionId, out int existingSkin))
            return existingSkin;

        if (availableSkinIndices.Count == 0)
        {
            Debug.LogWarning("Нет доступных скинов — возвращаем скин по умолчанию (0)");
            return 0;
        }

        int randomIndex = Random.Range(0, availableSkinIndices.Count);
        int skinId = availableSkinIndices[randomIndex];
        availableSkinIndices.RemoveAt(randomIndex);
        playerSkinMap[conn.connectionId] = skinId;

        Debug.Log($"Игрок {conn.connectionId} получил скин {skinId}");
        return skinId;
    }
}
