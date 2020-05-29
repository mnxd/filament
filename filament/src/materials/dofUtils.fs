
/*
 * DoF Utils
 */

#define MAX_IN_FOCUS_COC    0.5

// The maximum circle-of-confusion radius we allow in high-resolution pixels.
// This is limited by our tile size (hard limit) and dilate pass as well as the kernel density
// (soft/quality limit).
#define MAX_COC_RADIUS      32.0

float random(const highp vec2 w) {
    const vec3 m = vec3(0.06711056, 0.00583715, 52.9829189);
    return fract(m.z * fract(dot(w, m.xy)));
}

float min2(vec2 v) {
    return min(v.x, v.y);
}

float max2(vec2 v) {
    return max(v.x, v.y);
}

float max4(vec4 v) {
    return max2(max(v.xy, v.zw));
}

float min4(vec4 v) {
    return min2(min(v.xy, v.zw));
}

float cocToAlpha(float coc) {
    // CoC is positive for background field.
    // CoC is negative for the foreground field.
    return saturate(abs(coc) - MAX_IN_FOCUS_COC);
}

// returns circle-of-confusion diameter in pixels
float getCOC(float depth, vec2 cocParams) {
    return depth * cocParams.x + cocParams.y;
}

vec4 getCOC(vec4 depth, vec2 cocParams) {
    return depth * cocParams.x + cocParams.y;
}

float isForeground(float coc) {
    return coc < 0.0 ? 1.0 : 0.0;
}

float isBackground(float coc) {
    return coc > 0.0 ? 1.0 : 0.0;
}

bool isForegroundTile(vec2 tiles) {
    // A foreground tile is one where the smallest CoC is negative
    return tiles.g < 0.0;
}

bool isBackgroundTile(vec2 tiles) {
    // A background tile is one where the largest CoC is positive
    return tiles.r > 0.0;
}

bool isFastTile(vec2 tiles) {
    float maxCocRadius = max(abs(tiles.r), abs(tiles.g));
    float minCocRadius = min(abs(tiles.r), abs(tiles.g));
    return (maxCocRadius - minCocRadius) < maxCocRadius * 0.05;
}

bool isTrivialTile(vec2 tiles) {
    float maxCocRadius = max(abs(tiles.r), abs(tiles.g));
    return maxCocRadius < MAX_IN_FOCUS_COC;
}
