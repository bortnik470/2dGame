using UnityEngine;

public class MenuScript : MonoBehaviour
{
    private bool isPause = true;
    public GameObject menu;

    private void Start()
    {
        Pause();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (isPause) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
        isPause = false;
    }
    
    private void Pause()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
        isPause = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
