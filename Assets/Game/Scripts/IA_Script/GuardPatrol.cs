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
    private Vector2 lastSeenPosition;  // Dernière position connue du joueur
    public float visionAngle = 60f;   // Angle de vision du garde
    public float visionRange = 5f;    // Plage de vision du garde
    public LayerMask playerLayer; // Layer du joueur pour le Raycast
    public float visionConeLength = 3f;  // Longueur du cône de vision

    private LineRenderer lineRenderer; // Référence au LineRenderer pour dessiner le cône

    void Start()
    {
        // Initialiser le LineRenderer
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
        // Débogage de l'état actuel
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

        // Débogage de l'angle et distance
        Debug.Log("Angle de vue : " + angle);
        Debug.Log("Distance joueur - garde : " + Vector2.Distance(transform.position, player.position));

        // Vérifier si le joueur est dans le cône de vision et dans la portée de détection
        if (Vector2.Distance(transform.position, player.position) < alertDistance && angle < visionAngle / 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, alertDistance, playerLayer);
            if (hit.collider != null && hit.collider.transform == player)
            {
                Debug.DrawLine(transform.position, player.position, Color.green); // Débogage pour voir le Raycast
                Debug.Log("Raycast a détecté le joueur !");
                return true;
            }
            else
            {
                Debug.DrawLine(transform.position, hit.point, Color.red); // Si un obstacle bloque la vue
                Debug.Log("Raycast a frappé un obstacle.");
            }
        }
        return false;
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
            // Rotation instantanée vers le prochain waypoint
            Vector2 nextWaypointDirection = waypoints[(currentWaypointIndex + 1) % waypoints.Length].position - transform.position;
            float angleToWaypoint = Mathf.Atan2(nextWaypointDirection.y, nextWaypointDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angleToWaypoint);

            // Passer au prochain waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // Vérifier si le joueur est visible
        if (CanSeePlayer())
        {
            Debug.Log("Joueur détecté ! Passage en mode Alerte !");
            lastSeenPosition = player.position;  // Sauvegarde de la dernière position vue
            currentState = GuardState.Alerted;    // Changer d'état en Alerted
        }
    }

    void Alert()
    {
        Debug.Log("Mode Alerte : L'IA se dirige vers l'intrus !");

        // Se diriger vers le joueur
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        // Si le joueur est assez proche, on passe en mode Attaque
        if (Vector2.Distance(transform.position, player.position) < attackDistance)
        {
            Debug.Log("Attaque !");
            currentState = GuardState.Attacking;
        }

        // Si le joueur s'éloigne, on passe en mode Recherche
        if (Vector2.Distance(transform.position, player.position) > alertDistance)
        {
            Debug.Log("L'intrus a disparu... Recherche en cours !");
            currentState = GuardState.Searching;
        }
    }

    void Search()
    {
        Debug.Log("Mode Recherche : Le garde cherche l'intrus...");

        // Se déplacer vers la dernière position connue
        transform.position = Vector2.MoveTowards(transform.position, lastSeenPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, lastSeenPosition) < 0.5f)
        {
            Debug.Log("Aucun intrus trouvé, retour à la patrouille.");
            Invoke("ReturnToPatrol", 3f); // Pause avant de retourner en patrouille
        }
    }

    void ReturnToPatrol()
    {
        Debug.Log("Retour en patrouille.");
        currentState = GuardState.Patrolling;  // Retourner en mode patrouille
    }

    void Attack()
    {
        Debug.Log("Mode Attaque : L'IA attaque l'intrus !");

        // Ici tu peux ajouter des animations, des dégâts, etc.

        // Après l'attaque, on retourne en patrouille
        currentState = GuardState.Patrolling;
    }

    // Fonction pour dessiner le cône de vision avec LineRenderer
    void DrawVisionCone()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;  // L'orientation de l'IA

        // Calculer les bords du cône
        float coneAngle = visionAngle / 2f;
        Vector2 leftEdge = RotateVector(direction, -coneAngle) * visionConeLength;
        Vector2 rightEdge = RotateVector(direction, coneAngle) * visionConeLength;

        // Définir les points du LineRenderer pour dessiner le cône
        lineRenderer.SetPosition(0, origin); // Point de départ
        lineRenderer.SetPosition(1, origin + leftEdge); // Bord gauche
        lineRenderer.SetPosition(2, origin + rightEdge); // Bord droit
    }

    // Fonction pour faire tourner un vecteur
    Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        return new Vector2(cos * vector.x - sin * vector.y, sin * vector.x + cos * vector.y);
    }
}
