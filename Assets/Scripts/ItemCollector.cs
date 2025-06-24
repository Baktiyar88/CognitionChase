

using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnMelonsChanged))] public int melons = 0;
    [SerializeField] private Text melonsText;
    [SerializeField] private AudioSource collectSoundEffect;

    public override void OnStartLocalPlayer()
    {
        if (melonsText == null)
        {
            GameObject textGO = GameObject.FindWithTag("MelonsText");
            if (textGO != null) melonsText = textGO.GetComponent<Text>();
            else Debug.LogWarning("Не найден объект с тегом 'MelonsText'.");
        }
    }

    private void Start()
    {
        if (isLocalPlayer && melonsText != null) melonsText.text = "Melons: " + melons;
    }

    private void OnMelonsChanged(int oldValue, int newValue)
    {
        if (isLocalPlayer && melonsText != null) melonsText.text = "Melons: " + newValue;
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Melon"))
        {
            melons++;
            NetworkServer.Destroy(collision.gameObject);
            TargetPlayCollectSound(connectionToClient);
        }
    }

    [TargetRpc]
    private void TargetPlayCollectSound(NetworkConnection target)
    {
        collectSoundEffect.Play();
    }

    [Server]
    public void CollectMelon()
    {
        melons++;
        TargetPlayCollectSound(connectionToClient);
    }
}