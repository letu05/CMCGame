using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiButton : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LevelOption");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
