using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Components;

[GlobalClass]
public partial class AttackComponent : Area2D
{

	[Export] public float AttackDamage = 20f;

	[Signal]
	public delegate void DamageDealtEventHandler();
	
	public override void _Process(double delta) {
		if (HasOverlappingAreas()) return;
		foreach (var area in GetOverlappingAreas()) {
			if (area is not HitBoxComponent) continue;
			HitBoxComponent hitbox = area as HitBoxComponent;
			Vector2 knockbackDirection = (area.GlobalPosition - GlobalPosition).Normalized();
			if (hitbox.Damage(AttackDamage,  knockbackDirection))
				EmitSignalDamageDealt();
		}
	}
}