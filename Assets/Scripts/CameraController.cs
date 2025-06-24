using Mirror;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera; // —сылка на камеру в инспекторе

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(true);
            GetComponent<AudioListener>().enabled = true; // ¬ключаем звук дл€ локального игрока
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            //  амера следует за игроком, сохран€€ свою Z-координату
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);
        }
    }
}