using System;
using Godot;

namespace DPlatformer.NET.Scripts.CS.Misc;

[GlobalClass]
public partial class InventoryItem : Resource
{
	[Export] public String Name = "";
	[Export] public Texture2D Texture;

}