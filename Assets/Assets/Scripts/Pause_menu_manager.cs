using UnityEngine;

public class Pause_menu_manager : MonoBehaviour
{
    public GameObject Player;  
    public GameObject Pause_Menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void exit()
    {

       Application.Quit();
    }
    public void ResumeGame()
    {
        Pause_Menu.SetActive(false);
        Player.SetActive(true);
    }
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu" );
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
