using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Obtener referencia al script del jugador
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Comprobar si el jugador comenzó a moverse antes de activar el movimiento del enemigo
        if (playerController != null && playerController.hasStartedMoving)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
}
