using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para generar la IA base de los enemigos. 
*/

public class EnemyAI : MonoBehaviour
{
    public enum AIstate
    {
        Idle,
        Move,
        Attack,
        Knockback,
        Dead
    }

    public AIstate currentState;

    public Transform target;
    public float attackRate;
    private bool attackCooldown;

    public Animator animator;

    private NavMeshAgent agent;

    [SerializeField] private float knockbackTime;
    public bool isKnocked;

    public GameObject[] dropItems;

    EnemyRadar radar;
    EnemyHealthSystem health;
    ActiveObjectCtrl objectCtrl;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        radar = GetComponentInParent<EnemyRadar>();
        health = GetComponentInParent<EnemyHealthSystem>();
        objectCtrl = GetComponentInParent<ActiveObjectCtrl>();
        //Se establecen valores iniciales.
        currentState = AIstate.Idle;
        isKnocked = false;
    }

    void Update()
    {
        switch(currentState)
        {
            default:
                
            case AIstate.Idle:
                isKnocked = false;
                radar.enabled = true;
                animator.SetBool("run", false);
                //Se establece como destino la posición propia del enemigo para prevenir que este siga avanzando aún si el
                //jugador sale de su radar.
                agent.SetDestination(transform.position);
            break;

            case AIstate.Move:
                //Para el movimiento del enemigo me basé en un video del canal "Brackeys", el cuál explica como usar el
                //componente NavMesh.
                float distance = Vector3.Distance(target.position, transform.position); //Se saca la distancia entre el enemigo y el jugador.
                Vector3 direction = target.position - transform.position; //Se crea un vector con dicha distancia.
                animator.SetBool("run", true);
                agent.SetDestination(target.position); //Se establece como destino la posición del jugador.
                FaceTarget();
            break;

            case AIstate.Attack:
                animator.SetBool("run", false);
                //Se establece como destino la posición propia del enemigo para que este frene al alcanzar al jugador.
                agent.SetDestination(transform.position); 
                FaceTarget(); //Método para que el enemigo se mantenga viendo al juagdor.
                //La variable bool "attackCooldown" identifica si ha pasado el tiempo de enfriamiento entre ataques.
                if (!attackCooldown) 
                {
                    animator.SetTrigger("attack");
                    attackCooldown = true; //Se activa el cooldown de ataques.
                    Invoke(nameof(ResetAttack), attackRate); //Y este cooldown se desactivará al pasar un tiempo establecido.
                }    
            break;
                
            case AIstate.Knockback:                
                if(!isKnocked)
                {
                    animator.Play("Hurt");
                    Invoke(nameof(IdleState), knockbackTime); //Una vez pase el tiempo de aturdimiento el enemigo regresa a su estado normal.
                    isKnocked = true; //Evita que el enemigo sea aturdido más de una vez durante el tiempo de aturdimiento.
                }
            break;

            case AIstate.Dead:
                objectCtrl.DeactivateObj(0); //Desactiva el collider que causa daño al jugador, en caso de que el enemigo muera mientras hace un ataque.
                agent.SetDestination(transform.position); //Frena el movimiento del enemigo.             
                animator.Play("Death");
            break;
        }
    }

    //Los siguientes métodos sirven para que el estado del enemigo pueda ser cambiado desde otro script o un animation event.
    //Esto lo hice para solucionar el error que tuve con los enemigos duplicados.
    public void IdleState()
    {
        currentState = AIstate.Idle;
    }

    public void MoveState()
    {
        currentState = AIstate.Move;
    }

    public void AttackState()
    {
        currentState = AIstate.Attack;
    }

    public void KnockbackState()
    {
        radar.enabled = false;
        currentState = AIstate.Knockback;
    }

    public void DeadState()
    {   
        radar.enabled = false;
        health.enabled = false;  
        currentState = AIstate.Dead;
    }

    private void ResetAttack()
    {
        attackCooldown = false; //Se desactiva el cooldown del ataque.
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//

    void FaceTarget()
    {
        //Método que hace que el enemigo mire en dirección a su objetivo.
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3 (direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Die()
    {
        //Este método es activado desde un animation event.
        //Genera un item aleatorio de un array para simular un "drop" al morir.
        int randItem = Random.Range(0, dropItems.Length);
        Instantiate(dropItems[randItem], this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }
}
