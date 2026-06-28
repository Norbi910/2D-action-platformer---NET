using DPlatformer.NET.Scripts.CS.Entities.Player;
using DPlatformer.NET.Scripts.CS.Misc;
using Godot;

namespace DPlatformer.NET.Scripts.CS.UI;

public partial class Hud : CanvasLayer
{
	public static Hud Singleton { get; private set; }
	
	private float maxHealth;
	
	private HpBar hpBar;
	private Label deathLabel;
	private Label winLabel;

	public override void _Ready()
	{
		hpBar = GetNode<HpBar>("$Control/HPBar");
		deathLabel = GetNode<Label>("$DeathLabel");
		winLabel = GetNode<Label>("$WinLabel");
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
		hpBar.Update(hp / maxHealth);
		deathLabel.Visible = hp == 0;
	}

	public void OnPauseMenuHintsToggle(bool state)
	{
		EmitSignal(SignalName.HintsToggled, state);
	}

	public void RegisterPlayer(Player player)
	{
		player.HealthChanged += Update;
		maxHealth = player.GetMaxHealth();
	}
	


}