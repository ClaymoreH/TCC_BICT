using UnityEngine;
using UnityEngine.UI;

public class ImagePanelController : MonoBehaviour
{
    public Image displayImage; 
    public GameObject helpButton;
    public AudioClip clickSound;   
    public AudioClip closeSound; 
    public AudioSource audioSource;
    public Sprite[] images;

    
    private int currentIndex = 0;

    private void UpdateImage()
    {
        if (images.Length > 0 && displayImage != null)
        {
            displayImage.sprite = images[currentIndex];
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void NextImage()
    {
        if (images.Length == 0) return;
        currentIndex = (currentIndex + 1) % images.Length;
        UpdateImage();
        PlaySound(clickSound); 
    }

    public void PreviousImage()
    {
        if (images.Length == 0) return;
        currentIndex = (currentIndex - 1 + images.Length) % images.Length;
        UpdateImage();
        PlaySound(clickSound);
        
    }

    public void ClosePanel()
    {
        PlaySound(closeSound); 
        gameObject.SetActive(false);
        if (helpButton != null)
        {
            helpButton.SetActive(true);
        }
    }
    public void OpenPanel()
    {
        gameObject.SetActive(true);
        PlaySound(closeSound);
        UpdateImage();
    }

    private void Start()
    {
        UpdateImage();
        PlaySound(closeSound); 

    }
}
