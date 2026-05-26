extends Panel

@onready var item_sprite: TextureRect = $ColorRect/TextureRect

func update(item: InventoryItem):
	if not item:
		item_sprite.visible = false
	else:
		item_sprite.visible = true
		item_sprite.texture = item.texture
