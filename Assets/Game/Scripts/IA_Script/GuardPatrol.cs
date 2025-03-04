using UnityEngine;

public class GuardPatrol : MonoBehaviour
{
    public enum GuardState
    {
        Patrolling,
        Alerted,
        Searching,
        Attacking
    }

    public Transform[] waypoints;  // Liste des points de patrouille
    public float speed = 2f;       // Vitesse de déplacement du garde
    private int currentWaypointIndex = 0;  // Indice du waypoint actuel
    private GuardState currentState = GuardState.Patrolling; // L'état initial est la patrouille
    public Transform player;   // Référence au joueur
    public float alertDistance = 5f;  // Distance à laquelle l'IA détecte le joueur
    public float attackDistance = 1f; // Distance pour attaquer
    public float stopDistance = 2f; // Distance à laquelle l'IA s'arrête avant d'attaquer
    private Vector2 lastSeenPosition;  // Dernière position connue du joueur
    public float visionAngle = 60f;   // Angle de vision du garde
    public float visionRange = 5f;    // Plage de vision du garde
    public LayerMask playerLayer; // Layer du joueur pour le Raycast
    private LineRenderer lineRenderer; // Référence au LineRenderer pour dessiner le cône

    void Start()
    {
        // Initialiser le LineRenderer pour la visualisation du cône de vision
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 3;  // Trois points : deux bords et le centre
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        // Debugging pour l'état actuel
        Debug.Log("État actuel de l'IA : " + currentState.ToString());

        switch (currentState)
        {
            case GuardState.Patrolling:
                Patrol();
                break;
            case GuardState.Alerted:
                Alert();
                break;
            case GuardState.Searching:
                Search();
                break;
            case GuardState.Attacking:
                Attack();
                break;
        }

        // Affichage du cône de vision
        DrawVisionCone();
    }

    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector2.Angle(transform.right, directionToPlayer);

        // Vérifier si le joueur est dans le cône de vision et dans la portée de détection
        if (Vector2.Distance(transform.position, player.position) < alertDistance && angle < visionAngle / 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, alertDistance, playerLayer);
            if (hit.collider != null && hit.collider.transform == player)
            {
                return true;  // Joueur détecté
            }
        }
        return false;  // Joueur non détecté
    }

    void Patrol()
    {
        if (waypoints.Length == 0)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector2 direction = targetWaypoint.position - transform.position;
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Si l'IA atteint le waypoint, on la fait tourner instantanément vers la nouvelle direction
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            Vector2 nextWaypointDirection = waypoints[(currentWaypointIndex + 1) % waypoints.Length].position - transform.position;
            float angleToWaypoint = Mathf.Atan2(nextWaypointDirection.y, nextWaypointDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angleToWaypoint);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // Si le joueur est détecté, passer en mode Alerte
        if (CanSeePlayer())
        {
            lastSeenPosition = player.position;  // Sauvegarder la dernière position connue
            currentState = GuardState.Alerted;    // Passer en mode Alerte
        }
    }

    void Alert()
    {
        // Se diriger vers le joueur, mais s'arrêter à une distance sécuritaire
        if (Vector2.Distance(transform.position, player.position) > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }

        // Si l'IA est à la distance d'attaque, passer en mode Attaque
        if (Vector2.Distance(transform.position, player.position) <= attackDistance)
        {
            currentState = GuardState.Attacking;
        }

        // Si le joueur s'éloigne trop, passer en mode Recherche
        if (Vector2.Distance(transform.position, player.position) > alertDistance)
        {
            currentState = GuardState.Searching;
        }
    }

    void Search()
    {
        // Se déplacer vers la dernière position connue
        transform.position = Vector2.MoveTowards(transform.position, lastSeenPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, lastSeenPosition) < 0.5f)
        {
            currentState = GuardState.Patrolling;  // Retour à la patrouille
        }
    }

    void Attack()
    {
        // Effectuer l'attaque (ici c'est une simple transition)
        Debug.Log("Attaque du joueur !");
        // Vous pouvez ajouter des mécaniques d'attaque ici (tirer, dégâts, etc.)

        // Après l'attaque, revenir en mode patrouille
        currentState = GuardState.Patrolling;
    }

    void DrawVisionCone()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        if (currentState == GuardState.Attacking)
        {
            // Changer la vision en cercle pendant l'attaque
            visionAngle = 360f;  // Vision circulaire pour l'attaque
        }

        // Dessiner un cône de vision (ou cercle en mode Attaque)
        float coneAngle = visionAngle / 2f;
        Vector2 leftEdge = RotateVector(direction, -coneAngle) * visionRange;
        Vector2 rightEdge = RotateVector(direction, coneAngle) * visionRange;

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, origin + leftEdge);
        lineRenderer.SetPosition(2, origin + rightEdge);
    }

    Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);
        return new Vector2(cos * vector.x - sin * vector.y, sin * vector.x + cos * vector.y);
    }
}
