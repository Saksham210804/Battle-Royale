using UnityEngine;

public class Game_Menu_Manager : MonoBehaviour
{
    public GameObject Player;
    public GameObject Pause_Menu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))

            {
            Pause_Menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Player.SetActive(false); // Stop the game time
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
