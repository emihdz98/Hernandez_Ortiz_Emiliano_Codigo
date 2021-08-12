using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script se encarga de almacenar los atributos de los diferentes items que hay en el juego. 
*/

public class ItemDatabase : MonoBehaviour
{
    public ItemConstructor[] newItem;
    void Awake()
    {
        newItem = new ItemConstructor[5];

        newItem[0] = new ItemConstructor(1, "healthItem", 50);
        newItem[1] = new ItemConstructor(2, "healthItem", 100);
        newItem[2] = new ItemConstructor(3, "healthItem", 200);
        newItem[3] = new ItemConstructor(4, "staminaItem", 25);
        newItem[4] = new ItemConstructor(5, "collectible", 1);
    }
}
