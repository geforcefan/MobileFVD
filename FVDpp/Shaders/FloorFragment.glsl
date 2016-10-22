#version 300 es
precision highp float;

in vec4 oColor;
out vec4 oFragColor;

uniform sampler2D RasterTexture;
uniform sampler2D FloorTexture;
//uniform sampler2D ShadowTexture;

in vec2 RasterCoordinate;
in vec2 FloorCoordinate;
in vec4 ScreenCoordinate;

uniform float Opacity;
uniform int Border;
uniform int Grid;

void main(void)
{
    /*oFragColor = texture(FloorTexture, FloorCoordinate);
    oFragColor.w = Opacity;

    //oFragColor = vec4(0.0f, 0.0f, 0.0f, 0.5f);

    if((FloorCoordinate.x > 1.0f || FloorCoordinate.y > 1.0f || FloorCoordinate.x < 0.0f || FloorCoordinate.y < 0.0f) && Border == 1)
    {
        oFragColor.xyz = vec3(0.5f, 0.5f, 0.5f);
    }

    if(Grid == 1) oFragColor.xyz *= (texture(RasterTexture, RasterCoordinate).x);

    float visible = 1.0f;
    oFragColor.xyz -= 0.5f * (1.0f - visible);
    oFragColor.xyz /= Opacity;
	oFragColor.xyz = pow(clamp(oFragColor.xyz, 0.0f, 1.0f), vec3(1.0f / 2.2f, 1.0f / 2.2f, 1.0f / 2.2f));*/

	oFragColor = oColor;
}