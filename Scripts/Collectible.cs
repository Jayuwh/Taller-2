using UnityEngine;
using Unity.Cinemachine;

public class Collectible : MonoBehaviour
{
    [Header("Tipo y valor")]
    public bool isDamage;            
    public float amount = 10f;     

    [Header("Cámara (solo coleccionables de daño)")]
    public float impulseForce = 1f;  

    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null) return;

        if (isDamage)
        {
            
            player.TakeDamage(amount, 0f, 0f, 0f, false);

            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(impulseForce);
            }
        }
        else
        {
            player.AddHealth(amount);
        }

        Destroy(gameObject);
    }
}