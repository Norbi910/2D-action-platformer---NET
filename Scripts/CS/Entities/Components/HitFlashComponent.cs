using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Components;

public partial class HitFlashComponent : Node2D
{

	[Export] public Sprite2D Sprite;
	[Export] public HealthComponent HealthComponent;
	[Export] public bool Enabled = false;
	[Export] public bool Strobe = false;
	[Export] public Color FlashColor = Color.FromString("WHITE",new  Color(1,1,1,1));
	
	private AnimationPlayer animationPlayer;
	private Timer flashTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("$AnimationPlayer");
		flashTimer = GetNode<Timer>("$FlashTimer");

		if (HealthComponent != null)
			HealthComponent.HealthChanged += Flash;
	}

	public override void _Process(double delta)
	{
		Sprite?.SetInstanceShaderParameter("enabled", Enabled);
		Sprite?.SetInstanceShaderParameter("color_parameter", FlashColor);
	}
	
	public void Flash(float _dmg)
	{
		Enabled = true;
		flashTimer.Start();
	}

	public void ToggleFlash()
	{
		Strobe = !Strobe;
		if (!flashTimer.Paused) return;
		if (!Strobe)
		{
			animationPlayer.Stop();
			FlashColor = Color.FromString("WHITE", FlashColor);
			Enabled = false;
			return;
		}

		Enabled = true;
		FlashColor = Color.FromString("RED", FlashColor);
		animationPlayer.Play("quick_flash");

	}

	private void OnFlashTimerTimeout()
	{
		Enabled = false;
		if (!Strobe && !animationPlayer.IsPlaying())
		{
			Enabled = true;
			FlashColor = Color.FromString("LIGHT_YELLOW", FlashColor);
			animationPlayer.Play("quick_flash");
		}
	}

}