
using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public int MaxSpeed { get; set; } = 50;

	public float Gravity { get; set; } = 9.81f;

	[Export]
	public float Mass { get; set; } = 1.0f;

	[Export]
	public float Acceleration { get; set; } = 10.0f;

	[Export]
	public float Friction { get; set; } = 8.0f;

	private Vector2 velocity = new Vector2();
	private string facingDirection = "right";

	public override void _PhysicsProcess(double delta)
	{
		var movementDirection = GetInput();
		ApplyGravity((float)delta);
		ApplyMovement(movementDirection, (float)delta);
		ApplyFriction(movementDirection, (float)delta);

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

	private void ApplyMovement(Vector2 direction, float delta)
	{
		if (direction.X != 0)
		{
			velocity.X += direction.X * Acceleration * Mass * delta;
			velocity.X = Mathf.Clamp(velocity.X, -MaxSpeed, MaxSpeed);
		}
	}

	private void ApplyFriction(Vector2 direction, float delta)
	{
		if (direction.X == 0)
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Friction * delta);
		}
	}
}

