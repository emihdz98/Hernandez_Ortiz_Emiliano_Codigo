using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para establecer los controles de combate del jugador.
Para el sistema de combos me basé completamente en un video del canal "Team Pig", el cuál explica como hacer dicho sistema.
*/

public class PlayerCombatSystem : MonoBehaviour
{
    //Declaración de variables
    #region Numeric variables
    public int attackNumber;
    public int heavyAttackNumber;
    private bool canAttack;
    #endregion

    #region Components and scripts
    public Animator animator;
    PlayerController playerCtrl;
    PlayerHealthSystem playerHP;
    #endregion

    private void Awake() 
    {
        animator = GetComponent<Animator>();
        playerCtrl = GetComponent<PlayerController>();
        playerHP = GetComponent<PlayerHealthSystem>();
        //Se establecen valores iniciales.
        attackNumber = 0;
        heavyAttackNumber = 0;
        canAttack = true;
    }

    public void StartCombo()
    {
        if(Input.GetKeyDown(KeyCode.E)) //Al presionar la tecla E se iniciará el combo
        {
            if(canAttack)//Si el jugador tiene permitido atacar, aumentará el número de ataques.
                attackNumber++;            

            if(attackNumber == 1)
                animator.SetInteger("attack", 1);
        }
    }

    public void ComboCheck() //Este método es llamado desde un animation event.
    {
        canAttack = false; //Se desactiva la posibilidad de atacar.

        //Si el número de ataques realizados se queda en 1, se reactiva la posibilidad de atacar y se reinicia el contador de ataques.
        if(animator.GetCurrentAnimatorStateInfo(2).IsTag("A1") && attackNumber == 1)
        {
            animator.SetInteger("attack", 0);
            canAttack = true;
            attackNumber = 0;
        }
        //Si el número de ataques realizados es mayor o igual a 2, se reactiva la posibilidad de atacar y se pasa al siguiente ataque.        
        else if(animator.GetCurrentAnimatorStateInfo(2).IsTag("A1") && attackNumber >= 2)
        {
            animator.SetInteger("attack", 2);
            canAttack = true;
        }
        //Si el número de ataques realizados se queda en 2, se reactiva la posibilidad de atacar y se reinicia el contador de ataques.
        else if(animator.GetCurrentAnimatorStateInfo(2).IsTag("A2") && attackNumber == 2)
        {
            animator.SetInteger("attack", 0);
            canAttack = true;
            attackNumber = 0;
        }
        //Si el número de ataques realizados es mayor o igual a 3, se reactiva la posibilidad de atacar y se pasa al siguiente ataque.
        else if(animator.GetCurrentAnimatorStateInfo(2).IsTag("A2") && attackNumber >= 3)
        {
            animator.SetInteger("attack", 3);
            canAttack = true;
        }
        else if(animator.GetCurrentAnimatorStateInfo(2).IsTag("A3"))//Una vez finalizado el tercer ataque se reinicia todo.
        {
            animator.SetInteger("attack", 0);
            canAttack = true;
            attackNumber = 0;
        }
    }

    //Los siguientes dos métodos funcionan de la misma manera que los dos anteriores.
    public void StartHeavyCombo()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(canAttack)
                heavyAttackNumber++;            

            if(heavyAttackNumber == 1)
                animator.SetInteger("heavyAttack", 1);
        }
    }

    public void HeavyComboCheck()
    {
        canAttack = false;

        if(animator.GetCurrentAnimatorStateInfo(2).IsTag("HA1") && heavyAttackNumber == 1)
        {
            animator.SetInteger("heavyAttack", 0);
            canAttack = true;
            heavyAttackNumber = 0;
        }
        else if(animator.GetCurrentAnimatorStateInfo(2).IsTag("HA1") && heavyAttackNumber >= 2)
        {
            animator.SetInteger("heavyAttack", 2);
            canAttack = true;
        }
        else if(animator.GetCurrentAnimatorStateInfo(2).IsTag("HA2"))
        {
            animator.SetInteger("heavyAttack", 0);
            canAttack = true;
            heavyAttackNumber = 0;
        }
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//

    //Los siguientes dos métodos sirven para asignar los inputs de ataques especiales que al ser activados llamarán a un método de otro
    //script, el cuál consumirá puntos de energía almacenados por el jugador. 
    public void SpecialAttack()
    {
        if(Input.GetKeyDown(KeyCode.R) && playerHP.currentStamina >= 50)
        {
            playerHP.currentStamina -= 50;
            animator.SetTrigger("spAttack");  
        }
    }

    public void QuickShot()
    {
        if(Input.GetMouseButtonDown(0) && playerHP.currentStamina >= 200)
        {
            playerHP.currentStamina -= 200;
            animator.SetTrigger("quickShot");
        }
    }
}
