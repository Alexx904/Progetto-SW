using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsUI; // Opzionale se vuoi le impostazioni anche qui

    void Start()
    {
        // Appena premi Play, forziamo la chiusura del menu
        // Così puoi lasciarlo attivo nell'editor per modificarlo, ma sparisce nel gioco
        Resume(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(false);
        Time.timeScale = 1f; // Ripristina il tempo normale
        GameIsPaused = false;
        
        // Riblocca il cursore se è un FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Ferma il tempo
        GameIsPaused = true;
        
        // Sblocca il cursore per cliccare i menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        SaveProgress(); // Salva prima di uscire
        Time.timeScale = 1f; // Importante: ripristina il tempo prima di cambiare scena
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        SaveProgress(); // Salva prima di uscire
        Debug.Log("Uscita dal gioco...");
        Application.Quit();
    }

    // Esempio semplice di salvataggio
    private void SaveProgress()
    {
        // Esempio: Salva il livello corrente come ultimo raggiunto
        // In un gioco reale salveresti posizione, inventario, ecc.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Salviamo solo se siamo andati avanti rispetto al salvataggio precedente
        if(currentSceneIndex > PlayerPrefs.GetInt("LevelReached", 1))
        {
            PlayerPrefs.SetInt("LevelReached", currentSceneIndex);
            PlayerPrefs.Save();
            Debug.Log("Progressi Salvati!");
        }
    }
}