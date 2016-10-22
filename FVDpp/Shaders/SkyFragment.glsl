#version 300 es
precision highp float;

out vec4 oFragColor;
in vec3 vTexCoord;

uniform samplerCube SkyTexture;

void main(void)
{
    oFragColor = vec4(texture(SkyTexture, normalize(vTexCoord)).rgb, 1);
}