using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Movimiento horizontal
    public float direction;
    public float speed;
    public Rigidbody2D rb;

    // Salto 
    public float jumpForce;
    public bool canJump;

    // DSuelo
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    // Animación
    public Animator playerAnimator;
    public bool isFacingRight;

    // Vida 
    public float health;
    [SerializeField] private float maxHealth;

    // Golpe
    public float hitTime;       // Duración del empuje (segundos). Mientras sea > 0 no hay control.
    public float hitForceX;     // Fuerza del empuje en X
    public float hitForceY;     // Fuerza del empuje en Y
    public bool hitFromRight;   // true = el golpe vino de la derecha

    // UI
    public TextMeshProUGUI healthText;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        UpdateHealthUI();
    }

    void Update()
    {
        
        canJump = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

      
        if (hitTime <= 0)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocityY);

        
            playerAnimator.SetFloat("Direction", Mathf.Abs(direction));
        }
        else
        {
     
            float dir = hitFromRight ? -1f : 1f;
            rb.linearVelocity = new Vector2(dir * hitForceX, rb.linearVelocityY);

            hitTime -= Time.deltaTime;
        }


        if (!isFacingRight && direction > 0f)
        {
            Flip();
        }
        else if (isFacingRight && direction < 0f)
        {
            Flip();
        }
    }

   
    public void Move(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>().x;
    }

  
    public void Jump(InputAction.CallbackContext context)
    {
        if (canJump && context.performed)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    
    public void TakeDamage(float damage, float forceX, float forceY, float time, bool fromRight)
    {
        health = Mathf.Max(health - damage, 0f);

        hitForceX = forceX;
        hitForceY = forceY;
        hitTime = time;
        hitFromRight = fromRight;

        if (time > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, forceY);
        }

        playerAnimator.SetTrigger("Hit");
        UpdateHealthUI();
    }

    public void AddHealth(float _health)
    {
        health = Mathf.Min(health + _health, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthText.text = $"Health: {health}/{maxHealth}";
    }
}
