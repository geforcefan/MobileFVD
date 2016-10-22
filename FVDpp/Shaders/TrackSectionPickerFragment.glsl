#version 300 es
precision highp float;

out vec4 oFragColor;
in vec4 oColor;

void main(void)
{
    oFragColor = vec4(oColor.xyz, 1.0f);
}