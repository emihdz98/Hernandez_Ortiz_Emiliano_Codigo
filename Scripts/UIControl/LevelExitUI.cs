using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para mostrar una pantalla de "Nivel completado" al llegar a la salida del nivel.
*/

public class LevelExitUI : MonoBehaviour
{
    public GameObject finalScreen;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            finalScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
