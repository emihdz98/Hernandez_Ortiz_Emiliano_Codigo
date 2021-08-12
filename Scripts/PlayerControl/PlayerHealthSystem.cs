using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para gestionar el sistema de salud y energía del jugador, así como alterar su estado en el script de control. 
*/

public class PlayerHealthSystem : MonoBehaviour
{
    #region Health variables
    public float maxHealth;
    public float currentHealth;
    #endregion

    #region Stamina variables
    public float maxStamina;
    public float currentStamina;
    #endregion

    #region Collectible variables
    public float collectibles;
    #endregion

    #region Damage Cooldown variables
    //Las variables de cooldown sirven para evitar que el jugador detecte más colisiones de las que debería, basicamente le
    //da un momento de invulnerabilidad tras recibir un golpe.
    [SerializeField] private float damageCooldown;
    private float cooldownCounter;
    #endregion

    #region Components and scripts
    public PlayerController player;
    public GameObject deathScreen;
    #endregion

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        //Se establecen valores iniciales.
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        collectibles = 0;     
    }

    void Update() 
    {
        if(cooldownCounter > 0) //Si el contador de cooldown es activado...
        {
            cooldownCounter -= Time.deltaTime; //Se irá reduciendo hasta llegar a 0.
        }

        if(currentHealth <= 0) //Si la vida llega a 0...
        {
            if(currentStamina > 0) //...y la energía es mayor a 0...
            {
                //Se drenará la stamina restante para transformarse en puntos de vida.
                currentHealth = currentStamina;
                currentStamina = 0;
            }
            //Si estas condiciones no se cumplen el jugador morirá.
            else
                player.currentState = PlayerController.PlayerStates.Dead;
        }
    }

    //Los siguientes métodos serán llamados desde otro script o desde un animation event:
    public void Inmunity(float i)
    {
        cooldownCounter = i; //Activa el contador de cooldown para que el jugador no reciba daño.
    }

    public void Damage(float d)
    {
        if(cooldownCounter <= 0) //Si el contador de cooldown está desactivado...
        {
            currentHealth -= d; //Se reduce una cantidad específica de vida al jugador.
            player.currentState = PlayerController.PlayerStates.Knockback; //Se entra en estado de aturdimiento.

            cooldownCounter = damageCooldown; //Se reinicia el contador de cooldown.
        }        
    }

    public void Heal(float h)
    {
        currentHealth += h; //El jugador recibe puntos de salud.
        if(currentHealth > maxHealth) //Pero no puede exceder su salud máxima.
        {
            currentHealth = maxHealth;
        }
    }

    public void GainStamina(float s)
    {
        currentStamina += s; //El jugador recibe puntos de energía.
        if(currentStamina > maxStamina) //Pero no puede exceder su energía máxima.
        {
            currentStamina = maxStamina;
        }
    }

    public void CollectItem(float c)
    {
        collectibles += c; //El jugador recibe un item coleccionable.
    }

    public void Die()
    {
        deathScreen.SetActive(true); //Se activa la pantalla de "moriste".
        Time.timeScale = 0f; //Se pausa el tiempo del juego.
    }
}
