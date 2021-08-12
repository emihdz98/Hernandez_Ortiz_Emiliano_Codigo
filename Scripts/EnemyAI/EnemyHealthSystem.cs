using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para gestionar el sistema de salud del enemigo, así como alterar su estado en el script de IA. 
*/

public class EnemyHealthSystem : MonoBehaviour
{
    #region Health variables
    public float maxHealth;
    public float currentHealth;
    #endregion

    #region Damage cooldown variables
    //Las variables de cooldown sirven para evitar que el enemigo detecte más colisiones de las que debería, basicamente le
    //da un momento de invulnerabilidad tras recibir un golpe.
    public float damageCooldown;
    public float cooldownCounter;
    #endregion

    #region Scripts
    EnemyAI state;
    #endregion

    void Start()
    {
        state = GetComponentInParent<EnemyAI>();
        //Se establecen valores iniciales.
        currentHealth = maxHealth;
    }

    void Update()
    {
        DamageCooldown();
    }

    private void DamageCooldown()
    {
        if(cooldownCounter > 0) //Si el contador de cooldown es mayor a 0...
        {
            cooldownCounter -= Time.deltaTime; //El contador se reducirá hasta llegar a 0.
        }
    }

    public void Damage(float d) //Este método es llamado por el script del hitbox del jugador
    {
        if(cooldownCounter <= 0) //Si el contador de cooldown está en 0...
        {
            currentHealth -= d; //Se reduce cierta cantidad de salud.
            
            state.KnockbackState(); //Se activa el estado Knockback del script de IA.

            cooldownCounter = damageCooldown; //Se le da un tiempo de cooldown al contador.
        }
        
        if(currentHealth <= 0) //Si la salud llega o se pasa de 0...
        {
            state.DeadState(); //Se activa el estado Death del script de IA.
        }
    }
}
