using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Nombre completo: Emiliano Hernández Ortiz.
Asignatura: Programación orientada a objetos.
Este script sirve para generar los estados, controles y el movimiento del jugador.
*/

public class PlayerController : MonoBehaviour
{
    //Declaración de variables:
 
    public enum PlayerStates
    {
        Normal,
        Attack,
        HookshotFlying,
        Knockback,
        Still,
        Dead
    }

    public PlayerStates currentState;

    #region Basic movement variables
    [Header ("Movement values")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float rotationSpeed;  
    #endregion

    #region Jumping system variables
    [Header ("Jump values")]
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float jumpGracePeriod;
    [SerializeField] private float gravity;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;

    private bool canDoubleJump = false;
    #endregion
   
    #region Knockback variables
    [Header ("Knockback values")]
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackTime;
    #endregion

    #region Evade roll variables;
    [Header ("Evade roll values")]
    [SerializeField] private float rollSpeed;
    [SerializeField] private float rollTime;
    #endregion

    #region Hookshot system variables;
    [Header ("Hookshot values")]
    private Vector3 hookshotPosition;
    [SerializeField] private LineRenderer chain;
    [SerializeField] private Transform hook;
    #endregion
    
    #region Component variables
    [Header ("Components")]
    [SerializeField] private Camera mainCam;    
    [SerializeField] private GameObject defaultCam;    
    [SerializeField] private GameObject zoomCam;  
    [SerializeField] private GameObject reticle;
    CharacterController characterController;
    Animator animator;
    PlayerHealthSystem playerHP;
    PlayerCombatSystem playerCombat;
    ActiveObjectCtrl objectCtrl;
    #endregion

    private void Awake() 
    {
        //Se buscan los componentes.
        characterController = GetComponent<CharacterController>();
        playerHP = GetComponent<PlayerHealthSystem>();
        playerCombat = GetComponent<PlayerCombatSystem>();
        objectCtrl = GetComponentInParent<ActiveObjectCtrl>();
        animator = GetComponent<Animator>();

        //Establece la altura que el controlador considera como "escalón".
        originalStepOffset = characterController.stepOffset; 

        currentState = PlayerStates.Normal; //Establece el estado inicial del jugador.
    }

    void Update() 
    {
        switch (currentState)
        {
            default:

            case PlayerStates.Normal:
                Movement(); //Método de movimiento básico.   
                Jump(); //Método de salto.
                Aim(); //Método de apuntado.
                if (Input.GetKeyDown(KeyCode.LeftShift)) //Al presionar la tecla shift izquierda...
                {
                    currentState = PlayerStates.Attack; //Se cambia al estado de ataque.
                    animator.SetInteger("attack", 0); //Establece el índice de ataques en 0.
                    animator.SetBool("atMode", true); //Activa el layer de animaciones de ataque.
                }
            break;

            case PlayerStates.Attack:
                Movement();
                Jump();
                //Métodos del script de las mecánicas de combate.
                playerCombat.StartCombo();
                playerCombat.StartHeavyCombo();
                playerCombat.SpecialAttack();
                playerCombat.QuickShot();
                //Al presionar la tecla shift izquierda...
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    currentState = PlayerStates.Normal; //Se regresa al estado normal.
                    animator.SetInteger("attack", 0); //Establece el índice de ataques en 0.
                    animator.SetBool("atMode", false); //Activa el layer de animaciones de ataque.
                }
                //Desactiva la retícula de apuntado y reestablece la cámara, esto en caso de activar el estado de ataque
                //mientras el jugador se encuentra apuntando.
                defaultCam.SetActive(true); 
                reticle.SetActive(false);
            break;

            case PlayerStates.HookshotFlying:
                HookShotMovement(); //Método de impulso al engancharse en una superficie.
            break;

            case PlayerStates.Knockback:
                StartCoroutine(Knockback());
                //Desactiva la retícula de apuntado y reestablece la cámara, esto en caso de ser noqueado
                //mientras el jugador se encuentra apuntando.
                defaultCam.SetActive(true);
                reticle.SetActive(false);    
            break;

            case PlayerStates.Still:
                //Este estado sirve para evitar que el jugador se mueva mientra realiza ciertos ataques.
                playerCombat.StartCombo();
                playerCombat.StartHeavyCombo();
                playerCombat.SpecialAttack();
                playerCombat.QuickShot();
            break;

            case PlayerStates.Dead:
                playerHP.enabled = false;
                animator.Play("Death");
            break;
        }
    }

    private void Movement()
    {
        /*Gran parte del código de movimiento base la extraje del canal de Youtube "Ketra games", el cuál me ayudó a ajustar el movimiento
        del jugador de acuerdo a su propia rotación y la de la cámara*/
        //Se leen los inputs de movimiento.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        //Convierte la magnitud del input en un vector.
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        float inputMagnitude = Mathf.Clamp01(moveDirection.magnitude);

        animator.SetFloat("InputMagnitude", inputMagnitude, 0.05f, Time.deltaTime);
        //Determina la velocidad de movimiento multiplicando la magnitud del input por una variable de velocidad máxima.
        float speed = inputMagnitude * maxSpeed;
        moveDirection.Normalize();    
        
        //Ajusta el movimiento del jugador a la rotación de la cámara. 
        moveDirection = Quaternion.AngleAxis(mainCam.transform.rotation.eulerAngles.y, Vector3.up) * moveDirection;
        
        //Se crea un vector que determinará la velocidad a la que se mueve el jugador.
        Vector3 velocity = moveDirection * speed;
        velocity.y = ySpeed;

        //Se utiliza un componente para que realice el movimiento del personaje
        characterController.Move(velocity * Time.deltaTime);

        //Si el jugador se encuentra en movimiento este rotará automáticamente en la dirección en la que se esté moviendo.
        if(moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        
        //Al presionar la tecla F se efectua el movimiento de evasión.
        if(Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(EvasionRoll(velocity));
        }
    }

    private void Jump()
    {
        /*Parte del código de salto también la extraje del canal "Ketra games", para darle más consistencia al salto y que este no requiera
        de que el botón sea presionado en un momento exacto*/
        //La gravedad aumentará con el tiempo en caso de estar en el aire
        ySpeed -= gravity * Time.deltaTime;
        //Determina cuando fue la última vez que el jugador estuvo en el suelo.
        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }
        //Determina cuanto tiempo ha pasado de que el jugador presionó el botón de salto.
        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        //Si la diferencia entre el tiempo transcurrido y la última vez que el jugador estuvo en el suelo
        //es menor al tiempo que tiene para saltar...
        if (Time.time - lastGroundedTime <= jumpGracePeriod)
        {
            characterController.stepOffset = originalStepOffset; //Se reestablece la altura que el controlador considera como "escalón".
            ySpeed = -0.5f; //Se establece la fuerza de gravedad.
            canDoubleJump = true; //Se habilita el doble salto.
            //Si la diferencia entre el tiempo transcurrido y la última vez que el jugador presionó el botón de salto
            //es menor al tiempo que tiene para saltar...
            if (Time.time - jumpButtonPressedTime <= jumpGracePeriod)
            {
                ySpeed = jumpSpeed; //Se le da el impúlso de salto al jugador.
                //Se nulifican estas variables.
                jumpButtonPressedTime = null; 
                lastGroundedTime = null;
                animator.SetTrigger("isJumping");
            }
        }
        else
        {
            characterController.stepOffset = 0; //Se reduce la altura que el controlador considera como "escalón".
            if (Input.GetButtonDown("Jump") && canDoubleJump) //Si el jugador puede efectuar un doble salto...
            {
                ySpeed = jumpSpeed * 0.75f; //Se le da el impúlso de salto reducido al jugador.
                canDoubleJump = false; //Se desactiva la posibilidad de hacer doble salto.
            }
        }
    }
    
    /*Los siguientes tres métodos los extraje del canal "Code Monkey" para resolver la mecánica del uso de un "gancho" para
    impulsarse hacia una superficie.*/
    private void Aim()
    {
        //Se establece la posición de un LineRenderer con textura de cadena.
        chain.SetPosition(0, hook.position);
        chain.SetPosition(1, hook.position);
        
        if(Input.GetMouseButton(1)) //Al dar click izquierdo...
        {
            animator.SetBool("aimMode", true);
            objectCtrl.ActivateObj(5); //Se habilita un game object.     
            defaultCam.SetActive(false); //Se desactiva la cámara principal para activar una cámara secundaria.
            reticle.SetActive(true); //Se activa la retíclua de apuntado.
            Shoot(); //Permite el método de disparo.
        }
        else
        {
            animator.SetBool("aimMode", false);
            objectCtrl.DeactivateObj(5); //Se desahiblita el game object activado al apuntar.
            defaultCam.SetActive(true); //Se reactiva la cámara principal.
            reticle.SetActive(false); //Se desactiva la retícula.
        }
        
    }

    private void Shoot()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit; //Variable que detecta el punto donde choca el raycast.

            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out raycastHit,100))
            {
                //Dependiendo del objeto con el que el raycast colisiona, cambiará su efecto.
                switch (raycastHit.collider.tag)
                {
                    //Si colisiona con un punto de anclaje iniciará el método que impulsa al personaje al punto de anclaje.
                    case "HookPoint":
                        animator.SetTrigger("hookshot");
                        hookshotPosition = raycastHit.point;

                        defaultCam.SetActive(true);
                        reticle.SetActive(false);

                        currentState = PlayerStates.HookshotFlying; 
                    break;
                    //Si colisiona con un item iniciará un método en el script del item para que este sea jalado hacia el jugador.
                    case "Item":
                        Item item = raycastHit.transform.GetComponentInParent<Item>();
                        item.SetItemDirection(this.transform);
                        chain.SetPosition(0, hook.position);
                        chain.SetPosition(1, raycastHit.point);
                    break;
                    //Este caso fue descartado debido a fallas con los enemigos, pero básciamente era un disparo que dañaba al enemigo.
                    /*case "Enemy":
                        EnemyHealthSystem enemy = raycastHit.transform.GetComponentInParent<EnemyHealthSystem>();
                        enemy.Damage(10);
                    break;*/
                }
               
            }
        }
            
    }

    private void HookShotMovement()
    {
        //Vector que mide la distancia entre el jugador y el punto de anclaje.
        Vector3 direction = (hookshotPosition - transform.position).normalized;
        //Rota al jugador en dirección al punto de anclaje.
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3 (direction.x, 0, direction.z)); 
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        //Se establece una velocidad mínima y máxima
        float hookshotMinSpeed = 25f;
        float hookshotMaxSpeed = 50f;
        //La velocidad a la que se impulsa el jugador es relativa a la distancia a recorrer,
        //sin embargo, repeta los límites de velocidad mínima y máxima, finalmete multiplica la velocidad obtenida por dos.
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotMinSpeed, hookshotMaxSpeed);
        float hookshotSpeedMultiplier = 2f;
        //El jugador se mueve en la dirección y velocidad definida anteriormente.
        characterController.Move(direction * hookshotSpeed *hookshotSpeedMultiplier * Time.deltaTime);
        //Se cambia la posición del line renderer para simular que el jugador se impulsa con una cadena.
        chain.SetPosition(0, hook.position);
        chain.SetPosition(1, hookshotPosition);
        //Al llegar a cierta distancia del punto de anclaje el jugador regresa a su estado normal.
        if(Vector3.Distance(hook.position, hookshotPosition) < 5f)
        {
            currentState = PlayerStates.Normal;
        }
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//

    //Los siguientes tres métodos se activan mediante animation events:
    private void GunRecoil()
    {
        //Este método empuja al jugador hacia atrás al usar un ataque con un arma de fuego, simulando el retroceso de la misma.
        Vector3 pushDirection = new Vector3(0, 0, -50f);
        pushDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * pushDirection;
        characterController.Move(pushDirection*Time.deltaTime);
    }

    private void AttackStateReset()
    {
        currentState = PlayerStates.Attack;
    }

    private void BlockMovement()
    {
        currentState = PlayerStates.Still;
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//

    IEnumerator EvasionRoll(Vector3 direction)
    {
        /*Parte de este código la extraje del canal "Vubi GameDev", de un video que explica como hacer un "dash".*/
        float startTime = Time.time; //Se establece el tiempo en el que se inició esta acción.

        while(Time.time < startTime + rollTime) //Mientras el tiempo actual no sobrepase la suma del tiempo de incio y la duración de la evasión...
        {
            animator.Play("Roll");
            playerHP.Inmunity(rollTime); //Se llama a un método de otro script que anula el daño del jugador.
            characterController.Move(direction*rollSpeed*Time.deltaTime); //Al movimiento del jugador se le añade la velocidad de evasión.
            //Se reinica cualquier combo que se estuviese haciendo antes de la evasión.
            playerCombat.attackNumber = 0;
            playerCombat.heavyAttackNumber = 0;
            animator.SetInteger("attack", 0);
            animator.SetInteger("heavyAttack", 0);
            
            yield return null;
        }
    }

    IEnumerator Knockback()
    {
        /*Parte de este código la extraje del canal "gamesplusjames", para dar el efecto de aturdimiento al ser atacado*/  
        animator.Play("Great Sword Impact");            
        //Se establece la dirección en la que el jugador será empujado.     
        Vector3 pushDirection = new Vector3(0, 0, -1f);
        pushDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * pushDirection;
        characterController.Move(pushDirection*knockbackForce*Time.deltaTime);
        
        //Después de que haya pasado el tiempo de aturdimiento:
        yield return new WaitForSeconds(knockbackTime);
        //Se reinicia cualquier combo que el jugador hicera antes de ser aturdido.
        playerCombat.attackNumber = 0;
        playerCombat.heavyAttackNumber = 0;
        animator.SetInteger("attack", 0);
        animator.SetInteger("heavyAttack", 0);
        //Se devuelve al jugador al estado en el que se encontraba.
        if(!animator.GetBool("atMode"))
            currentState = PlayerStates.Normal;
        else if (animator.GetBool("atMode"))
            currentState = PlayerStates.Attack;
    }

    //El siguiente método sirve para ocultar el cursor mientra se está en el juego.
    private void OnApplicationFocus(bool focusStatus) 
    {
        if (focusStatus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}