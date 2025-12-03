using UnityEngine;

public class BinController : MonoBehaviour
{
    [Tooltip("Scrivi qui il tag esatto che questo bidone deve accettare (es. Plastica)")]
    public string tagAccettato; 

    // Questa funzione parte in automatico quando qualcosa entra nel Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Controlliamo se l'oggetto che è entrato ha il Tag giusto
        if (other.gameObject.CompareTag(tagAccettato))
        {
            Debug.Log("✅ CORRETTO! Hai buttato " + other.gameObject.name);
            // Qui in futuro metteremo: Punteggio + 10
            
            // Distruggiamo il rifiuto per pulire la scena
            Destroy(other.gameObject);
        }
        else
        {
            Debug.Log("❌ ERRORE! Questo bidone non accetta " + other.gameObject.tag);
            // Qui in futuro metteremo: Suono di errore
            
            // Facciamo "sputare" fuori il rifiuto (opzionale)
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.linearVelocity = Vector3.up * 5; // Lo spinge in alto
            }
        }
    }
}