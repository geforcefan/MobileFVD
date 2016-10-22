#version 300 es
precision highp float;

in vec4 vPosition;
out vec3 vTexCoord;

uniform vec3 TopLeft;
uniform vec3 TopRight;
uniform vec3 BottomLeft;
uniform vec3 BottomRight;

void main()   
{
    gl_Position = vPosition;

    if(vPosition.x < -0.5)
    {
        if(vPosition.y < -0.5)
        {
            vTexCoord = BottomLeft;
        }
        else
        {
            vTexCoord = TopLeft;
        }
    }
    else
    {
        if(vPosition.y < -0.5)
        {
            vTexCoord = BottomRight;
        }
        else
        {
            vTexCoord = TopRight;
        }
	}
}