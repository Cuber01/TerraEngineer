shader_type canvas_item;

uniform sampler2D texture_source : source_color, filter_nearest;
uniform int region_count : hint_range(1, 16) = 4;
uniform ivec2 region_size_px = ivec2(64, 64);
uniform float seed : hint_range(0.0, 1.0) = 0.123;

void fragment() {
    ivec2 texture_size = textureSize(texture_source, 0);
    vec2 region_size = vec2(region_size_px) / vec2(texture_size);
    ivec2 region_grid = ivec2(texture_size) / region_size_px;
    int total_regions = region_grid.x * region_grid.y;
    int clamped_count = min(region_count, total_regions);

    // Stable per-instance randomness using only seed (same region everywhere)
    float rand_val = fract(sin(seed * 12.9898) * 43758.5453);
    int region_idx = int(floor(rand_val * float(clamped_count)));

    // Region position
    ivec2 region_pos = ivec2(region_idx % region_grid.x, region_idx / region_grid.x);
    vec2 region_offset = vec2(region_pos) * region_size;

    vec2 region_uv = UV * region_size + region_offset;

    COLOR = texture(texture_source, region_uv);
}