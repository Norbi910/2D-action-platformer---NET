shader_type canvas_item;

uniform bool dye = false;
uniform vec3 dyeColor : source_color = vec3(1.000000, 1.000000, 1.000000);
uniform float dyeRate = 0.6;

void fragment() {
	vec4 texColor = texture(TEXTURE, UV);
	if(dye){
		vec3 dyedTextureRGB = mix(texColor.rgb ,dyeColor ,dyeRate);
		COLOR = vec4(dyedTextureRGB, texColor.a);
	} else {
		COLOR = texColor;
	}
}
