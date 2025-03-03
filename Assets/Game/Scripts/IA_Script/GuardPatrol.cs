using UnityEngine;

public class GuardPatrol : MonoBehaviour
{
    public Transform[] waypoints;  // Liste des points de patrouille
    public float speed = 2f;       // Vitesse de déplacement du garde
    private int currentWaypointIndex = 0;  // Indice du waypoint actuel

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        // Vérifier si nous avons des points de patrouille
        if (waypoints.Length == 0)
            return;

        // Se déplacer vers le point de patrouille actuel
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector2 direction = targetWaypoint.position - transform.position; // Direction vers le point
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Si le garde atteint le point de patrouille
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // Passer au prochain point de patrouille
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // (Optionnel) Faire en sorte que le garde regarde dans la direction de son mouvement
        if (direction.x > 0) // Si le garde se déplace vers la droite
        {
            transform.localScale = new Vector3(1, 1, 1);  // Regarder vers la droite
        }
        else if (direction.x < 0) // Si le garde se déplace vers la gauche
        {
            transform.localScale = new Vector3(-1, 1, 1);  // Regarder vers la gauche
        }
    }
}
