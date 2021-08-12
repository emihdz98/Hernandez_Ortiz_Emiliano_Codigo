using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para mostrar en pantalla cuanta vida tiene un enemigo.
*/

public class EnemyUI : MonoBehaviour
{
    public TMP_Text healthTMP;

    EnemyHealthSystem enemy;

    void Awake()
    {
        enemy=GetComponentInParent<EnemyHealthSystem>(); 

        healthTMP.text=enemy.currentHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        healthTMP.text=enemy.currentHealth.ToString();
    }
}
