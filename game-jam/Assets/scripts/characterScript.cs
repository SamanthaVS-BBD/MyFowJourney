using System;
using UnityEngine;
using UnityEngine.UI;

public class characterScript : MonoBehaviour
{
	public Camera cameraComponent;
	public Rigidbody2D Rigidbody;

	[Header("Movement")]
	public float VerticalSpeed = 1500;
	public float MaxY = 3500;
	public float MaxVerticalSpeed;
	public float antiGravity;
	public float MinX = 800;
	public float HorizontalSpeed = 175;
	public float MaxX = 1000;

	[Header("Jump")]
	public float RotationalSpeed = 325;
	public bool IsGrounded = false;
	public int NumFlips = 0;
	public int MaxFlips = 10;
	public float FlipBoost = 500;
	public float rotationSpeed = 8f;
	public Transform characterTransform; // The transform of your character.
	public string groundTag = "ground"; // Tag that represents the ground or slope.

	[SerializeField] private float rayDistance = 4; // Distance of the raycast.
	
	[Header("References")]
	public sceneManager _sceneManager;
	public gameOverScript _gameOverScript;
	public Text ScoreDisplay;
	public GameObject gameOverUI;
	public GameObject Ground;

	private int score;
	public int getScore() => score;
	private float prevScoreDistance;
	private bool isGameOver;

	private float groundCheckDelay = 0.1f;
	private float lastGroundedTime = 0;

	// Start is called before the first frame update
	void Start()
	{
		isGameOver = false;
		VerticalSpeed = 1500;
		MaxY = 1000;
		antiGravity = 10f;

		MinX = 2000;
		HorizontalSpeed = 2000;//1750;
		MaxX = 3500;

		RotationalSpeed = 325;

		IsGrounded = false;

		score = 0;
		prevScoreDistance = transform.position.x;

		Rigidbody.velocity = Vector2.right * (float)(HorizontalSpeed / 100.0);
	}

	// Update is called once per frame
	void Update()
	{
		float currentDeltaTime = Time.deltaTime;
		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(IsGrounded)
			{
				double ratio = 1 / 100.0;
				currentDeltaTime = (float)(ratio);
				if(currentDeltaTime >= 1 / 50.0)
				{
					currentDeltaTime = (float)(ratio);
				}

				float possibleY = Rigidbody.velocity.y + 0.1f * Rigidbody.velocity.x + antiGravity * currentDeltaTime;
				float maxY = MaxY * currentDeltaTime;
				var newY = Math.Max(possibleY, maxY);

				Rigidbody.velocity += Vector2.up * newY;
				IsGrounded = false;
			}
		}

		if(!IsGrounded)
		{
			if(Input.GetKey(KeyCode.Space))
			{
				var prevRotation = Rigidbody.rotation;
				Rigidbody.rotation += RotationalSpeed * Time.deltaTime;
				Rigidbody.rotation = WrapAngle(Rigidbody.rotation);
				if(prevRotation < -45 && Rigidbody.rotation > -45)
				{
					if(NumFlips < MaxFlips) NumFlips++;
				}
			}
		}
		else
		{
			RotateCharacterToGround();
			//Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Mathf.Clamp(Rigidbody.velocity.y, -MaxVerticalSpeed, 0));
		}
		
		BoostX(0, currentDeltaTime);

		if(transform.position.x - prevScoreDistance > 20)
		{
			score++;
			prevScoreDistance = transform.position.x;
		}
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag("ground"))
		{
			Rigidbody.gravityScale = 10;
			IsGrounded = true;
			lastGroundedTime = Time.time;
			var boost = FlipBoost * NumFlips;
			var deltaT = Time.deltaTime;
			BoostX(boost, deltaT);
			score += NumFlips;
			NumFlips = 0;
		}

		if(collision.gameObject.CompareTag("obstacle")){
			GameOver();
		}
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		GameOver();
	}

	public void BoostX(float boost, float currentDeltaTime)
	{
		if(!isGameOver){
			var xVelocity = Math.Min(Rigidbody.velocity.x + boost * currentDeltaTime, MaxX * currentDeltaTime);
			xVelocity = Math.Max(xVelocity, HorizontalSpeed * currentDeltaTime);
			Rigidbody.velocity = Rigidbody.velocity.y * Vector2.up + xVelocity * Vector2.right;
		}
	}
		

	public void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("ground") && Time.time - lastGroundedTime > groundCheckDelay)
		{
			IsGrounded = false;
			Rigidbody.gravityScale = 1;
		}
	}

	private float WrapAngle(float angle)
	{
		var answer = ((angle + 180) % 360) - 180;
		return answer;
	}

	private float rotationVelocity = 0.0f;
	void RotateCharacterToGround()
	{
		// // Cast a ray downwards from the character's position to detect the ground.
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDistance);


		if(hit.collider != null && hit.collider.CompareTag(groundTag)) // Check if the ray hit something with the ground tag
		{
			Vector2 surfaceNormal = hit.normal;


			float angle = Mathf.Atan2(surfaceNormal.y, surfaceNormal.x) * Mathf.Rad2Deg;
			if(Math.Abs(angle) < 30)
			{
				GameOver();
			}
			if(angle > 10 || angle < -10)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle - 90f), rotationSpeed * Time.deltaTime);
			}
		}
		
	}


	private void GameOver()
	{
		GameObject UIM = GameObject.FindGameObjectWithTag("UIManager");
		UIM.GetComponent<UIManager>().setGameScore(false);
		UIM.GetComponent<UIManager>().gameOverUI(score.ToString());
		isGameOver = true;
		Rigidbody.velocity = new Vector2() { x=0, y=0 };
	}
}
