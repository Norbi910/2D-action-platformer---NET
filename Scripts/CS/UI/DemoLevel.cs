using Godot;

namespace DPlatformer.NET.Scripts.CS.UI;

public partial class DemoLevel : Node2D
{
	private Control labels;

	public override void _Ready()
	{
		labels = GetNode<Control>("Labels");
		GetNode<Hud>("%Hud").HintsToggled += HudHintsSwitch;
	}

	private void HudHintsSwitch(bool state)
	{
		labels.Visible = state;
	}
}