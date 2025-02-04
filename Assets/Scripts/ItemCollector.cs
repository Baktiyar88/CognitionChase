using UnityEngine;
using UnityEngine.UI;    

public class ItemCollector :MonoBehaviour
{
    private int melons = 0;

    [SerializeField] private Text melonsText;

    [SerializeField] private AudioSource collectSoundEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Melon"))
        {
            collectSoundEffect.Play();
            Destroy(collision.gameObject);
            melons++;
            melonsText.text = "Melons: " + melons;
        }


    }
}
