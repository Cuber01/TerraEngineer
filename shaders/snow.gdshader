shader_type particles;

uniform float movement_speed_x = 150.0;     
uniform float movement_speed_y = 6.0;     
uniform float base_wave_speed = 4.0;      
uniform float max_wave_amplitude = 30.0;   
uniform float spawn_spread_y = 180.0;      

float rand(inout uint seed) {
    seed = (seed ^ 61u) ^ (seed >> 16u);
    seed *= 9u;
    seed = seed ^ (seed >> 4u);
    seed *= 0x27d4eb2du;
    seed = seed ^ (seed >> 15u);
    return float(seed) / 4294967296.0;
}

void start() {
    uint seed = NUMBER + RANDOM_SEED;

    TRANSFORM[3].x = 0.0; 
    float random_y_offset = rand(seed) * (spawn_spread_y / 2.0);
    TRANSFORM[3].y = random_y_offset;

    // Bake unique custom properties into each pixel so they don't look identical
    // CUSTOM.x = amplitude multiplier
    // CUSTOM.y = frequency modifier
    // CUSTOM.z = starting time phase
    CUSTOM.x = rand(seed) * max_wave_amplitude; 
    CUSTOM.y = base_wave_speed + (rand(seed) * 2.0 - 1.0); 
    CUSTOM.z = rand(seed) * 6.28; // 6.28=2*PI
}

void process() {    
    TRANSFORM[3].x -= movement_speed_x * DELTA;

    float my_amplitude = CUSTOM.x;
    float my_freq     = CUSTOM.y;
    float my_phase     = CUSTOM.z;

	// CUSTOM.w = current position in sin, so taking frequency into account unlike TRANSFORM.x
    CUSTOM.w += DELTA * my_freq; 
    float sine_wave = sin(CUSTOM.w + my_phase) * my_amplitude;

    TRANSFORM[3].y += sine_wave * DELTA * movement_speed_y; 
}