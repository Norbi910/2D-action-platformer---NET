using Godot;
using Godot.Collections;

namespace DPlatformer.NET.Scripts.CS.Misc;

public partial class Inventory : Resource
{
	[Export] public Array<InventoryItem> Items;
	[Export] public Array<StringName> CompletedQuests;

	public void Reset()
	{
		Items.Clear();
		CompletedQuests.Clear();
	}

	public bool HasItem(StringName name)
	{
		foreach (InventoryItem item in Items) {
			if (item.Name == name)
				return  true;
		}
		return false;
	}

	public void RemoveItem(StringName name)
	{
		foreach (InventoryItem item in Items) {
			if (item.Name == name) {
				Items.Remove(item);
				EmitChanged();
				return;
			}
		}
	}
}