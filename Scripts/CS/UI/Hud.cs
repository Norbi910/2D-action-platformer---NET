using DPlatformer.NET.Scripts.CS.Entities.Player;
using DPlatformer.NET.Scripts.CS.Misc;
using Godot;

namespace DPlatformer.NET.Scripts.CS.UI;

[GlobalClass]
public partial class Hud : CanvasLayer
{
	[Export]
	public Player Player;
	
	private HpBar hpBar;
	private Control bar;
	private Control deathMenu;
	private Button restartButton;
	private Label winLabel;

	public override void _Ready()
	{
		hpBar = GetNode<HpBar>("%HPBar");
		restartButton = GetNode<Button>("%RestartButton");
		restartButton.Pressed += OnRestartButtonPressed;
		deathMenu = GetNode<Control>("%DeathMenu");
		bar = GetNode<Control>("%Bar");
		winLabel = GetNode<Label>("%WinLabel");
		
		Player.HealthChanged += Update;
		
		GetNode<PauseMenu>("PauseMenu").HintsToggle += OnPauseMenuHintsToggle;
	}

	private static readonly Inventory PlayerData =
		ResourceLoader.Load<Inventory>("uid://bvyijoa5sha6v");

	[Signal] 
	public delegate void HintsToggledEventHandler(bool state);

	public override void _Process(double delta)
	{
		if (PlayerData.CompletedQuests.Contains("bunny_star"))
			Win();
	}

	private void Win()
	{
		winLabel.Visible = true;
	}

	public void Update(float hp)
	{
		if (hpBar == null || deathMenu == null) return;
		hpBar.Update(hp/Player.GetMaxHealth());

		if (hp <= 0) {
			deathMenu.Visible = true;
			bar.Visible = false;
		}
		
	}

	public void OnPauseMenuHintsToggle(bool state)
	{
		EmitSignalHintsToggled(state);
	}

	public void OnRestartButtonPressed()
	{
		GetTree().ReloadCurrentScene();
	}
}