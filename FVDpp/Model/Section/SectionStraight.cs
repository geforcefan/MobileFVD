using System;
using System.Linq;
using GlmNet;

namespace FVD.Model
{
	public class SectionStraight : Section
	{
		public float HeartLineLength { set; get; } = 0.0f;
		public Function rollFunc;

		public SectionStraight(Track _track, MNode first, float length = 10.0f) : base(_track, SectionType.Straight, first)
		{
			HeartLineLength = length;

			argument = SectionArgument.Time;
			orientation = SectionOrientation.Quaternion;

			bSpeed = false;
			velocity = 10;

			rollFunc = new Function(0.0f, length, 0.0f, 0.0f, this, FunctionType.Roll);

			rollFunc.OnFunctionChanged += (Function obj) =>
			{
				if (OnSectionChanged != null)
					OnSectionChanged(this);
			};
		}

		public void changeLength(float length)
		{
			HeartLineLength = length;
			updateSection();
		}

		public override int updateSection(int node = 0)
		{
			Console.WriteLine("Called updateSection Straight " + node);

    		int numNodes = 1;
			length = 0.0f;

			HeartLineLength = getMaxArgument();

    		while(nodes.Count > 1)
			{
				nodes.RemoveAt(1);
			}

			nodes[0].updateNorm();

			float diff = nodes[0].RollSpeed;
			rollFunc.subFunctions.First().translateValues(diff);
			rollFunc.translateValues(rollFunc.subFunctions.First());

			bool lastNode = false;

			float fCurLength = 0.0f;

			float curLength = 0.0f;

			heavyChangedNodes.Clear();
			heavyChangedNodes.Add(0);

			while (fCurLength < HeartLineLength - Double.Epsilon && !lastNode)
			{
				nodes.Add(nodes.Last());

				float dTime;
				MNode prevNode = nodes[numNodes - 1];
				MNode curNode = nodes[numNodes];

				if (curNode.Velocity < 0.1f)
				{
					break;
				}
				if (curNode.Velocity / Core.Misc.F_HZ < HeartLineLength - fCurLength)
				{
					dTime = Core.Misc.F_HZ;
				}
				else {
					lastNode = true;
					dTime = (curNode.Velocity + (float)Double.Epsilon) / (HeartLineLength - fCurLength);
				}

				curNode.Pos += curNode.Dir * (curNode.Velocity / dTime);

				fCurLength += curNode.Velocity / dTime;

				curNode.setRoll(rollFunc.getValue(fCurLength) / dTime);

				curNode.ForceNormal = -curNode.Norm.y;
				curNode.ForceLateral = -curNode.Lat.y;

				curNode.DistFromLast = glm.distance(curNode.getPosHeart(track.heartline), prevNode.getPosHeart(track.heartline));
				curNode.TotalLength += curNode.DistFromLast;
				curNode.HeartDistFromLast = glm.distance(curNode.Pos, prevNode.Pos);
				curNode.TotalHeartLength += curNode.HeartDistFromLast;

				curNode.RollSpeed = rollFunc.getValue(fCurLength);

				curNode = calcDirFromLast(curNode, numNodes);

				curNode.AngleFromLast = 0.0f;
				curNode.DirFromLast = 0.0f;
				curNode.YawFromLast = 0.0f;
				curNode.PitchFromLast = 0.0f;

				if (glm.abs(curNode.RollSpeed) < 0.001f)
				{
					curNode.TrackAngleFromLast = 0.0f;
				}

				if (bSpeed)
				{
					curNode.Energy -= (curNode.Velocity * curNode.Velocity * curNode.Velocity / Core.Misc.F_HZ * track.resistance);
					curNode.Velocity = glm.sqrt(2.0f * (curNode.Energy - Core.Misc.F_G * (curNode.getPosHeart(track.heartline * 0.9f).y + curNode.TotalLength * track.friction)));
				}
				else {
					curNode.Velocity = velocity;
					curNode.Energy = 0.5f * velocity * velocity + Core.Misc.F_G * (curNode.getPosHeart(track.heartline * 0.9f).y + curNode.TotalLength * track.friction);
				}

				nodes[numNodes] = curNode;
				length += curNode.DistFromLast;

				if (curLength > 5.0f)
				{
					heavyChangedNodes.Add(numNodes);
					curLength = 0.0f;
				}

				curLength += curNode.DistFromLast;

				++numNodes;
			}

			while (nodes.Count > numNodes)
			{
				nodes.Remove(nodes.Last());
			}

			heavyChangedNodes.Add(nodes.Count - 1);

			if (nodes.Count > 0) length = nodes.Last().TotalLength - nodes.First().TotalLength;
			else length = 0;

			return 0;
		}

		public override bool isLockable()
		{
			return false;
		}

		public override float getMaxArgument()
		{
			return rollFunc.maxArgument;
		}

		public override bool isInFunction(int index, SubFunction subFunction)
		{
			if (subFunction == null) return false;
			if (index >= nodes.Count) return false;
			float dist = nodes[index].TotalHeartLength - nodes[0].TotalHeartLength;
			if (dist >= subFunction.minArgument && dist <= subFunction.maxArgument)
			{
				return true;
			}
			return false;
		}
	}
}
