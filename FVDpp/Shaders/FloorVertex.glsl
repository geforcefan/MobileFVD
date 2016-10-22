#version 300 es
precision highp float;

in vec4 vPosition;
in vec4 vColor;

out vec2 RasterCoordinate;
out vec2 FloorCoordinate;
out vec4 ScreenCoordinate;

out vec4 oColor;

uniform mat4 ProjectionMatrix;
uniform mat4 ModelMatrix;
uniform vec3 EyePos;

void main()   
{
	/*vec3 eye = EyePos.xyz;
    eye.y = 0.0f;

    gl_Position = ProjectionMatrix * ModelMatrix * vec4(vPosition.xyz + eye, 1.0f);

    RasterCoordinate = 0.1f * (vPosition.xz + eye.xz + vec2(5.0f, 5.0f));
    FloorCoordinate = (vPosition.xz + eye.xz + vec2(220.0f, 220.0f)) / 440.0f;
	ScreenCoordinate = gl_Position;*/

	oColor = vColor;
    gl_Position = ProjectionMatrix * ModelMatrix * vec4(vPosition.xyz, 1.0f);
}