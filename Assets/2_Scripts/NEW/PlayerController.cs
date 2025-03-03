using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public GridManager gridManager; // Reference to the GridManager
    public float moveDelay = 0.2f;  // Delay between moves (for smooth movement)

    public GameObject[] bulletPrefabs;
    public GameObject bulletSpeedUpgrade, coinPickup;

    private int currentRow = 0, currentColumn = 0;  // Current row ,column of the player on the grid
    private float moveCooldown = 0f, shootCooldown = 0f, shootDelay = 1f;

    private PlayerInput playerInputActions;  // Input Actions asset

    private Vector2 moveInput;  // Store movement input

    [HideInInspector] public bool isDead = false, isMoved = false, isShootPressed = false, isMenuButtonPressed = false;

    private GameObject currentBulletPrefab = null;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float bulletSpeedUpFactor = 1f;
    public float BulletSpeedUpFactor { get => bulletSpeedUpFactor; set => bulletSpeedUpFactor = value; }

    private float gameplayTime = 0f;
    public float GameplayTime { get => gameplayTime; set => gameplayTime = value; }

    private bool hasMenuOpened = false;
    private bool hasShot = false, canShoot = false;

    private AudioSource audioSource;

    private void OnEnable()
    {
        playerInputActions.Player.Enable();  // Enable input actions when the object is active
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();  // Disable input actions when the object is inactive
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        isDead = false;

        GameplayTime = 0f;

        // Initialize input actions
        playerInputActions = new PlayerInput();

        // Hook up movement actions
        playerInputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();

        // Sweep button
        playerInputActions.Player.Sweep.performed += ctx =>
        {
            if (!hasShot)
            {
                isShootPressed = true;
                hasShot = true; // Set the flag to true to prevent further calls
            }
        };

        // Reset the sweep flag when the action is canceled
        playerInputActions.Player.Sweep.canceled += ctx =>
        {
            isShootPressed = false; // Reset sweep button state
            hasShot = false; // Allow sweep to be registered again when canceled
        };

        // Menu Button
        playerInputActions.Player.Menu.performed += ctx =>
        {
            if (!hasMenuOpened)
            {
                LevelManager.Instance.BackButtonPressed();
                isMenuButtonPressed = true;
                hasMenuOpened = true; // Set the flag to true to prevent further calls
            }
        };

        // Reset the menu flag when the action is canceled
        playerInputActions.Player.Menu.canceled += ctx =>
        {
            isMenuButtonPressed = false; // Reset menu button state
            hasMenuOpened = false; // Allow menu to be registered again when canceled
        };
    }

    private void Start()
    {
        moveCooldown = moveDelay;
        shootCooldown = shootDelay;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsGamePlaying()
    {
        return !isDead;
    }

    private void Update()
    {
        if (!IsGamePlaying())
        {
            return;
        }
        else
        {
            GameplayTime += Time.deltaTime;
        }

        HandleMovement();
        HandleShooting();
        HandleWhalePassTimeAchievement();
    }

    private void HandleMovement()
    {
        moveCooldown -= Time.deltaTime;  // Reduce the cooldown timer

        if (moveCooldown > 0f)
        {
            return;  // If we're still on cooldown, exit
        }

        int horizontal = Mathf.RoundToInt(moveInput.x);  // Get horizontal movement input
        int vertical = Mathf.RoundToInt(moveInput.y);  // Get vertical movement input

        // Check if movement input was received
        if (horizontal != 0 || vertical != 0)
        {
            isMoved = true;
            MovePlayer(horizontal, vertical);  // Move the player in the grid
            moveCooldown = moveDelay;  // Reset the move cooldown
        }
    }

    public void SetPosition(int row, int column)
    {
        currentRow = row;
        currentColumn = column;
        Vector2 position = new(column * gridManager.TileSize, (row * gridManager.TileSize) - 0.2f);
        transform.position = position;
    }

    private void HandleShooting()
    {
        shootCooldown -= Time.deltaTime;  // Reduce the cooldown timer
        if (shootCooldown > 0f)
        {
            return;  // If we're still on cooldown, exit
        }

        canShoot = true;

        if (isShootPressed && canShoot)
        {
            ShootBullet();
        }
    }

    public void ShootBullet()
    {
        if (currentBulletPrefab == null)
        {
            BulletSingle(); // Default bullet
        }

        var bullet = Instantiate(currentBulletPrefab);
        bullet.transform.position = transform.position;
        bullet.SetActive(true);
        animator.SetTrigger("IsShooting");
        shootCooldown = shootDelay / BulletSpeedUpFactor;
        canShoot = false;

        // Play Shoot Clip
        audioSource.PlayOneShot(AudioClips.Instance.shootClip);
    }

    private void MovePlayer(int horizontal, int vertical)
    {
        // Calculate the new row and column
        int newRow = currentRow + vertical;
        int newColumn = currentColumn + horizontal;

        // Check if the new position is within the grid boundaries
        if (newRow >= 0 && newRow < gridManager.Rows && newColumn >= 0 && newColumn < gridManager.Columns)
        {
            // Check if the new position is valid based on tile state
            GridTile tile = gridManager.GetTileAt(newRow, newColumn);
            if (tile != null && tile.currentState == GridTile.TileState.Swept)
            {
                // Update player’s row and column
                currentRow = newRow;
                currentColumn = newColumn;

                // Update player position to match the tile’s position in the grid
                Vector2 newPosition = new(currentColumn * gridManager.TileSize, (currentRow * gridManager.TileSize) - 0.2f);
                transform.position = newPosition;

                // Flip the player sprite based on movement direction
                if (moveInput.x < 0)
                {
                    spriteRenderer.flipX = false;
                }
                else //(moveInput.x < 0)
                {
                    spriteRenderer.flipX = true;
                }

                //moving condition
                if (moveInput.x == 0 && moveInput.y == 0)
                {
                    animator.SetBool("IsMoving", false);
                }
                else
                {
                    animator.SetBool("IsMoving", true);
                }
            }
        }

        moveInput = Vector2.zero;
    }

    private void HandleWhalePassTimeAchievement()
    {
        if ((int)GameplayTime == 30)
        {
            WhalepassManager.Instance.Survive30Seconds();
        }
        else if ((int)GameplayTime == 60)
        {
            WhalepassManager.Instance.Survive60Seconds();
        }
        else if ((int)GameplayTime == 90)
        {
            WhalepassManager.Instance.Survive90Seconds();
        }
        else if ((int)GameplayTime == 120)
        {
            WhalepassManager.Instance.Survive120Seconds();
        }
        else if ((int)GameplayTime == 300)
        {
            WhalepassManager.Instance.Survive300Seconds();
        }
    }

    public void Damage()
    {
        if (HealthManager.Instance.GetHealth() > 0)
        {
            HealthManager.Instance.RemoveHealth();
            animator.SetTrigger("IsHit");
            audioSource.PlayOneShot(AudioClips.Instance.playerHitClip);
        }

        if (HealthManager.Instance.GetHealth() <= 0)
        {
            IsDead();
        }
    }

    public void PlayCoinPickup()
    {
        audioSource.PlayOneShot(AudioClips.Instance.coinPickupClip);
    }

    public void IsDead()
    {
        LevelManager.Instance.m_gameLost.SetActive(true);
        GetComponent<Animator>().SetBool("IsDead", true);
        isDead = true;
    }

    #region Shoot Mechanics

    public void AddBuff(int buffIndex)
    {
        switch (buffIndex)
        {
            case 0: BulletSingle(); break;
            case 1: BulletDoubleSequence(); break;
            case 2: BulletDoubleParallel(); break;
            case 3: BulletTripleSequence(); break;
            case 4: BulletTripleParallel(); break;
            case 5: BulletQuad(); break;
            case 6: BulletQuadParallel(); break;
            default: break;
        }
    }

    public void BulletSingle()
    {
        shootDelay = 0.2f;
        currentBulletPrefab = bulletPrefabs[0];

        WhalepassManager.Instance.UnlockWeaponSingle();
    }

    public void BulletDoubleSequence()
    {
        shootDelay = 0.35f;
        currentBulletPrefab = bulletPrefabs[1];

        WhalepassManager.Instance.UnlockWeaponDoubleSequence();
    }

    public void BulletDoubleParallel()
    {
        shootDelay = 0.6f;
        currentBulletPrefab = bulletPrefabs[2];

        WhalepassManager.Instance.UnlockWeaponDoubleParallel();
    }

    public void BulletTripleSequence()
    {
        shootDelay = 0.65f;
        currentBulletPrefab = bulletPrefabs[3];

        WhalepassManager.Instance.UnlockWeaponTripleSequence();
    }

    public void BulletTripleParallel()
    {
        shootDelay = 0.8f;
        currentBulletPrefab = bulletPrefabs[4];

        WhalepassManager.Instance.UnlockWeaponTripleParallel();
    }

    public void BulletQuad()
    {
        shootDelay = 0.75f;
        currentBulletPrefab = bulletPrefabs[5];

        WhalepassManager.Instance.UnlockWeaponQuad();
    }

    public void BulletQuadParallel()
    {
        shootDelay = 1f;
        currentBulletPrefab = bulletPrefabs[6];

        WhalepassManager.Instance.UnlockWeaponQuadParallel();
    }

    #endregion
}
