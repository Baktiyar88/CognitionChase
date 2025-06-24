using Mirror;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera; // ������ �� ������ � ����������

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(true);
            GetComponent<AudioListener>().enabled = true; // �������� ���� ��� ���������� ������
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            // ������ ������� �� �������, �������� ���� Z-����������
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);
        }
    }
}