
using Godot;

public partial class Player : CharacterBody2D
{
	private float WalkHorizontalSpeed { get; set; } = 125f;
	private float JumpHorizontalSpeed { get; set; } = 250f;
	private float Mass { get; set; } = 1f;
	private float TimeToJumpPeak { get; set; } = 0.4f;
	private float MaxJumpHeight { get; set; } = 132f;
	private float MinJumpHeight { get; set; } = 32f;
	private float MaxDurationOfJump { get; set; } = 0.65f;


	private float JumpSpeed;
	private float Gravity;
	private Vector2 velocity = new();
	private string facingDirection = "right";
	private float jumpKeyHoldTime = 0f;
	private bool isJumping = false;

	public override void _Ready()
	{
		Gravity = 2 * MaxJumpHeight / Mathf.Pow(TimeToJumpPeak, 2);
		JumpSpeed = Gravity * TimeToJumpPeak;
	}

	public override void _PhysicsProcess(double delta)
	{
		var movementDirection = GetInput();
		ApplyGravity((float)delta);

		if (isJumping)
		{
			if (IsOnCeiling())
			{
				velocity.Y = -velocity.Y;
			}

			if (IsOnWall())
			{
				velocity.X = -velocity.X;
			}
		}

		if (IsOnFloor())
		{
			isJumping = false; // Reset the jumping flag
			ApplyMovement(movementDirection);
			velocity.Y = 0;
			if (Input.IsActionPressed("jump"))
			{
				jumpKeyHoldTime += (float)delta; // Update the duration while the jump key is held
			}
		}
		else
		{
			jumpKeyHoldTime = 0f; // Reset the jump key hold time if the player is not on the floor
		}

		// Jump logic remains unchanged
		if ((Input.IsActionJustReleased("jump") || jumpKeyHoldTime > MaxDurationOfJump) && IsOnFloor())
		{
			Jump();
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private Vector2 GetInput()
	{
		Vector2 direction = new Vector2();
		if (Input.IsActionPressed("walk_left"))
		{
			direction.X -= 1;
			facingDirection = "left";
		}
		if (Input.IsActionPressed("walk_right"))
		{
			direction.X += 1;
			facingDirection = "right";
		}
		return direction;
	}

	private void ApplyGravity(float delta)
	{
		velocity.Y += Gravity * delta;
	}

	private void ApplyMovement(Vector2 direction)
	{
		if (direction.X != 0)
		{
			velocity.X = direction.X * WalkHorizontalSpeed;
		}
		else
		{
			velocity.X = 0;
		}
	}

	private float CalculateJumpHeight()
	{
		return jumpKeyHoldTime / MaxDurationOfJump * MaxJumpHeight;
	}

	private float CalculateJumpSpeed()
	{
		return jumpKeyHoldTime / MaxDurationOfJump * JumpHorizontalSpeed;
	}

	private void Jump()
	{
		float dynamicJumpHeight = CalculateJumpHeight();
		if (dynamicJumpHeight < MinJumpHeight)
		{
			dynamicJumpHeight = MinJumpHeight;
		}
		float dynamicJumpSpeed = Mathf.Sqrt(2 * dynamicJumpHeight * Gravity); // Calculate the necessary jump speed for the desired height

		// Determine the horizontal jump force based on the facing direction
		float horizontalJumpForce = facingDirection == "right" ? CalculateJumpSpeed() : -CalculateJumpSpeed();

		// Apply the dynamic jump speed and horizontal force
		velocity.Y = -dynamicJumpSpeed; // Apply the dynamic jump speed vertically
		velocity.X = horizontalJumpForce; // Apply horizontal force based on facing direction

		jumpKeyHoldTime = 0f; // Reset the jump key hold time
		isJumping = true; // Set the jumping flag to true
	}
}

