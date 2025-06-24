using UnityEngine;

public class EndGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
