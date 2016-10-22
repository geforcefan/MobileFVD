precision highp float;

attribute vec4 vPosition;
attribute vec4 vColor;

varying vec2 RasterCoordinate;
varying vec2 FloorCoordinate;
varying vec4 ScreenCoordinate;

varying vec4 oColor;

uniform mat4 ProjectionMatrix;
uniform mat4 ModelMatrix;
uniform vec3 EyePos;

void main()   
{
	oColor = vColor;
    gl_Position = ProjectionMatrix * ModelMatrix * vec4(vPosition.xyz, 1.0);
}