using Godot;

namespace DPlatformer.NET.Scripts.CS.UI;

[GlobalClass]
public partial class HpBar : Control
{
	private ColorRect remainingHpBar;

	public override void _Ready()
	{
		remainingHpBar = GetNode<ColorRect>("$hp");
	}
	public void Update(float percentage) => remainingHpBar.Scale = remainingHpBar.Scale with { X = percentage };
}