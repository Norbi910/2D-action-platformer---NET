using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Components;

[GlobalClass]
public partial class HitFlashComponent : Node2D
{

	[Export] public Sprite2D Sprite;
	[Export] public HitBoxComponent HitBoxComponent;
	[Export] public bool Strobe = false;
	
	private Timer durationTimer;
	private Timer flashTimer;
	private Timer strobeTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		durationTimer = GetNode<Timer>("%DurationTimer");
		durationTimer.Timeout += OnDurationTimerTimeout;
		flashTimer = GetNode<Timer>("%FlashTimer");
		flashTimer.Timeout += OnFlashTimerTimeout;
		strobeTimer = GetNode<Timer>("%StrobeTimer");
		strobeTimer.Timeout += OnStrobeTimerTimeout;

		HitBoxComponent.OnHit += StartFlashing;

	}

	private void StartFlashing(float duration)
	{
		durationTimer.Start(duration);
		if (Strobe) strobeTimer.Start();
		((ShaderMaterial)Sprite.Material).SetShaderParameter("dye", true);
		flashTimer.Start(Mathf.Max(duration/4, 0.2));
	}
	
	private void OnStrobeTimerTimeout()
	{
		Sprite.Visible = !Sprite.Visible;
	}
	private void OnDurationTimerTimeout()
	{
		Sprite.Visible = true;
		strobeTimer.Stop();
		((ShaderMaterial)Sprite.Material).SetShaderParameter("dye", false);
	}

	private void OnFlashTimerTimeout()
	{
		((ShaderMaterial)Sprite.Material).SetShaderParameter("dye", false);
	}


}