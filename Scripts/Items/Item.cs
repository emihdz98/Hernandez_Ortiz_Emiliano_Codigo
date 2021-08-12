using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script asigna los atributos de la base al item y con base a esos datos determina el efecto que tiene sobre el jugador.
También crea un estado en el que el item se mueve hacia el jugador para simular que el item en cuestion fue jalado por el jugador. 
*/

public class Item : MonoBehaviour
{
    public enum ItemState
    {
        StaticItem,
        MovingItem
    }

    #region  Item Attributes
    public int itemID;
    public string type;
    public float value;
    #endregion

    #region Components and scripts
    ItemDatabase ItemBase;
    public ItemState currentState;
    Rigidbody rb;
    Transform direction;
    #endregion

    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        ItemBase = GameObject.FindObjectOfType<ItemDatabase>();
        LoadData(itemID);
        //Establece el estado inicial del item.
        currentState = ItemState.StaticItem;
    }

    void Update()
    {
        switch (currentState)
        {
            default:
            //El item se encuentra inmovil
            case ItemState.StaticItem:
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            break;
            //El item se mueve en dirección al jugador.
            case ItemState.MovingItem:
                Vector3 moveDirection = (direction.position - this.transform.position).normalized;
                rb.AddForce(moveDirection, ForceMode.VelocityChange);
                if(Vector3.Distance(transform.position,direction.position) < 5f)
                    currentState = ItemState.StaticItem;
            break;
        }
    }

    void LoadData(int id) //Método que asigna los atributos a cada item.
    {
        for(int i = 0; i < ItemBase.newItem.Length; i++)
        {
            if(ItemBase.newItem[i].itemID == id)
            {
                this.type = ItemBase.newItem[i].itemType;
                this.value = ItemBase.newItem[i].itemValue;
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player") //El item solo tendrá efecto si colisiona con el jugador.
        {
            switch(type)
            {
                //Si el item es de tipo vida, otorgará puntos de vida al jugador siempre y cuando este no tenga su vida al máximo,
                //de lo contrario el item no podrá ser recogido por el jugador.
                case "healthItem":
                    if(other.GetComponent<PlayerHealthSystem>().currentHealth < other.GetComponent<PlayerHealthSystem>().maxHealth)
                    {
                        other.GetComponent<PlayerHealthSystem>().Heal(value);
                        Destroy(this.gameObject);
                    }
                break;
                //Si el item es de tipo energía, otorgará puntos de energía al jugador siempre y cuando este no tenga su energía al máximo,
                //de lo contrario el item no podrá ser recogido por el jugador.
                case "staminaItem":
                    if(other.GetComponent<PlayerHealthSystem>().currentStamina < other.GetComponent<PlayerHealthSystem>().maxStamina)
                    {
                        other.GetComponent<PlayerHealthSystem>().GainStamina(value);
                        Destroy(this.gameObject);
                    }
                break;
                //Si el item es de tipo coleccionable, añadirá un coleccionable al jugador.
                case "collectible":
                    other.GetComponent<PlayerHealthSystem>().CollectItem(value);
                    Destroy(this.gameObject);
                break;
            }
        }
    }

    //Este método es llamado desde otro script y sirve para determinar la dirección en la que el item se moverá.
    public void SetItemDirection(Transform d)
    {
        direction = d;
        currentState = ItemState.MovingItem;
    }
}
