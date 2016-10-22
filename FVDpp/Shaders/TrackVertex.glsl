#version 300 es
precision highp float;

out vec3 oColor;

uniform mat4 ProjectionMatrix;
uniform mat4 ModelMatrix;
uniform mat4 AnchorBase;
uniform vec3 EyePos;
uniform int ActiveSection;

layout(location = 0) in vec4 vPosition;
layout(location = 1) in float vVelocity;
layout(location = 2) in float vRollSpeed;
layout(location = 3) in float vForceNormal;
layout(location = 4) in float vForceLateral;
layout(location = 5) in float vFlexion;
layout(location = 6) in int vSectionIndex;
layout(location = 7) in int vIsHeartline;

int colorMode = 6;

vec3 getColor()
{
	if(vIsHeartline == 1) {
		return vec3(1.0f, 1.0f, 0.0f);
	} else if(vSectionIndex == ActiveSection) {
		return vec3(1.0f, 0.0f, 0.0f);
	} else {
		switch (colorMode)
		{
			case 1: // velocity
				if (vVelocity > 60.0f)
					return vec3(1.0f, 0.0f, 1.0f);
				else if (vVelocity >= 40.0f)
					return vec3(1.0f, 0.0f, (vVelocity - 40.0f) / 20.0f);
				else if (vVelocity >= 30.0f)
					return vec3(1.0f, (40.0f - vVelocity) / 10.0f, 0.0f);
				else if (vVelocity >= 20.0f)
					return vec3((vVelocity - 20.0f) / 10.0f, 1.0f, 0.0f);
				else if (vVelocity >= 10.0f)
					return vec3(0.0f, 1.0f, (20.0f - vVelocity) / 10.0f);
				else if (vVelocity >= 1.0f)
					return vec3(0.0f, (vVelocity - 1.0f) / 9.0f, 1.0f);
				else
					return vec3(0.0f, 0.0f, 0.0f);
			case 2: // rollspeed
				if (vRollSpeed > 240.0f)
					return vec3(0.0f, 0.0f, 0.0f);
				else if (vRollSpeed >= 160.0f)
					return vec3((240.0f - vRollSpeed) / 80.0f, 0.0f, (240.0f - vRollSpeed) / 80.0f);
				else if (vRollSpeed >= 80.0f)
					return vec3(1.0f, 0.0f, (vRollSpeed - 80.0f) / 80.0f);
				else if (vRollSpeed >= 40.0f)
					return vec3(1.0f, (80.0f - vRollSpeed) / 40.0f, 0.0f);
				else if (vRollSpeed >= 20.0f)
					return vec3((vRollSpeed - 20.0f) / 20.0f, 1.0f, 0.0f);
				else if (vRollSpeed >= 10.0f)
					return vec3(0.0f, 1.0f, (20.0f - vRollSpeed) / 10.0f);
				else
					return vec3(0.0f, vRollSpeed / 10.0f, 1.0f);
			case 3: // normal force
				if (vForceNormal > 6.5f)
					return vec3(0.0f, 0.0f, 0.0f);
				else if (vForceNormal > 5.0f)
					return vec3((6.5f - vForceNormal) / 1.5f, 0.0f, (6.5f - vForceNormal) / 1.5f);
				else if (vForceNormal >= 3.5f)
					return vec3(1.0f, 0.0f, (vForceNormal - 3.5f) / 1.5f);
				else if (vForceNormal >= 2.0f)
					return vec3(1.0f, (3.5f - vForceNormal) / 1.5f, 0.0f);
				else if (vForceNormal >= 1.0f)
					return vec3(vForceNormal - 1.0f, 1.0f, 0.0f);
				else if (vForceNormal >= 0.0f)
					return vec3(0.0f, 1.0f, 1.0f - vForceNormal);
				else if (vForceNormal >= -1.0f)
					return vec3(0.0f, vForceNormal + 1.0f, 1.0f);
				else if (vForceNormal >= -2.5f)
					return vec3(0.0f, 0.0f, (vForceNormal + 2.5f) / (1.5f));
				else
					return vec3(0.0f, 0.0f, 0.0f);
			case 4: // lateral force
				if (vForceLateral > 2.0f)
					return vec3(0.0f, 0.0f, 0.0f);
				else if (vForceLateral >= 1.5f)
					return vec3((2.0f - vForceLateral) / 0.5f, 0.0f, (2.0f - vForceLateral) / 0.5f);
				else if (vForceLateral >= 1.0f)
					return vec3(1.0f, 0.0f, (vForceLateral - 1.0f) / 0.5f);
				else if (vForceLateral >= 0.5f)
					return vec3(1.0f, (1.0f - vForceLateral) / 0.5f, 0.0f);
				else if (vForceLateral >= 0.25f)
					return vec3((vForceLateral - 0.25f) / 0.25f, 1.0f, 0.0f);
				else if (vForceLateral >= 0.1f)
					return vec3(0.0f, 1.0f, (0.25f - vForceLateral) / 0.15f);
				else
					return vec3(0.0f, vForceLateral * 10.0f, 1.0f);
			case 5: // flexion
				if (vFlexion > 30.0f)
					return vec3(0.0f, 0.0f, 0.0f);
				else if (vFlexion >= 6.0f)
					return vec3((30.0f - vFlexion) / 24.0f, 0.0f, (30.0f - vFlexion) / 24.0f);
				else if (vFlexion >= 4.5f)
					return vec3(1.0f, 0.0f, (vFlexion - 4.5f) / 1.5f);
				else if (vFlexion >= 3.5f)
					return vec3(1.0f, (4.5f - vFlexion) / 1.0f, 0.0f);
				else if (vFlexion >= 2.5f)
					return vec3((vFlexion - 2.5f) / 1.0f, 1.0f, 0.0f);
				else if (vFlexion >= 1.0f)
					return vec3(0.0f, 1.0f, (2.5f - vFlexion) / 1.5f);
				else
					return vec3(0.0f, vFlexion, 1.0f);
			case 6: // default
				return vec3(0.0f, 0.50f, 0.54f);
		}
	}
}

void main()   
{
    oColor = getColor();

    vec4 bPosition = AnchorBase * vec4(vPosition.xyz, 1.0f);
    gl_Position = ProjectionMatrix * ModelMatrix * bPosition;
}