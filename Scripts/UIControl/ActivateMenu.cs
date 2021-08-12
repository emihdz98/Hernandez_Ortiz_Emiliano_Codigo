using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para crear un menú de pausa y permitir que se pueda reiniciar o salir del juego.
Parte de este código lo extraje del canal "Brackeys", para ver como se hacía un menú de pausa.
*/


public class ActivateMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseScreen;
    public GameObject mainScreen;

    void Start() 
    {
        //El tiempo siempre se reestablece a su velocidad por defecto, ya que al morir o completar el nivel el tiempo se detiene.
        Time.timeScale = 1f; 
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) //Al presionar la tecla enter, se alterna entre dos métodos.
        {
            if(GameIsPaused)
                Resume();
            else
                Pause();
        }
        if(Input.GetKeyDown(KeyCode.Backspace)) //La tecla back reinicia el nivel.
            SceneManager.LoadScene("DemoLevel");
        if(Input.GetKeyDown(KeyCode.Escape)) //La tecla esc cierra el juego.
            Application.Quit();
    }

    void Resume()
    {
        pauseScreen.SetActive(false); //Desactiva la pantalla de pausa.
        mainScreen.SetActive(true); //Activa la pantalla principal del juego.
        Time.timeScale = 1f; //El tiempo se reestablece a su velocidad normal.
        GameIsPaused = false; //Variable que identifica si el juego está pausado.
    }

    void Pause()
    {
        pauseScreen.SetActive(true); //Activa la pantalla de pausa.
        mainScreen.SetActive(false); //Desactiva la pantalla principal del juego.
        Time.timeScale = 0f; //El tiempo se detiene.
        GameIsPaused = true;
    }
}
