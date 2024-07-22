using Godot;
using System;

public partial class Label : Godot.Label
{
	private Player player; // Reference to the parent Player node

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent<Player>(); // Initialize the player reference
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (player != null)
		{
			// Update the label's text based on the player's isJumping status
			Text = player.GetLastMove();
		}
	}
}
