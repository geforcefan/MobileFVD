precision highp float;

varying vec4 oColor;

uniform mat4 ProjectionMatrix;
uniform mat4 ModelMatrix;
uniform mat4 AnchorBase;
uniform vec3 EyePos;

attribute vec4 vPosition;
attribute vec4 vPickingColor;

void main()   
{
    oColor = vPickingColor;

    vec4 bPosition = AnchorBase * vec4(vPosition.xyz, 1.0);
    gl_Position = ProjectionMatrix * ModelMatrix * bPosition;
}