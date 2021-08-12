using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para activar y desactivar game objects desde animation events. Principalmente lo usé para activar y desactivar las 
armas del jugador y las hitbox de los ataques tanto del jugador como del enemigo.
*/

public class ActiveObjectCtrl : MonoBehaviour
{
    public GameObject[] obj; 
    //La creación de un array me permitió gestionar las hitbox de cada ataque realizado por el jugador,
    //así como hacer aparecer las armas cuando el jugador las usa.

    public void ActivateObj(int n)
    {
        obj[n].SetActive(true);
    }
    public void DeactivateObj(int n)
    {
        obj[n].SetActive(false);
    }

}
