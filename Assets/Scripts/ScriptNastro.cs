using UnityEngine;
using System.Collections.Generic;

public class ScriptNastro : MonoBehaviour
{
    [Header("Impostazioni Globali")]
    public float speed = 0.2f;

    [Header("Collegamenti")]
    public List<Rigidbody> nastriMobili; 

    void FixedUpdate()
    {
        foreach (Rigidbody rb in nastriMobili)
        {
            if (rb != null)
            {
                Vector3 pos = rb.position;
                
                // --- MODIFICA QUI ---
                // Prima era: transform.forward (Direzione del Padre NastroFinale)
                // Adesso è: rb.transform.forward (Direzione del singolo pezzo di gomma)
                
                // Nota: Se vanno al contrario, cambia il "-=" in "+="
                rb.position -= rb.transform.forward * speed * Time.fixedDeltaTime;
                
                rb.MovePosition(pos);
            }
        }
    }
}