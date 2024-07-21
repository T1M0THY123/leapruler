using System;
using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	private float WalkHorizontalSpeed { get; set; } = 125f;
	[Export]
	private float JumpHorizontalSpeed { get; set; } = 250f;
	[Export]
	private float MinJumpHorizontalSpeed { get; set; } = 42.5f;
	[Export]
	private float MaxJumpHorizontalSpeed { get; set; } = 155f; // Adjust the value as needed
	[Export]
	private float MaxJumpHeight { get; set; } = 69f; //nice
	[Export]
	private float MinJumpHeight { get; set; } = 16f;
	[Export]
	private float MaxDurationOfJump { get; set; } = 0.65f;
	[Export]
	private float TimeToJumpPeak { get; set; } = 0.4f;
	[Export]
	private float DegreesOfRotation { get; set; } = 90f;

	private float Gravity;
	private float JumpSpeed;
	private Vector2 velocity = new Vector2();
	private string facingDirection = "right";
	private float jumpKeyHoldTime = 0f;
	private bool isJumping = false;
	private bool ableToJump = true;
	private Sprite2D sprite;

	public override void _Ready()
	{
		Gravity = 2 * MaxJumpHeight / Mathf.Pow(TimeToJumpPeak, 2);
		JumpSpeed = Gravity * TimeToJumpPeak;
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 movementDirection = GetInput();
		ApplyGravity((float)delta);

		// Check if there is horizontal or vertical movement and set ableToJump accordingly
		if (IsOnWallOnly() || IsOnCeiling() || Velocity.X != 0 || Velocity.Y != 0)
		{
			ableToJump = false;
		}
		else
		{
			ableToJump = true;
		}

		if (isJumping)
		{
			if (IsOnCeiling())
			{
				velocity.Y = -velocity.Y;
			}

			if (IsOnWall())
			{
				velocity.X = -velocity.X;
				facingDirection = facingDirection == "right" ? "left" : "right";
				sprite.Rotation = facingDirection == "right" ? DegreesOfRotation : -DegreesOfRotation;
			}
		}

		if (IsOnFloor())
		{
			sprite.Rotation = 0;
			isJumping = false;

			ApplyMovement(movementDirection);

			velocity.Y = 0;

			if (Input.IsActionPressed("jump") && movementDirection.X == 0 && movementDirection.Y == 0)
			{
				jumpKeyHoldTime += (float)delta;
			}
		}
		else
		{
			jumpKeyHoldTime = 0f;
		}

		if ((Input.IsActionJustReleased("jump") || jumpKeyHoldTime > MaxDurationOfJump) && IsOnFloor() && ableToJump)
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
		// Ensure the ratio is between 0 and 1
		float ratio = Mathf.Clamp(jumpKeyHoldTime / MaxDurationOfJump, 0f, 1f);
		float scalingFactor = Mathf.Pow(ratio, 2);
		return MinJumpHeight + (MaxJumpHeight - MinJumpHeight) * scalingFactor;
	}

	private float CalculateJumpSpeed()
	{
		// Ensure the ratio is between 0 and 1
		float ratio = Mathf.Clamp(jumpKeyHoldTime / MaxDurationOfJump, 0f, 1f);
		// Calculate speed based on ratio
		float speed = Mathf.Lerp(MinJumpHorizontalSpeed, JumpHorizontalSpeed, ratio);
		// Ensure speed does not exceed MaxJumpHorizontalSpeed
		return Mathf.Min(speed, MaxJumpHorizontalSpeed);
	}

	private void Jump()
	{
		// Calculate dynamic jump height and speed
		float dynamicJumpHeight = CalculateJumpHeight();
		float dynamicJumpSpeed = Mathf.Sqrt(2 * dynamicJumpHeight * Gravity);

		// Calculate horizontal jump force. Ensure it respects MinJumpHorizontalSpeed and JumpHorizontalSpeed.
		float horizontalJumpForce = CalculateJumpSpeed() * (facingDirection == "right" ? 1 : -1);

		// Apply vertical and horizontal forces
		velocity.Y = -dynamicJumpSpeed;
		velocity.X = horizontalJumpForce;

		// Apply rotation based on the facing direction
		sprite.Rotation = facingDirection == "right" ? DegreesOfRotation : -DegreesOfRotation;

		// Reset jump key hold time and set isJumping to true
		jumpKeyHoldTime = 0f;
		isJumping = true;
	}

	public bool IsPlayer()
	{
		return true;
	}
}
