#version 330 core

in vec2 fragCoord;
out vec4 FragColor;

uniform vec2 center;
uniform float scale;
uniform int maxIterations;
uniform vec3 color1;
uniform vec3 color2;

void main()
{
    vec2 c = (fragCoord * scale) + center;
    vec2 z = vec2(0.0);

    int iter;

    for (iter = 0; iter < maxIterations; iter++)
    {
        z = vec2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y) + c;
        if (dot(z, z) > 4.0) break;
    }

    float t = float(iter) / float(maxIterations);

    vec3 color;
    if (iter == maxIterations) {
        color = vec3(0.0);
    }
    else {
        float t = float(iter) / float(maxIterations);
        color = mix(color1, color2, t);
    }

    FragColor = vec4(color, 1.0);
}