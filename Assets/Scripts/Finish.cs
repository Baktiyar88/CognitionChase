using Mirror;
using UnityEngine;

public class Finish : NetworkBehaviour
{
    private AudioSource finishSound;
    private GameManager gameManager;
    private bool hasFinished = false;

    private void Start()
    {
        finishSound = GetComponent<AudioSource>();
        // Предполагается, что в сцене есть только один GameManager
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // Обработка коллизии только на сервере
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasFinished) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            hasFinished = true;
            RpcPlayFinishSound();
            //finishSound.Play();
            gameManager.CompleteLevel();
        }
    }
    [ClientRpc]
    private void RpcPlayFinishSound()
    {
        if (finishSound != null && !finishSound.isPlaying)
        {
            finishSound.Play();
        }
    }
}

