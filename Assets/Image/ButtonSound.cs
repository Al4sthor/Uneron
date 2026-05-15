using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource;

    public void Sonar()
    {
        audioSource.Play();
    }
}