precision highp float;

varying vec3 oColor;

uniform mat4 ProjectionMatrix;
uniform mat4 ModelMatrix;
uniform mat4 AnchorBase;
uniform vec3 EyePos;

attribute vec4 vPosition;
attribute float vVelocity;
attribute float vRollSpeed;
attribute float vForceNormal;
attribute float vForceLateral;
attribute float vFlexion;
attribute float vIsActive;
attribute float vHasColor;
attribute vec4 vColor;

int colorMode = 6;

vec3 getColor()
{
	if(vHasColor > 0.0) {
		return vColor.xyz;
	}

	if(vIsActive > 0.0) {
		return vec3(1.0, 0.0, 0.0);
	} else {
		if(colorMode == 1) { // velocity
			if (vVelocity > 60.0)
				return vec3(1.0, 0.0, 1.0);
			else if (vVelocity >= 40.0)
				return vec3(1.0, 0.0, (vVelocity - 40.0) / 20.0);
			else if (vVelocity >= 30.0)
				return vec3(1.0, (40.0 - vVelocity) / 10.0, 0.0);
			else if (vVelocity >= 20.0)
				return vec3((vVelocity - 20.0) / 10.0, 1.0, 0.0);
			else if (vVelocity >= 10.0)
				return vec3(0.0, 1.0, (20.0 - vVelocity) / 10.0);
			else if (vVelocity >= 1.0)
				return vec3(0.0, (vVelocity - 1.0) / 9.0, 1.0);
			else
				return vec3(0.0, 0.0, 0.0);
		}
		if(colorMode == 2) { // rollspeed
			if (vRollSpeed > 240.0)
				return vec3(0.0, 0.0, 0.0);
			else if (vRollSpeed >= 160.0)
				return vec3((240.0 - vRollSpeed) / 80.0, 0.0, (240.0 - vRollSpeed) / 80.0);
			else if (vRollSpeed >= 80.0)
				return vec3(1.0, 0.0, (vRollSpeed - 80.0) / 80.0);
			else if (vRollSpeed >= 40.0)
				return vec3(1.0, (80.0 - vRollSpeed) / 40.0, 0.0);
			else if (vRollSpeed >= 20.0)
				return vec3((vRollSpeed - 20.0) / 20.0, 1.0, 0.0);
			else if (vRollSpeed >= 10.0)
				return vec3(0.0, 1.0, (20.0 - vRollSpeed) / 10.0);
			else
				return vec3(0.0, vRollSpeed / 10.0, 1.0);
		}
		if(colorMode == 3) { // normal force
			if (vForceNormal > 6.5)
				return vec3(0.0, 0.0, 0.0);
			else if (vForceNormal > 5.0)
				return vec3((6.5 - vForceNormal) / 1.5, 0.0, (6.5 - vForceNormal) / 1.5);
			else if (vForceNormal >= 3.5)
				return vec3(1.0, 0.0, (vForceNormal - 3.5) / 1.5);
			else if (vForceNormal >= 2.0)
				return vec3(1.0, (3.5 - vForceNormal) / 1.5, 0.0);
			else if (vForceNormal >= 1.0)
				return vec3(vForceNormal - 1.0, 1.0, 0.0);
			else if (vForceNormal >= 0.0)
				return vec3(0.0, 1.0, 1.0 - vForceNormal);
			else if (vForceNormal >= -1.0)
				return vec3(0.0, vForceNormal + 1.0, 1.0);
			else if (vForceNormal >= -2.5)
				return vec3(0.0, 0.0, (vForceNormal + 2.5) / (1.5));
			else
				return vec3(0.0, 0.0, 0.0);
		}
		if(colorMode == 4) { // lateral force
			if (vForceLateral > 2.0)
				return vec3(0.0, 0.0, 0.0);
			else if (vForceLateral >= 1.5)
				return vec3((2.0 - vForceLateral) / 0.5, 0.0, (2.0 - vForceLateral) / 0.5);
			else if (vForceLateral >= 1.0)
				return vec3(1.0, 0.0, (vForceLateral - 1.0) / 0.5);
			else if (vForceLateral >= 0.5)
				return vec3(1.0, (1.0 - vForceLateral) / 0.5, 0.0);
			else if (vForceLateral >= 0.25)
				return vec3((vForceLateral - 0.25) / 0.25, 1.0, 0.0);
			else if (vForceLateral >= 0.1)
				return vec3(0.0, 1.0, (0.25 - vForceLateral) / 0.15);
			else
				return vec3(0.0, vForceLateral * 10.0, 1.0);
		}
		if(colorMode == 5) { // flexion
			if (vFlexion > 30.0)
				return vec3(0.0, 0.0, 0.0);
			else if (vFlexion >= 6.0)
				return vec3((30.0 - vFlexion) / 24.0, 0.0, (30.0 - vFlexion) / 24.0);
			else if (vFlexion >= 4.5)
				return vec3(1.0, 0.0, (vFlexion - 4.5) / 1.5);
			else if (vFlexion >= 3.5)
				return vec3(1.0, (4.5 - vFlexion) / 1.0, 0.0);
			else if (vFlexion >= 2.5)
				return vec3((vFlexion - 2.5) / 1.0, 1.0, 0.0);
			else if (vFlexion >= 1.0)
				return vec3(0.0, 1.0, (2.5 - vFlexion) / 1.5);
			else
				return vec3(0.0, vFlexion, 1.0);
		}
		if(colorMode == 6) { // default
			return vec3(0.0, 0.50, 0.54);
		}
	}
}

void main()   
{
    oColor = getColor();

    vec4 bPosition = AnchorBase * vec4(vPosition.xyz, 1.0);
    gl_Position = ProjectionMatrix * ModelMatrix * bPosition;
}