using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerLife : NetworkBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    [SerializeField] private AudioSource deathSoundEffect;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLocalPlayer) return;
        if (collision.gameObject.CompareTag("Trap"))
        {
            CmdDie();
        }
    }

    [Command]
    public void CmdDie()
    {
        RpcDie();
        StartCoroutine(RespawnAfterDelay(2f));
    }

    [ClientRpc]
    public void RpcDie()
    {
        Die();
    }

    public void Die()
    {
        deathSoundEffect.Play();
        rb.bodyType = RigidbodyType2D.Static; // Замораживаем движение
        anim.SetBool("isdead", true); // Запускаем анимацию смерти
    }


 [TargetRpc]
    public void TargetDieOnClient(NetworkConnection target)
    {
        DieOnClient();
    }

    [Client]
    public void DieOnClient()
    {
        if (!isLocalPlayer) return;
        CmdDie();
    }

  

    [Server]
    public void RespawnPlayer()
    {
        Vector3 newPos = GetStartPosition();
        // Обновляем позицию на сервере
        transform.position = newPos;
        rb.bodyType = RigidbodyType2D.Dynamic; // Размораживаем физику
        RpcRespawn(newPos);
    }

    //[ClientRpc]
    //public void RpcRespawn(Vector3 newPosition)
    //{
    //    // Обновляем позицию на клиентах
    //    transform.position = newPosition;
    //    rb.bodyType = RigidbodyType2D.Dynamic;
    //    anim.SetBool("isdead", false);
    //    anim.SetInteger("state", 0);
    //    sprite.enabled = true;
    //}
    [ClientRpc]
    public void RpcRespawn(Vector3 newPosition)
    {
        transform.position = newPosition;
        rb.bodyType = RigidbodyType2D.Dynamic;
        anim.SetBool("isdead", false);
        anim.SetInteger("state", 0);
        sprite.enabled = true;
        // Разморозить игрока после респавна
        PlayerMovement pMove = GetComponent<PlayerMovement>();
        if (pMove != null) pMove.SetFrozen(false);
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isServer)
        {
            RespawnPlayer();
        }
    }

    private Vector3 GetStartPosition()
    {
        NetworkStartPosition[] startPositions = FindObjectsByType<NetworkStartPosition>(FindObjectsSortMode.None);
        if (startPositions.Length > 0)
        {
            return startPositions[Random.Range(0, startPositions.Length)].transform.position;
        }
        return Vector3.zero;
    }
}