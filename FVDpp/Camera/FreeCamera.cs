using System;
using GlmNet;

namespace FVD.Camera
{
	public class FreeCamera : Camera
	{
		public vec3 cameraSide = new vec3(1.0f, 0.0f, 0.0f);
		public vec3 cameraDir = new vec3(0.0f, 0.0f, -1.0f);

		private float maxHeight = 200.0f;
		private float minHeight = -40.0f;

		private float zoomGestureLastScale;

		private vec2 panGestureLastPoint = new vec2();
		private int panGestureStartTouches;

		private vec3 moveToCameraPos = new vec3();
		private vec3 moveToCameraDir = new vec3();
		private vec3 moveToCameraSide = new vec3();

		private float changeSpeed = 0.03f; // USE THIS VALUE IN PRODUCTION
		//private float changeSpeed = 1.0f;

		public FreeCamera()
		{
			moveToCameraPos = cameraPos;
			moveToCameraDir = cameraDir;
			moveToCameraSide = cameraSide;

			OnTapRecognized = (Gestures.TapGestureRecognizer gestureRecognizer) => {
				if (gestureRecognizer.state == Gestures.RecognizerState.Ended)
				{ 
					moveToCameraPos = cameraPos;
					moveToCameraDir = cameraDir;
					moveToCameraSide = cameraSide;
				}
			};

			OnPanRecognized = (Gestures.PanGestureRecognizer gestureRecognizer) =>
			{
				if (gestureRecognizer.state == Gestures.RecognizerState.Began)
				{
					panGestureStartTouches = gestureRecognizer.numberOfTouches;
				}

				if (panGestureStartTouches == 1 && gestureRecognizer.numberOfTouches == 1)
				{
					if (gestureRecognizer.state == Gestures.RecognizerState.Moved ||
						gestureRecognizer.state == Gestures.RecognizerState.Began)
					{
						float relativeCameraPosY = Math.Abs(cameraPos.y) / maxHeight;
						float factor = lerp(relativeCameraPosY, 0.001f, 0.2f);

						vec4 cameraMov = new vec4(0.0f, 0.0f, 0.0f, 0.0f);
						float cameraBoost = 1.0f;
						float cameraJump = 0.0f;

						// Backward
						if (gestureRecognizer.velocity.y < 0)
							cameraMov.z = Math.Abs(gestureRecognizer.velocity.y) * factor;

						// Forward
						if (gestureRecognizer.velocity.y > 0)
							cameraMov.x = gestureRecognizer.velocity.y * factor;

						// Left
						if (gestureRecognizer.velocity.x > 0)
							cameraMov.y = gestureRecognizer.velocity.x * factor;

						// Right
						if (gestureRecognizer.velocity.x < 0)
							cameraMov.w = Math.Abs(gestureRecognizer.velocity.x) * factor;

						float y = moveToCameraPos.y;
						moveToCameraPos += 0.3f * cameraBoost * (cameraMov.x - cameraMov.z) * cameraDir - 0.3f * cameraBoost * (cameraMov.y - cameraMov.w) * glm.normalize(glm.cross(cameraDir, new vec3(0.0f, 1.0f, 0.0f)));
						moveToCameraPos += 0.01f * cameraJump * cameraBoost * glm.cross(cameraDir, glm.normalize(glm.cross(cameraDir, new vec3(0.0f, 1.0f, 0.0f))));
						moveToCameraPos.y = Math.Min(maxHeight, Math.Max(minHeight, y));
					}
				}

				if (panGestureStartTouches == 2 && gestureRecognizer.numberOfTouches == 2)
				{
					if (gestureRecognizer.state == Gestures.RecognizerState.Began)
					{
						panGestureLastPoint = gestureRecognizer.point;
					}

					if (gestureRecognizer.state == Gestures.RecognizerState.Moved)
					{
						Rotate(new vec2((panGestureLastPoint.x - gestureRecognizer.point.x) * 0.75f,
						                (panGestureLastPoint.y - gestureRecognizer.point.y) * 0.75f));
						
						panGestureLastPoint = gestureRecognizer.point;
					}
				}
			};

			OnPitchRecognized = (Gestures.PitchGestureRecognizer gestureRecognizer) =>
			{
				if (gestureRecognizer.state == Gestures.RecognizerState.Began)
				{
					zoomGestureLastScale = gestureRecognizer.scale;
				}
				else if (gestureRecognizer.state == Gestures.RecognizerState.Moved)
				{
					float relativeCameraPosY = Math.Abs(cameraPos.y) / maxHeight;
					float factor = lerp(relativeCameraPosY, 0.1f, 5.0f);

					if (zoomGestureLastScale - gestureRecognizer.scale > 0)
						moveToCameraPos.y += factor;
					else
						moveToCameraPos.y -= factor;

					zoomGestureLastScale = gestureRecognizer.scale;
				}
			};
		}

		private float lerp(float t, float a, float b)
		{
			return (1 - t) * a + t * b;
		}

		public void Rotate(vec2 rotation)
		{
			float sign = 1.0f;
			vec3 up = glm.cross(moveToCameraSide, moveToCameraDir);

			if (up.y < 0)
			{
				sign = -1.0f;
			}

			moveToCameraDir = glm.angleAxis(glm.radians(rotation.x), sign * new vec3(0.0f, 1.0f, 0.0f)) * moveToCameraDir;
			moveToCameraSide = glm.angleAxis(glm.radians(rotation.x), sign * new vec3(0.0f, 1.0f, 0.0f)) * moveToCameraSide;
			moveToCameraDir = glm.angleAxis(glm.radians(rotation.y), moveToCameraSide) * moveToCameraDir;
		}

		override public void Update()
		{
			cameraPos += (moveToCameraPos - cameraPos) * changeSpeed;
			cameraSide += (moveToCameraSide - cameraSide) * changeSpeed;
			cameraDir += (moveToCameraDir - cameraDir) * changeSpeed;

			if (glm.length(moveToCameraPos - cameraPos) < Double.Epsilon)
			{
				cameraIsMoving = false;
			}
			else {
				cameraIsMoving = true;
			}

			float offset = 0.0f;
			float scew = 0.0f;
			float side = glm.tan(fov * (float)Math.PI / 360.0f) * (near);
			vec3 pos = cameraPos + offset * cameraSide;

			ProjectionMatrix = glm.frustum(-side + scew, side + scew, -(float)viewPortHeight / (float)viewPortWidth * side, (float)viewPortHeight / (float)viewPortWidth * side, near, far);
			ModelMatrix = glm.lookAt(pos, pos + cameraDir, glm.cross(cameraSide, cameraDir));
			ProjectionModelMatrix = ProjectionMatrix * ModelMatrix;
		}

		public override void StopMovement()
		{
			moveToCameraPos = cameraPos;
			moveToCameraDir = cameraDir;
			moveToCameraSide = cameraSide;
		}
	}
}