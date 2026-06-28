using System;
using Godot;

namespace DPlatformer.NET.Scripts.CS.UI;

public partial class PauseMenu : Control
{
	[Signal] public delegate void HintsToggleEventHandler(bool state);

	private MarginContainer settingsMenu;
	private MarginContainer baseMenu;
	
	private Button resumeButton;
	private Button settingsButton;
	private Button quitButton;
	private HSlider volumeSlider;
	private CheckButton hintButton;

	public override void _Ready()
	{
		settingsMenu = GetNode<MarginContainer>("%SettingsMenu");
		baseMenu = GetNode<MarginContainer>("%BaseMenu");
		
		resumeButton = GetNode<Button>("%ResumeButton");
		resumeButton.Pressed += OnResumeButtonPressed;
		settingsButton = GetNode<Button>("%SettingsButton");
		settingsButton.Pressed += OnSettingsButtonPressed;
		quitButton = GetNode<Button>("%QuitButton");
		quitButton.Pressed += OnQuitButtonPressed;
		volumeSlider = GetNode<HSlider>("%VolumeSlider");
		volumeSlider.ValueChanged += OnHSliderValueChanged;
		hintButton = GetNode<CheckButton>("%HintButton");
		hintButton.Pressed += OnHintButtonPressed;
		
		
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			if (settingsMenu.Visible)
			{
				settingsMenu.Visible = false;
				baseMenu.Visible = true;
			}
			else if (Visible)
			{
				Visible = false;
				GetTree().Paused = false;
			}
			else
			{
				Visible = true;
				GetTree().Paused = true;
			}
		}
	}

	private void OnResumeButtonPressed()
	{
		settingsMenu.Visible = false;
		baseMenu.Visible = true;
		Input.ActionPress("pause");
	}

	private void OnQuitButtonPressed() => GetTree().Quit();

	private void OnSettingsButtonPressed()
	{
		baseMenu.Visible = false;
		settingsMenu.Visible = true;
	}

	private void OnHSliderValueChanged(double value)
	{
		AudioServer.SetBusVolumeDb(0, (float)value);
		AudioServer.SetBusMute(0, Math.Abs(value - volumeSlider.MinValue) < 0.01f);
	}

	private void OnHintButtonPressed() =>
		EmitSignalHintsToggle(hintButton.ButtonPressed);
}