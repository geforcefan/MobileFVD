using System;
using GlmNet;

namespace FVD.Camera
{
	public class POVCamera : Camera
	{
		public vec3 cameraSide = new vec3(1.0f, 0.0f, 0.0f);
		public vec3 cameraDir = new vec3(0.0f, 0.0f, -1.0f);

		private vec3 moveToCameraPos = new vec3();
		private vec3 moveToCameraDir = new vec3();
		private vec3 moveToCameraSide = new vec3();
		private vec3 headPos = new vec3(0.0f);

		private float changeSpeed = 0.03f; // USE THIS VALUE IN PRODUCTION
										   //private float changeSpeed = 1.0f;

		private int moveToNodeIndex = 0;
		private int nodeIndex = 0;

		private int numberOfNodes = 0;

		private Model.Track track = null;

		public POVCamera(Model.Track _track)
		{
			track = _track;

			cameraIsMoving = true;

			OnPanRecognized += (Gestures.PanGestureRecognizer gestureRecognizer) => {
				if (gestureRecognizer.state == Gestures.RecognizerState.Moved)
				{
					IncrementNodePosition((int)(Core.Misc.F_HZ * getRenderTimeInSeconds() * (gestureRecognizer.velocity.y / 100.0f)));
				}
			};

			OnTapRecognized += (Gestures.TapGestureRecognizer gestureRecognizer) =>
			{
				if (gestureRecognizer.state == Gestures.RecognizerState.Ended)
				{
					StopMovement();
				}
			};

			numberOfNodes = track.getNumPoints();

			track.OnUpdateTrack = (int node) =>
			{
				numberOfNodes = track.getNumPoints();
			};
		}

		public void IncrementNodePosition(int amountOfNodes)
		{
			moveToNodeIndex += amountOfNodes;
		}

		override public void Update()
		{
			nodeIndex += (int)((moveToNodeIndex - nodeIndex) * changeSpeed);

			if(nodeIndex < 0)
			{
				moveToNodeIndex = numberOfNodes - Math.Abs(moveToNodeIndex);
				nodeIndex += numberOfNodes;
			}
			if (nodeIndex > numberOfNodes)
			{
				moveToNodeIndex = moveToNodeIndex - nodeIndex;
				nodeIndex = 0;
			}

			if (moveToNodeIndex - nodeIndex == 0)
			{
				cameraIsMoving = false;
			}
			else {
				cameraIsMoving = true;
			}

			
			Model.MNode povNode = track.getPoint(nodeIndex);

			var pos = povNode.getRelPos(0.0f, 0.0f, 0.0f);
			var direction = povNode.getDirHeart(track.heartline);

			var front = direction;
			var side = povNode.getLatHeart(track.heartline);
			var down = glm.cross(front, side);

			cameraPos = new vec3(track.anchorBase * new vec4(pos, 1.0f));
			cameraDir = new vec3(track.anchorBase * new vec4(povNode.getDirHeart(track.heartline), 0.0f));

			float pScew = 0.0f;
			float pSide = glm.tan(fov * (float)Math.PI / 360.0f) * (near);

			ModelMatrix = glm.lookAt(new vec3(track.anchorBase * new vec4(pos, 1.0f)), new vec3(track.anchorBase * new vec4(pos + direction, 1.0f)), new vec3(track.anchorBase * new vec4(down, 0.0f)) * -1.0f);
			ProjectionMatrix = glm.frustum(-pSide + pScew, pSide + pScew, -(float)viewPortHeight / (float)viewPortWidth * pSide, (float)viewPortHeight / (float)viewPortWidth * pSide, near, far);
			ProjectionModelMatrix = ProjectionMatrix * ModelMatrix;
		}

		public override void StopMovement()
		{
			moveToNodeIndex = nodeIndex;
		}
	}
}