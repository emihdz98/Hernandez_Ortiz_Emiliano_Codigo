using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script crea una clase constructora para los items. 
*/

[System.Serializable]
public class ItemConstructor
{
    //Atributos
    public int itemID;
    public string itemType;
    public float itemValue;

    public ItemConstructor(int id, string t, float v)
    {
        this.itemID = id;
        this.itemType = t;
        this.itemValue = v;
    }
}
