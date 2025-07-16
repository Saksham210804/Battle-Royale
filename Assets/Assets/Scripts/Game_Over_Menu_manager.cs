using UnityEngine;

public class Game_Over_Menu_manager : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }
    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    public void Quit()

    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Application.Quit();
        Debug.Log("Quit");
    }
}
