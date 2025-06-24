using Mirror;
using UnityEngine;

public class PlayerSkin : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSkinChanged))]
    private int skinId = -1;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // ���� ����� ������������� ���������� �� ������� ��� �������� �������
    public override void OnStartServer()
    {
        base.OnStartServer();

        if (SkinManager.Instance != null && connectionToClient != null)
        {
            skinId = SkinManager.Instance.GetOrAssignSkin(connectionToClient);
        }
    }

    // ��� ��������� SyncVar �� ��������
    void OnSkinChanged(int oldId, int newId)
    {
        ApplySkin(newId);
    }

    private void ApplySkin(int id)
    {
        if (animator == null || SkinManager.Instance == null || id < 0) return;
        animator.runtimeAnimatorController = SkinManager.Instance.skins[id];
    }
}
