using DPlatformer.NET.Scripts.CS.Entities.Components;
using Godot;

namespace DPlatformer.NET.Scripts.CS.UI;

[GlobalClass]
public partial class HpBar : Control
{
	private ColorRect remainingHpBar;
	[Export] HealthComponent healthComponent;

	public override void _Ready()
	{
		remainingHpBar = GetNode<ColorRect>("hp");
		if(healthComponent != null) {
			healthComponent.HealthFractionRemaining += Update;
		}
	}
	public void Update(float fraction) {
		remainingHpBar.Scale = remainingHpBar.Scale with { X = fraction };
	}
}