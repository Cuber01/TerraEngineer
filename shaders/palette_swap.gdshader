shader_type canvas_item;
uniform sampler2D palette;

void fragment() {
    vec4 tex_color = texture(TEXTURE, UV);
	
	// Ignore white
	if(tex_color != vec4(1.0, 1.0, 1.0, 1.0))
	{
		vec4 new_color = texture(palette, vec2(tex_color.r, 0));
    	COLOR = vec4(new_color.rgb, tex_color.a);	
	}
    
}
