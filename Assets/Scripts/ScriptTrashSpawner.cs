using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // Necessario per leggere le cartelle nell'Editor
#endif

public class ScriptTrashSpawner : MonoBehaviour
{
    [Header("Impostazioni Spawn")]
    public Transform puntoDiSpawn; // Trascina qui l'oggetto vuoto da dove nascono i rifiuti
    public float tempoTraSpawn = 2.0f; // Velocità di spawn (secondi)

    [Header("Categorie Rifiuti")]
    // Questa lista conterrà le nostre categorie (Carta, Plastica, ecc.)
    public List<CategoriaRifiuto> categorie;

    private Coroutine spawnCoroutine;
    private bool isSpawning = false;

    [System.Serializable]
    public class CategoriaRifiuto
    {
        public string nome;       // Es: "Carta", "Plastica"
        public bool attiva = true; // Se false, questa categoria non verrà spawnata
        public List<GameObject> prefabs; // La lista dei prefab
    }

    void Start()
    {
        // Se non hai assegnato un punto di spawn, usa la posizione di questo oggetto
        if (puntoDiSpawn == null) puntoDiSpawn = transform;
        
        StartSpawning();
    }

    // --- LOGICA DI SPAWN ---

    public void StartSpawning()
    {
        if (isSpawning) return;
        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
    }

    IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            GeneraRifiuto();
            // Aspetta il tempo definito prima del prossimo spawn
            yield return new WaitForSeconds(tempoTraSpawn);
        }
    }

    void GeneraRifiuto()
    {
        // 1. Filtra solo le categorie attive che hanno almeno un prefab
        List<CategoriaRifiuto> categorieAttive = new List<CategoriaRifiuto>();
        foreach (var cat in categorie)
        {
            if (cat.attiva && cat.prefabs.Count > 0)
                categorieAttive.Add(cat);
        }

        if (categorieAttive.Count == 0) return; // Nessuna categoria attiva

        // 2. Scegli una categoria a caso
        CategoriaRifiuto categoriaScelta = categorieAttive[Random.Range(0, categorieAttive.Count)];

        // 3. Scegli un prefab a caso dentro quella categoria
        GameObject prefabScelto = categoriaScelta.prefabs[Random.Range(0, categoriaScelta.prefabs.Count)];

        // 4. Spawna l'oggetto
        Instantiate(prefabScelto, puntoDiSpawn.position, Quaternion.identity);
    }

    // --- FUNZIONI PER CAMBIARE LIVELLO/DIFFICOLTA' ---

    public void CambiaVelocita(float nuoviSecondi)
    {
        tempoTraSpawn = nuoviSecondi;
    }

    public void AttivaCategoria(string nomeCategoria, bool stato)
    {
        foreach(var cat in categorie)
        {
            if(cat.nome == nomeCategoria)
            {
                cat.attiva = stato;
                return;
            }
        }
    }

    // --- AUTOMAZIONE EDITOR (MAGIA) ---
    // Questo codice funziona solo nell'Editor di Unity per caricare i file dalle cartelle
#if UNITY_EDITOR
    [ContextMenu("Carica Prefab dalle Cartelle")]
    void CaricaPrefabAutomaticamente()
    {
        string pathBase = "Assets/Prefabs/Rifiuti"; // Il percorso che mi hai dato
        string[] nomiCartelle = { "Carta", "Plastica", "Speciale", "Umido", "Vetro" };

        categorie = new List<CategoriaRifiuto>();

        foreach (string nomeCartella in nomiCartelle)
        {
            CategoriaRifiuto nuovaCat = new CategoriaRifiuto();
            nuovaCat.nome = nomeCartella;
            nuovaCat.prefabs = new List<GameObject>();

            // Cerca tutti i prefab nella cartella specifica
            string fullPath = pathBase + "/" + nomeCartella;
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { fullPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (prefab != null)
                {
                    nuovaCat.prefabs.Add(prefab);
                }
            }
            
            categorie.Add(nuovaCat);
            Debug.Log($"Caricati {nuovaCat.prefabs.Count} prefabs per la categoria {nomeCartella}");
        }
    }
#endif
}