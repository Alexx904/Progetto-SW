using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("Impostazioni Partita")]
    public float tempoTotale = 60f;
    private float tempoRimanente;
    private bool partitaInCorso = true;

    [Header("Riferimenti per Bloccare il Gioco")] // NUOVO
    [Tooltip("Trascina qui l'oggetto 'PlayerPC' o la Camera che ha lo script di movimento")]
    public GameObject playerController; 
    
    [Tooltip("Trascina qui l'oggetto 'Spawner'")]
    public GameObject spawnerRifiuti; 

    [Header("Impostazioni UI")]
    public TextMeshProUGUI whiteBoardText; 
    public TextMeshProUGUI timerText; 
    public GameObject pannelloGameOver; 
    public TextMeshProUGUI testoPunteggioFinale;

    private float punteggioAttuale = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // IMPORTANTE: Assicuriamoci che il tempo riparta quando ricarichi la scena!
        Time.timeScale = 1f; // NUOVO

        tempoRimanente = tempoTotale;
        partitaInCorso = true;
        
        if(pannelloGameOver != null) pannelloGameOver.SetActive(false);
        AggiornaGrafica();
    }

    private void Update()
    {
        if (partitaInCorso)
        {
            tempoRimanente -= Time.deltaTime;

            if (timerText != null)
            {
                int minuti = Mathf.FloorToInt(tempoRimanente / 60);
                int secondi = Mathf.FloorToInt(tempoRimanente % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minuti, secondi);
            }

            if (tempoRimanente <= 0)
            {
                FinePartita();
            }
        }
    }

    public void ModificaPunteggio(float valore)
    {
        if (!partitaInCorso) return;
        punteggioAttuale += valore;
        AggiornaGrafica();
    }

    private void AggiornaGrafica()
    {
        if (whiteBoardText != null)
            whiteBoardText.text = "Punteggio: " + punteggioAttuale.ToString("F0");
    }

    void FinePartita()
    {
        partitaInCorso = false;
        tempoRimanente = 0;
        
        Debug.Log("🛑 GAME OVER");

        // 1. Mostra il pannello
        if (pannelloGameOver != null) pannelloGameOver.SetActive(true);
        if(testoPunteggioFinale != null) testoPunteggioFinale.text = "Hai fatto " + punteggioAttuale + " punti!";

        // 2. Ferma il tempo (Fisica, Spawner, Nastri)
        Time.timeScale = 0f; // NUOVO: Congela tutto il gioco

        // 3. Sblocca il Mouse per cliccare
        Cursor.lockState = CursorLockMode.None; // NUOVO: Libera il cursore dal centro
        Cursor.visible = true; // NUOVO: Rendi visibile la freccina

        // 4. Disattiva i controlli del Player (così non ti giri più)
        if (playerController != null)
        {
            // Proviamo a spegnere tutti gli script di movimento sul player
            MonoBehaviour[] scripts = playerController.GetComponents<MonoBehaviour>();
            foreach(var script in scripts)
            {
                // Non spegnere AudioListener o Camera, solo gli script custom
                if(script != this) script.enabled = false; 
            }
        }

        // 5. Spegni lo Spawner (sicurezza extra)
        if (spawnerRifiuti != null) spawnerRifiuti.SetActive(false);
    }

    public void TornaAlMenu()
    {
        // Prima di cambiare scena, dobbiamo riattivare il tempo!
        Time.timeScale = 1f; // NUOVO
        SceneManager.LoadScene("MainMenu");
    }
}