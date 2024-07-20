using Godot;
using System;

public partial class NextLevel : Area2D
{
	[Export] public string NextScenePath { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Node2D body)
	{
		if (!body.HasMethod("IsPlayer")) return;
		GetTree().ChangeSceneToFile(NextScenePath);
	}
}



