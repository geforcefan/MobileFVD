precision highp float;

varying vec4 oColor;

uniform float Opacity;
uniform int Border;
uniform int Grid;

void main(void)
{
	gl_FragColor = oColor;
}