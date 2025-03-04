using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Vitesse du joueur

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // R�cup�rer le Rigidbody2D du joueur
    }

    void Update()
    {
        // R�cup�rer l'entr�e du clavier pour le mouvement (ZQSD)
        movement.x = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
        movement.y = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
    }

    void FixedUpdate()
    {
        // D�placer le joueur en appliquant la vitesse
        rb.linearVelocity = movement * speed;
    }
}

