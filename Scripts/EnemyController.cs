using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Vector2 movement;

    [Header("Patrulla")]
    public float enemySpeed = 2f;
    public Rigidbody2D enemyRb;
    public float detectionRadius = 0.3f;
    public Transform actualObjective;
    public Transform[] enemyMovementPoints;   // Tamaño 2: inicio y fin de la patrulla
    public Animator enemyAnimator;
    public bool isFacingRight;                // Marca si el sprite mira a la derecha por defecto

    [Header("Daño al jugador (distinto en cada enemigo)")]
    public float enemyDamage;           // Vida que le quita al jugador
    public float enemyHitStrengthX;     // Empuje en X
    public float enemyHitStrengthY;     // Empuje en Y
    public float enemyHitTime;          // Duración del knockback (segundos)

    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();

        actualObjective = enemyMovementPoints[0];
    }

    void Update()
    {
        // 1. Distancia HORIZONTAL al objetivo (ignora la altura de los puntos)
        float distanceToObjective = Mathf.Abs(transform.position.x - actualObjective.position.x);

        // 2. Si llegó, cambia de punto (esto crea el ir y venir)
        if (distanceToObjective < detectionRadius)
        {
            if (actualObjective == enemyMovementPoints[0])
            {
                actualObjective = enemyMovementPoints[1];
            }
            else
            {
                actualObjective = enemyMovementPoints[0];
            }
        }

        // 3. Dirección hacia el objetivo (-1 izquierda, 1 derecha)
        float diffX = actualObjective.position.x - transform.position.x;
        int roundDirection = diffX > 0 ? 1 : -1;
        movement = new Vector2(roundDirection, 0);

        // 4. Voltear sprite según hacia dónde camina
        if (roundDirection < 0 && isFacingRight)
        {
            Flip();
        }
        else if (roundDirection > 0 && !isFacingRight)
        {
            Flip();
        }

        // 5. Animación
        enemyAnimator.SetFloat("Direction", Mathf.Abs(roundDirection));

        // 6. Movimiento
        enemyRb.MovePosition(enemyRb.position + movement * enemySpeed * Time.deltaTime);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            // Si el jugador está a la izquierda del enemigo, el golpe le llega por la derecha
            bool hitFromRight = collision.transform.position.x <= transform.position.x;

            player.TakeDamage(enemyDamage, enemyHitStrengthX, enemyHitStrengthY, enemyHitTime, hitFromRight);

            // El movimiento de cámara lo genera el componente
            // Cinemachine Collision Impulse Source de este mismo enemigo.
        }
    }
}