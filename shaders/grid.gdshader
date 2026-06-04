shader_type canvas_item;

uniform vec4 dot_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec2 rect_size = vec2(200.0, 200.0);
uniform int space_between_pixels = 10;

void fragment() {
	vec2 current_pos = UV * rect_size;
	
	// Cast to ivec2 so we can use the modulo operator
	ivec2 pixel_pos = ivec2(current_pos);
	
	if (pixel_pos.x % space_between_pixels == 0 && pixel_pos.y % space_between_pixels == 0) {
		// Replace color with dot color
		COLOR = dot_color;
	} 
}