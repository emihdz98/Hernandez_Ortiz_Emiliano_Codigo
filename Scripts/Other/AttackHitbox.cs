using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para detectar colisiones de ataques, ya sean del enemigo o del jugador. 
*/

public class AttackHitbox : MonoBehaviour
{
    //Declaración de variables
    [SerializeField] private float hitbox;
    
    public float damage;

    [SerializeField] private LayerMask layer;


    void Update()
    {
        //Se crea una esfera que detectará colisiones.
        Collider[] objective = Physics.OverlapSphere(transform.position, hitbox, layer);
        foreach(Collider e in objective)
        {
            if(e.tag == "Enemy") //Si detecta una colisión con un enemigo...
            {
                e.GetComponent<EnemyHealthSystem>().Damage(damage); //Activará el método de daño de su respectivo script.
            }
            else if(e.tag == "Player") //Si detecta una colisión con el jugador...
            {
                e.GetComponent<PlayerHealthSystem>().Damage(damage); //Activará el método de daño de su respectivo script.
            }
        }

    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitbox);
    }
}
