#version 300 es
precision highp float;

out vec4 oColor;

uniform mat4 ProjectionMatrix;
uniform mat4 ModelMatrix;
uniform mat4 AnchorBase;
uniform vec3 EyePos;

layout(location = 0) in vec4 vPosition;
layout(location = 1) in vec4 vPickingColor;

void main()   
{
    oColor = vPickingColor;

    vec4 bPosition = AnchorBase * vec4(vPosition.xyz, 1.0f);
    gl_Position = ProjectionMatrix * ModelMatrix * bPosition;
}