shader_type canvas_item;

uniform sampler2D texture_source : source_color, filter_nearest;
uniform int region_count : hint_range(1, 16) = 4;
uniform ivec2 region_size_px = ivec2(64, 64);
uniform float seed : hint_range(0.0, 1.0) = 0.123;
uniform float tiles_per_unit : hint_range(0.0, 20.0) = 5.0;
uniform float line_length_scale : hint_range(0.0, 1.0, 0.00001) = 0.15625;  // 50 (rope length) / 320(pixel perfect screen y)

void fragment() {
    ivec2 texture_size = textureSize(texture_source, 0);
    vec2 region_size = vec2(region_size_px) / vec2(texture_size);
    ivec2 region_grid = ivec2(texture_size) / region_size_px;

    // Scale UV.x by estimated line length
    float effective_progress = UV.x * (tiles_per_unit * line_length_scale);
    float tile_progress = floor(effective_progress);

	// Choose random index
    float rand_val = fract(sin((tile_progress + seed) * 12.9898) * 43758.5453);
    int region_idx = int(floor(rand_val * float(region_count)));

	// Wrap+clamp
	region_idx = int(mod(float(region_idx), float(region_count)));
	region_idx = min(region_idx, region_grid.x * region_grid.y - 1);

	// Calculate region offset
	ivec2 region_pos = ivec2(region_idx % region_grid.x, region_idx / region_grid.x);
    vec2 region_offset = vec2(region_pos) * region_size;

	vec2 tile_uv = fract(UV * vec2(tiles_per_unit * line_length_scale, 1.0));
	vec2 texel_uv = floor(tile_uv * vec2(region_size_px)) / vec2(region_size_px);
	vec2 corrected_uv = vec2(texel_uv.y, 1.0 - texel_uv.x);
	vec2 region_uv = corrected_uv * region_size + region_offset;

    COLOR = texture(texture_source, region_uv);

}