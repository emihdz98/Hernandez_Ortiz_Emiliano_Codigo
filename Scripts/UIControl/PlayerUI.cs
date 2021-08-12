using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve mostrar los datos de vida, energía y coleccionables del jugador en la UI.
*/

public class PlayerUI : MonoBehaviour
{
    public Text healthTXT;
    public Text staminaTXT;
    public Text collectibleTXT;

    PlayerHealthSystem player;

    void Awake()
    {
        player=GetComponentInParent<PlayerHealthSystem>(); 
        //Convierte los datos a string para poder ser mostrados en un cuadro de texto
        healthTXT.text=player.currentHealth.ToString();
        staminaTXT.text=player.currentStamina.ToString();
    }

    void Update()
    {
        //Convierte los datos a string para poder ser mostrados en un cuadro de texto
        healthTXT.text=player.currentHealth.ToString();
        staminaTXT.text=player.currentStamina.ToString();
        collectibleTXT.text=player.collectibles.ToString(); //Este último solo será mostrado en la pantalla de nivel completado.
    }
}
