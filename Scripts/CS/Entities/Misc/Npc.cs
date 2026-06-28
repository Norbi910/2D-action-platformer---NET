using DPlatformer.NET.Scripts.CS.Misc;
using Godot;

namespace DPlatformer.NET.Scripts.CS.Entities.Misc;

[GlobalClass]
public partial class Npc : CharacterBody2D
{
	private Label textBox;
	private Inventory playerData;
	private enum QuestState {
		None, Accepted, Completed
	}
	[Export] QuestState questState = QuestState.None;
	
	public override void _Ready()
	{
		textBox = GetNode<Label>("$TextBox");
		textBox.Visible = false;
		
		playerData = GD.Load<Inventory>("res://Resources/Inventory/player_inventory.tres");
		
	}

	public void Interact()
	{
		if (playerData.HasItem("star")) {
			playerData.RemoveItem("star");
			questState = QuestState.Completed;
			playerData.CompletedQuests.Add("bunny_star");
		}

		if (textBox.Visible) {
			Talk();
		}

		UpdateDialogue();
		textBox.Visible = true;
	}

	private void Talk()
	{
		textBox.LinesSkipped += 1;

		if (questState == QuestState.None && textBox.LinesSkipped == textBox.GetLineCount()) {
			questState = QuestState.Accepted;
		}
		textBox.LinesSkipped %= textBox.GetLineCount();

	}

	private void UpdateDialogue()
	{
		textBox.Text = questState switch
		{
			QuestState.None =>
				"Hi!\nMy name is bun!\nNice to meet you!",
			QuestState.Accepted =>
				"Bring me a Star and Come back! \nYou'll find it East of here!\nIt's guarded by a HUGE ANT",
			QuestState.Completed =>
				"Thanks for bringing me this star!\nSo shinyy!\nI love it <3",
			_ => textBox.Text
		};
	}
}