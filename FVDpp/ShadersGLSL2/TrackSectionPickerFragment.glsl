precision highp float;

varying vec4 oColor;

void main(void)
{
    gl_FragColor = vec4(oColor.xyz, 1.0);
}