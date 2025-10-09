using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private PlatformerValuesSO valuesSO;

	//Components and State Values
	private Rigidbody2D rb;
	private Animator animator;
	[SerializeField] private Vector2 currentVelocity;
	private float velocitySmoothing;

	//Timers
	private float coyoteTimeTimer;
	private float jumpBufferTimer;

	//Internal Calculations
	private float calculatedGravity;
	private float calculatedInitialJumpVelocity;

	//Input Values
	private InputAction moveInput;
	private float moveDirection;
	private InputAction jumpInput;
	[SerializeField] private bool isGrounded;
	private bool isHoldingJump;

	//InputSystem
	private PlayerInput playerInput;
	private string assignedScheme;


	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		InitializeInputActions(playerInput.currentActionMap);

		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();

		rb.gravityScale = 0f;
		rb.freezeRotation = true;

		calculatedGravity = 2 * valuesSO.peakHeight / -(valuesSO.timeToPeak * valuesSO.timeToPeak);

		calculatedInitialJumpVelocity = -calculatedGravity * valuesSO.timeToPeak;
	}
	private void InitializeInputActions(InputActionMap inputActions)
	{
		moveInput = playerInput.actions.FindAction("Move");
		moveInput.Enable();

		jumpInput = playerInput.actions.FindAction("Jump");
		jumpInput.started += OnJumpStarted;
		jumpInput.canceled += OnJumpCanceled;
		jumpInput.Enable();
    }

    void OnDestroy()
    {
		if (jumpInput != null)
		{
			jumpInput.started -= OnJumpStarted;
			jumpInput.canceled -= OnJumpCanceled;
		}
    }

	private void OnJumpStarted(InputAction.CallbackContext context)
	{
		jumpBufferTimer = valuesSO.jumpBufferTime;
		isHoldingJump = true;
	}

	private void OnJumpCanceled(InputAction.CallbackContext context)
	{
		isHoldingJump = false;
	}


    void Update()
    {
		UpdateHorizontalDirection();

		UpdateJumpValues();

		HandleJump();
    }
	
	private void UpdateHorizontalDirection() => moveDirection = moveInput.ReadValue<float>();

	private void UpdateJumpValues()
	{
        if (jumpBufferTimer > 0)
		{
			jumpBufferTimer -= Time.deltaTime;
		}

        if (isGrounded)
        {
            coyoteTimeTimer = valuesSO.coyoteTime;
        }
        else
        {
            coyoteTimeTimer -= Time.deltaTime;
        }
	}

    private void HandleJump()
    {
        bool canJump = jumpBufferTimer > 0f && coyoteTimeTimer > 0f;

        if (canJump)
        {
            currentVelocity.y = calculatedInitialJumpVelocity;

			isGrounded = false;
            
            jumpBufferTimer = 0f;
            coyoteTimeTimer = 0f; 
        }
    }

	void FixedUpdate()
	{
		HandleHorizontalMovement();
		HandleGravity();
		
		rb.linearVelocity = currentVelocity;
    }

    private void HandleHorizontalMovement()
	{
		float targetSpeed = moveDirection * valuesSO.maxSpeed;

		float smoothTime = Mathf.Abs(targetSpeed) > 0.01f ? valuesSO.accelerationTime : valuesSO.decelerationTime;

		currentVelocity.x = Mathf.SmoothDamp(
			currentVelocity.x,
			targetSpeed,
			ref velocitySmoothing,
			smoothTime
		);
	}

	private void HandleGravity()
	{
		if (isGrounded)
		{
			if (currentVelocity.y <= 0)
				currentVelocity.y = -0.1f;

			return;
		}

		currentVelocity.y += calculatedGravity * Time.deltaTime;

		if (currentVelocity.y < 0)
		{
			currentVelocity.y += calculatedGravity * (valuesSO.fallMultiplier - 1f) * Time.deltaTime;
		}
		else if (currentVelocity.y > 0 && !isHoldingJump)
		{
			currentVelocity.y += calculatedGravity * (valuesSO.lowJumpMultiplier - 1f) * Time.deltaTime;
		}
    }
	
    private void OnCollisionStay2D(Collision2D collision)
	{
		isGrounded = true;
	}

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

	public void OnAssignedScheme(string schemeName)
	{
		assignedScheme = schemeName;

		playerInput.SwitchCurrentControlScheme(assignedScheme, playerInput.devices.ToArray());

		playerInput.SwitchCurrentActionMap("Player");
	}

	public void OnPlayerLeft(PlayerInput playerInput)
	{
		if (assignedScheme != "Gamepad")
		{
			InputManager.instance.FreeKeyboardScheme(assignedScheme);
		}
	}


	// 	Components
	//  public float moveSpeed = 2f;
	//  public float jumpForce = 10f;
	//  private bool isGrounded;

	// 	private Rigidbody2D rb;
	// 	[SerializeField] private Animator animator;
	// 	[SerializeField] private Transform spriteTransform;
	// 	public LayerMask groundLayer;
	//  public float raycastDistance = 0.1f;

	// 	float horizontalInput = 0f;
	// 	float controllerInputDeathZone = 0.1f;

	// 	// Block placing
	// 	BlockScript currentBlockHolding = null;
	// 	float cooldownTime = 3;
	// 	float cooldownCurrentTime = 0;
	// 	Vector2 placeBlockPosition = new Vector2 (0.5f, -0.47f);

	// 	public AnimatorController blueAnimator;
	// 	public AnimatorController orangeAnimator;

	// 	PoolingManager blocksPool => GameManager.instance.blocksPool;

	// 	int nextBlockNumber = 0;
	// 	bool canPlaceFix = true;
	// 	float xLocalScale = 1f;




	// 	void Start()
	//     {
	// 		animator.runtimeAnimatorController = orangeAnimator;
	// 		nextBlockNumber = Random.Range(0, blocksPool.pooledObjects.Count - 1);
	//     }




	//     void Update()
	//     {
	// 		Vector2 pos = transform.position;
	// 		RaycastHit2D hit = Physics2D.Raycast(new Vector2(pos.x - 0.25f, pos.y - 0.51f) , Vector2.down, raycastDistance, groundLayer);
	// 		RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(pos.x + 0.25f, pos.y - 0.51f) , Vector2.down, raycastDistance, groundLayer);

	// 		isGrounded = hit.collider || hit2.collider;

	// 		animator.SetBool ("isGrounded", isGrounded);
	//         Debug.DrawRay(new Vector2(pos.x - 0.25f, pos.y - 0.51f), Vector2.down * raycastDistance, Color.cyan);
	//         Debug.DrawRay(new Vector2(pos.x + 0.25f, pos.y - 0.51f), Vector2.down * raycastDistance, Color.cyan);

	// 		// Read Movement input
	// 	 	horizontalInput = Input.GetAxis(playerTag + "Horizontal");

	// 		string jumpButton = (playerTag == "Player1") ? "Player1Jump" : "Player2Jump";
	//         if (Input.GetButtonDown(jumpButton) && isGrounded)
	//         {
	// 			Debug.Log ("TARAO");
	// 			// Jump
	// 			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
	// 			SoundManager.instance.PlaySound("jump");
	// 			// SONIDO DE JUMP
	// 		}

	// 		// Flip sprite
	// 		if (horizontalInput < -controllerInputDeathZone && rb.linearVelocity.x < 0)
	// 		{
	// 			xLocalScale = -1;
	// 			// spriteTransform.transform.localScale = new Vector3 (xLocalScale, 1, 1);
	// 			animator.SetBool ("run", true);
	// 		}
	// 		else if (horizontalInput > controllerInputDeathZone && rb.linearVelocity.x > 0)
	// 		{
	// 			xLocalScale = 1;
	// 			animator.SetBool ("run", true);
	// 		}
	// 		else
	// 		{
	// 			animator.SetBool ("run", false);
	// 		}

	// 		spriteTransform.transform.localScale = new Vector3 (xLocalScale, 1, 1);

	// 		// Place blocks input
	// 		KeyCode placeButton = (playerTag == "Player1") ? KeyCode.F : KeyCode.RightShift;

	// 		if (currentBlockHolding == null)
	// 		{
	// 			if (Input.GetKeyDown (placeButton) && GameManager.instance.gameState == GameManager.GameState.Started)
	// 			{
	// 				currentBlockHolding = blocksPool.GetPooledObject (nextBlockNumber, transform.position + (Vector3)placeBlockPosition,
	// 										Vector3.zero, 0).GetComponent<BlockScript>();
	// 				currentBlockHolding.StartHold(playerTag == "Player1" ? 1 : 2);
	// 				StartCoroutine (HoldBlockTimeFix(0.3f));
	// 				canPlaceFix = false;
	// 			}
	// 		}
	// 		else
	// 		{
	// 			// Flip block xScale
	// 			currentBlockHolding.transform.localScale = new Vector3 (xLocalScale, 1, 1);
	// 			placeBlockPosition.x = xLocalScale * 0.5f;

	// 			// Make block follow player
	// 			currentBlockHolding.transform.position = transform.position + (Vector3)placeBlockPosition;

	// 			bool overlapping = false;
	// 			foreach (BoxCollider2D col in currentBlockHolding.boxCollider2Ds)
	// 			{
	// 				if (VerificarSuperposicion (col))
	// 					overlapping = true;
	// 			}
	// 			currentBlockHolding.overlapping = overlapping;

	// 			// Place
	// 			if (Input.GetKeyDown (placeButton) && isGrounded && canPlaceFix && !overlapping)
	// 			{
	// 				currentBlockHolding.StopHold ();
	// 				currentBlockHolding.AnimateAppear ();
	// 				nextBlockNumber = Random.Range(0, blocksPool.pooledObjects.Count - 1);
	// 				SoundManager.instance.PlaySound("placeBlock");
	// 				currentBlockHolding = null;
	// 			}

	// 			KeyCode rotateButton = (playerTag == "Player1") ? KeyCode.R : KeyCode.Return;

	// 			// Rotate
	// 			if (Input.GetKeyDown (rotateButton))
	// 			{
	// 				currentBlockHolding.Rotate ();
	// 				SoundManager.instance.PlaySound("rotate");
	// 				// SONIDO DE ROTAR
	// 			}
	// 		}
	//     }




	// 	void FixedUpdate()
	// 	{
	// 		rb.linearVelocity = new Vector2 (horizontalInput * moveSpeed, rb.linearVelocity.y);
	// 	}




	// 	bool VerificarSuperposicion(BoxCollider2D collider)
	//     {
	//         // Obtén el centro y tamaño del collider
	//         Vector2 centroCollider = collider.bounds.center;
	//         Vector2 tamañoCollider = new Vector2((collider.size.x - 0.1f) * transform.lossyScale.x, (collider.size.y - 0.1f) * transform.lossyScale.y);

	//         // Realiza la verificación de superposición en un área rectangular alrededor del collider
	//         Collider2D[] collidersSuperpuestos = Physics2D.OverlapBoxAll(centroCollider, tamañoCollider, 0f);

	//         // Excluye el propio collider de la verificación
	//         foreach (var colliderSuperpuesto in collidersSuperpuestos)
	//         {
	//             if (colliderSuperpuesto != collider && !colliderSuperpuesto.isTrigger)
	//             {
	// 				Debug.Log (colliderSuperpuesto.transform.name);
	//                 return true; // Hay superposición con al menos un collider diferente
	//             }
	//         }

	//         return false; // No hay superposición con otros colliders
	//     }





	// 	IEnumerator HoldBlockTimeFix (float time)
	// 	{
	// 		yield return new WaitForSeconds (time);
	// 		canPlaceFix = true;
	// 	}
}
