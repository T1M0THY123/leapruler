using System;
using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	private float WalkHorizontalSpeed { get; set; } = 45f;
	[Export]
	private float JumpHorizontalSpeed { get; set; } = 250f;
	[Export]
	private float MinJumpHorizontalSpeed { get; set; } = 66f;
	[Export]
	private float MaxJumpHorizontalSpeed { get; set; } = 188f; // Adjust the value as needed
	[Export]
	private float MaxJumpHeight { get; set; } = 77f; //nice
	[Export]
	private float MinJumpHeight { get; set; } = 5f;
	[Export]
	private float MaxDurationOfJump { get; set; } = 0.38f;
	[Export]
	private float TimeToJumpPeak { get; set; } = 0.35f;
	[Export]
	private float gravityFactor { get; set; } = 1f;
	[Export]
	private float MinimumBounceVelocity { get; set; } = 150f;
	private float Gravity;
	private float JumpSpeed;
	private Vector2 velocity = new Vector2();
	private Sprite2D sprite;
	private AudioStreamPlayer audioStreamPlayer;
	private string facingDirection = "right";
	private float jumpKeyHoldTime = 0f;
	private bool isJumping = false;
	private string lastInputAction = "";
	public bool IsPlayer() => true;

	public override void _Ready()
	{
		Gravity = 2 * MaxJumpHeight / Mathf.Pow(TimeToJumpPeak, 2);
		JumpSpeed = Gravity * TimeToJumpPeak;
		sprite = GetNode<Sprite2D>("Sprite2D");
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		isJumping = false;
		Vector2 movementDirection = GetInput();
		sprite.Rotation = 0;


		ApplyGravity((float)delta);


		if (IsOnFloor()) // If the player is on the floor
		{
			isJumping = false;
			velocity.X = 0;
			velocity.Y = 0;
			bool jumpInitiated = false;

			if (Input.IsActionPressed("jump") && movementDirection.X == 0 && movementDirection.Y == 0)
			{
				if (Input.IsActionJustPressed("jump"))
				{
					lastInputAction = "jump";
				}
				jumpKeyHoldTime += (float)delta;
				jumpInitiated = true;
			}
			if ((Input.IsActionJustReleased("jump") || jumpKeyHoldTime > MaxDurationOfJump) && lastInputAction == "jump")
			{
				Jump();
				jumpInitiated = true;
			}

			if (!jumpInitiated)
			{
				velocity.X = movementDirection.X * WalkHorizontalSpeed;
				jumpKeyHoldTime = 0f;
			}
		}
		else
		{
			if (IsOnCeiling())
			{
				// Check if the current Y velocity is too small to provide a noticeable bounce
				GD.Print(velocity.Y + " " + MinimumBounceVelocity);
				if (Math.Abs(velocity.Y) < MinimumBounceVelocity)
				{
					// Apply a minimum bounce velocity downwards
					velocity.Y = MinimumBounceVelocity;
				}
				else
				{
					// Invert the Y velocity as usual
					velocity.Y = -velocity.Y;
				}
				PlayHitSound();
			}
			if (IsOnWall())
			{
				velocity.X = -velocity.X;
				facingDirection = facingDirection == "right" ? "left" : "right";
				PlayHitSound();
			}

			if (facingDirection == "left")
			{
				sprite.RotationDegrees = -45;
			}
			else
			{
				sprite.RotationDegrees = 45;
			}
		}


		Velocity = velocity;
		MoveAndSlide();
	}
	private Vector2 GetInput()
	{
		Vector2 direction = new();
		if (Input.IsActionPressed("walk_left"))
		{
			direction.X -= 1;
			facingDirection = "left";
			if (Input.IsActionJustPressed("walk_left"))
			{
				lastInputAction = "move";
			}
		}
		if (Input.IsActionPressed("walk_right"))
		{
			direction.X += 1;
			facingDirection = "right";
			if (Input.IsActionJustPressed("walk_right"))
			{
				lastInputAction = "move";
			}
		}
		return direction;
	}

	private void ApplyGravity(float delta)
	{
		velocity.Y += Gravity * delta * gravityFactor;
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

		// Reset jump key hold time and set isJumping to true
		jumpKeyHoldTime = 0f;
		isJumping = true;
		PlayJumpSound();
	}

	public String GetLastMove()
	{
		return lastInputAction;
	}

	public String GetFacingDirection()
	{
		return facingDirection;
	}

	private void PlayHitSound()
	{
		audioStreamPlayer.PitchScale = 0.25f;
		audioStreamPlayer.Play();
	}

	private void PlayJumpSound()
	{
		audioStreamPlayer.PitchScale = 1f;
		audioStreamPlayer.Play();
	}
}
