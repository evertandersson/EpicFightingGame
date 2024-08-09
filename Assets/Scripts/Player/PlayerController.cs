using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    bool gameStarted = false;

    private PlayerSelection playerSelection;
    [HideInInspector] public GameObject Player1GO;
    [HideInInspector] public GameObject Player2GO;

    public RectTransform waypoint;

    GameManager gameManager;
    public Animator animator;

    private Quaternion targetRotation;

    public float snapToRot;

    PlayerStats playerstats;

    //Movement variables
    public float moveSpeed;
    public float moveAcc = 30f;
    private float moveDir;

    //Jump variables
    private float jumpHeight;
    private float jumpAction;
    private bool canJump;
    int jumpsRemaining = 2;

    //Attack variables
    private float attackAction;
    public bool attackAnimPlaying = false;
    private bool waitUntilNextAttack = false;
    private bool isAttacking = false;
    public bool alreadyAttacked = false;
    private float attackTime = 0.1f;
    [SerializeField] private float attackTimer;
    public float damage;
    [SerializeField] GameObject bloodSplash;

    //MeshRenderers for children
    SkinnedMeshRenderer[] skinnedMeshRenderers;
    MeshRenderer[] meshRenderers;
    Color[] origColor;
    Color[] skinnedOrigColor;

    //Health
    public float maxHealth = 100f;
    public float currentHealth;
    public HealthBar healthBar;

    //PowerUps
    private float abilityAction;
    public bool rocketPickedUp = false;
    [SerializeField] GameObject rocket;
    public GameObject target;
    Vector3 offset = new Vector3 (0, 3f, 0);

    Vector3 spawnPos;

    public string playerTag = "Player";

    private Rigidbody rb;

    public InputActionAsset inputActions;
    InputActionMap gameplayActionMap;
    InputAction moveInputAction;
    InputAction jumpInputAction;
    InputAction attackInputAction;
    InputAction abilityInputAction;

    public float MoveSpeed
    {
        get { return moveSpeed; }
    }

    private void Awake()
    {
        gameStarted = false;
        gameManager = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody>();  
        playerstats = GetComponent<PlayerStats>();

        spawnPos = transform.position;
    }

    void Start()
    {
        gameObject.AddComponent<Waypoint>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        StartCoroutine("DisableScoringFirstSecond");

        if (meshRenderers != null && meshRenderers.Length > 0)
        {
            origColor = new Color[meshRenderers.Length]; // Make sure to initialize the array
            for (int i = 0; i < meshRenderers.Length; i++)
                origColor[i] = meshRenderers[i].material.color;
        }

        if (skinnedMeshRenderers != null && skinnedMeshRenderers.Length > 0)
        {
            skinnedOrigColor = new Color[skinnedMeshRenderers.Length]; // Initialize the array
            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                skinnedOrigColor[i] = skinnedMeshRenderers[i].material.color;
        }

        playerSelection = PlayerSelection.Instance;

        CreateControls();
        GetComponent<PlayerBounce>().GetPlayers();
    }

    public void GetHealth()
    {
        healthBar = FindObjectOfType<HealthBar>();
        ResetHealth();
    }

    public void SetUpStats()
    {
        //Set up stats
        moveAcc = playerstats.speed;
        maxHealth = playerstats.health;
        damage = playerstats.damage;
        GetComponent<PlayerBounce>().SetDamage();
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<float>();
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        jumpAction = context.ReadValue<float>();
    }

    public void GetAttackInput(InputAction.CallbackContext context)
    {
        attackAction = context.ReadValue<float>();
    }

    public void GetAbilityInput(InputAction.CallbackContext context)
    {
        abilityAction = context.ReadValue<float>();
    }

    private void OnEnableControls()
    {
        moveInputAction.Enable();
        jumpInputAction.Enable();
        attackInputAction.Enable();
        abilityInputAction.Enable();
    }

    private void OnDisable()
    {
        moveInputAction.Disable();
        jumpInputAction.Disable();
        attackInputAction.Disable();
        abilityInputAction.Disable();   
    }

    private void CreateControls()
    {
        if (SceneManager.GetActiveScene().name != "CharacterSelection")
        {
            Player1GO = playerSelection.Player1GO;
            Player2GO = playerSelection.Player2GO;

            if (gameObject == playerSelection.Player1GO)
            {
                gameplayActionMap = inputActions.FindActionMap("Player1");
                target = Player2GO;
            }
            else if (gameObject == playerSelection.Player2GO)
            {
                gameplayActionMap = inputActions.FindActionMap("Player2");
                target = Player1GO;
            }

            moveInputAction = gameplayActionMap.FindAction("Move");
            jumpInputAction = gameplayActionMap.FindAction("Jump");
            attackInputAction = gameplayActionMap.FindAction("Attack");
            abilityInputAction = gameplayActionMap.FindAction("Ability");

            moveInputAction.performed += GetMoveInput;
            moveInputAction.canceled += GetMoveInput;

            jumpInputAction.performed += GetJumpInput;
            jumpInputAction.canceled += GetJumpInput;

            attackInputAction.performed += GetAttackInput;
            attackInputAction.canceled += GetAttackInput;

            abilityInputAction.performed += GetAbilityInput;
            abilityInputAction.canceled += GetAbilityInput;

            OnEnableControls();
        }
    }

    private void FixedUpdate()
    {
        if (moveDir != 0)
            snapToRot = moveDir;

        Move();
        Jump();
        CheckMovingPlatform();
        CheckForAbility();
        CheckAnimation();

        moveSpeed = rb.velocity.x;
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y + jumpHeight);

        animator.SetFloat("MoveSpeed", Mathf.Abs(moveSpeed));
        animator.SetFloat("RunningPressed", Mathf.Abs(moveDir));
        animator.SetBool("AttackWait", waitUntilNextAttack);
        animator.SetBool("isAttacking", isAttacking);

        if (!gameManager.targets.Contains(gameObject.transform) && currentHealth > 0)
        {
            OutOfBounds();
        }
    }

    IEnumerator DisableScoringFirstSecond()
    {
        yield return new WaitForSeconds(1);
        gameStarted = true;
    }

    private void Move()
    {
        float horizontalForce = moveDir * moveAcc;
        float directionForce = snapToRot * moveAcc;

        // Apply force to move the player horizontally
        rb.AddForce(new Vector3(horizontalForce, 0, 0));

        SetRotation(directionForce);
    }

    private void SetRotation(float direction)
    {
        // Determine the rotation angle based on the direction
        float targetRotationAngle = (direction > 0) ? 90 : 260;

        // Create a quaternion to represent the rotation
        targetRotation = Quaternion.Euler(0, targetRotationAngle, 0);

        // Apply the quaternion rotation to the player's transform
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 6f * Time.deltaTime);
    }

    private void CheckMovingPlatform()
    {
        RaycastHit hit;
        float raycastDistance = 0.2f;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                transform.SetParent(hit.transform);
            }
        }
        else
        {
            transform.SetParent(null);
        }
    }

    private void Jump()
    {
        RaycastHit hit;
        float raycastDistance = 0.2f;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                canJump = true;
                jumpsRemaining = 2;
            }  
            else
                canJump = false;
        }
        else
            canJump = false;

        if (canJump && jumpAction == 1)
        {
            jumpHeight = 20f;
            canJump = false;
            jumpsRemaining--;
            jumpAction = 0;
        }
        else if (!canJump && jumpAction == 1 && jumpsRemaining > 0)
        {
            rb.velocity = new Vector2(moveSpeed, 0);
            jumpHeight = 15f;
            jumpsRemaining--;
        }
        else
            jumpHeight = 0f;
    }

    void CheckForAbility()
    {
        if (abilityAction == 1 && rocketPickedUp)
        {
            Debug.Log("Rocket used!");
            Instantiate(rocket, transform.position + offset, target.transform.rotation);
            FindObjectOfType<Rocket>().OnLaunch(target);
            healthBar.HideRocketIcon(gameObject);
            rocketPickedUp = false;
        }
    }

    private void CheckAnimation()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("1H_Melee_Attack_Slice_Horizontal"))
        {
            if (attackTimer > 0)
            {
                attackAnimPlaying = true;
                attackTimer -= Time.deltaTime;
            }
            if (attackTimer <= 0)
                attackAnimPlaying = false;
        }
        else
        {
            attackTimer = attackTime;
            attackAnimPlaying = false;
            alreadyAttacked = false;
        }
            

        if (attackAction == 1 && !isAttacking && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("1H_Melee_Attack_Slice_Horizontal"))
        {
            // Player pressed the attack button, is not currently attacking, and not in the middle of an attack animation
            isAttacking = true;
            waitUntilNextAttack = true;
        }

        if (isAttacking && this.animator.GetCurrentAnimatorStateInfo(0).IsName("1H_Melee_Attack_Slice_Horizontal"))
        {
            // Attack animation is playing while the player is attacking
            waitUntilNextAttack = false;
        }
        else
        {
            // Check for button release to allow a new attack
            if (attackAction == 0)
            {
                isAttacking = false;
            }
        }
    }

    public void Bounce(float bouncePower)
    {
        float forceMagnitude = bouncePower * 16f;
        rb.AddForce(forceMagnitude,0,0);
    }

    private void OutOfBounds()
    {
        rb.velocity = Vector3.zero;
        transform.position = spawnPos;
        ResetHealth();
        if (gameStarted)
            healthBar.UpdateScore(gameObject);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Instantiate(bloodSplash, new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z - 5f), Quaternion.identity, gameObject.transform);
        StartCoroutine("DamageFlash");
        UpdateHealth();
        if (currentHealth <= 0)
        {
            Invoke("OutOfBounds", 1f);
        }    
    }

    IEnumerator DamageFlash()
    {
        foreach (var mesh in meshRenderers)
            mesh.material.color = Color.red;
        foreach (var mesh in skinnedMeshRenderers)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].material.color = origColor[i];
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            skinnedMeshRenderers[i].material.color = skinnedOrigColor[i];
    }

    public void UpdateHealth()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth, gameObject);
    }

    
    private void ResetHealth()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth, gameObject);
    }
    

    public void CheckSpeed(float amount)
    {
        StartCoroutine(resetSpeed(amount));
    }

    private IEnumerator resetSpeed(float amount)
    {
        yield return new WaitForSeconds(5);
        moveAcc -= amount;
    }


}