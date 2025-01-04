using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    public Image displayImage;
    public Sprite[] images;
    public Button nextButton;
    public Button backButton;
    public Button infoButton;
    public Button finishButton;
    public Button exitButton;
    public GameObject darkPanel;
    public AudioSource audioSource;
    public AudioClip transitionSound;

    private int currentIndex = 0;
    private bool isPaused = false;

    void Start()
    {
        nextButton.onClick.AddListener(NextImage);
        backButton.onClick.AddListener(PreviousImage);
        finishButton.onClick.AddListener(NextImage);
        infoButton.onClick.AddListener(RestartSequence);
        exitButton.onClick.AddListener(EndSequence);

        PauseScene(true);
        UpdateImage();

        if (darkPanel != null)
        {
            darkPanel.SetActive(true);
        }

        backButton.gameObject.SetActive(false);
        infoButton.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(false);
    }

    void NextImage()
    {
        if (currentIndex < images.Length - 1)
        {
            currentIndex++;
            UpdateImage();
        }
        else
        {
            EndSequence();
        }
    }

    void PreviousImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateImage();
        }
    }

    void RestartSequence()
    {
        currentIndex = 0;
        displayImage.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        finishButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        infoButton.gameObject.SetActive(false);
        PauseScene(true);
        UpdateImage();

        if (darkPanel != null)
        {
            darkPanel.SetActive(true);
        }
    }

    void UpdateImage()
    {
        displayImage.sprite = images[currentIndex];

        if (audioSource && transitionSound)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        backButton.gameObject.SetActive(currentIndex > 0);

        if (currentIndex == images.Length - 1)
        {
            nextButton.gameObject.SetActive(false);
            finishButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            finishButton.gameObject.SetActive(false);
        }
    }

    void EndSequence()
    {
        if (audioSource && transitionSound)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        nextButton.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        displayImage.gameObject.SetActive(false);
        infoButton.gameObject.SetActive(true);
        PauseScene(false);

        if (darkPanel != null)
        {
            darkPanel.SetActive(false);
        }
    }

    void PauseScene(bool pause)
    {
        if (pause && !isPaused)
        {
            Time.timeScale = 0;
            isPaused = true;
        }
        else if (!pause && isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
        }
    }
}












