using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para crear radares que definirán el comportamiento del enemigo. 
*/

public class EnemyRadar : MonoBehaviour
{
    #region Range variables
    public float detectRange;
    public float attackRange;
    #endregion

    #region Components and scripts
    EnemyAI state;
    public LayerMask layer;
    #endregion

    void Start()
    {
        state = GetComponentInParent<EnemyAI>();
    }


    void Update()
    {
        //Se crean los radares para detectar y atacar al jugador.
        bool detectRadar = Physics.CheckSphere(transform.position, detectRange, layer);
        bool attackRadar = Physics.CheckSphere(transform.position, attackRange, layer);

        if(detectRadar == true && attackRadar == false) //Si el jugador se encuentra dentro del primer radar...
        {
            state.MoveState(); //Se activa el estado Move del script de IA.
        }
        else if (detectRadar == true && attackRadar == true) //Si el jugador se encuentra dentro del primer y segundo radar...
        {
            state.AttackState(); //Se activa el estado Attack del script de IA.
        }
        else
        {
            state.IdleState(); //De lo contrario se activa el estado Idle del script de IA.
        }
    }

    private void OnDrawGizmos() //Crea la visualización del radar.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
