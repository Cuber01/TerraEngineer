shader_type canvas_item;

uniform sampler2D texture_source : source_color; //filter_nearest
uniform int region_count : hint_range(1, 16) = 4;
uniform ivec2 region_size_px = ivec2(64, 64);
uniform float seed : hint_range(0.0, 1.0) = 0.123;
uniform float tiles_per_unit : hint_range(1.0, 20.0) = 1.0;

void fragment() {
    ivec2 texture_size = textureSize(texture_source, 0);
    vec2 region_size = vec2(region_size_px) / vec2(texture_size);
    ivec2 region_grid = ivec2(texture_size) / region_size_px;
    
    // Per-tile randomness using UV.x progress
    float tile_progress = floor(UV.x * tiles_per_unit);
    float rand_val = fract(sin((tile_progress + seed) * 12.9898) * 43758.5453);
    int region_idx = int(floor(rand_val * float(region_count)));
    
    // Clamp to available regions
    region_idx = min(region_idx, region_grid.x * region_grid.y - 1);
    
    ivec2 region_pos = ivec2(region_idx % region_grid.x, region_idx / region_grid.x);
    vec2 region_offset = vec2(region_pos) * region_size;
    
    // We need to apply 90deg rotation to compensate for godot rotating the texture by default
    vec2 local_tile_uv = vec2(fract(UV.x * tiles_per_unit), UV.y);
    vec2 rotated_uv = vec2(1.0 - local_tile_uv.y, local_tile_uv.x); 
    vec2 region_uv = rotated_uv * region_size + region_offset;
    
    COLOR = texture(texture_source, region_uv);
}
