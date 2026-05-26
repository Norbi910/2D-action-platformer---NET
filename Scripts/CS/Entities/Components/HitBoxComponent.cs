using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Components;

[GlobalClass]
public partial class HitBoxComponent : Area2D
{
	[Export] public HealthComponent HealthComponent;
	[Export] public float InvincibilityDuration = 0.5f;
	//TODO [Export] hitFlashComponent hitFlashComponent
	
	private Timer invincibilityTimer;
	public override void _Ready() {
		invincibilityTimer = GetNode<Timer>("%InvincibilityTimer");
		invincibilityTimer.Timeout += OnInvincibilityTimerTimeout;
	}
	
	[Signal]
	public delegate void OnKnockbackEventHandler(Vector2 force);

	public bool Damage(float damage, Vector2 direction)
	{
		if (HealthComponent == null || !invincibilityTimer.IsStopped())
			return false;
		HealthComponent.Damage(damage);
		float force = Mathf.Max(400, damage * 10);
		EmitSignalOnKnockback(direction * force);
		invincibilityTimer.Start(InvincibilityDuration);

		//TODO if (hitFlash != null) {
		// 	hitFlash.toggleFlashing()
		// }
		return true;
	}
	
	private void OnInvincibilityTimerTimeout() {
		//TODO
		// if (hitflash == null) return;
		// hitflash.toggleflashing()
	}

}