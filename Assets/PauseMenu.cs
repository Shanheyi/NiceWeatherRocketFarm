using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    public GameObject menuPanel;
    public GameObject shopPanel;
    public Slider volumeSlider;
    public KeyCode pauseKey = KeyCode.Escape;

    private bool isOpen = false;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        menuPanel.SetActive(false);

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (shopPanel != null && shopPanel.activeInHierarchy)
            {
                return;
            }

            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        menuPanel.SetActive(isOpen);
        Time.timeScale = isOpen ? 0f : 1f;
        Cursor.visible = isOpen;
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CloseMenu()
    {
        if (isOpen)
        {
            ToggleMenu();
        }
    }
}
