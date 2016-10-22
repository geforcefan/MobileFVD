using System;
using System.Linq;
using GlmNet;

namespace FVD.Model
{
	public class SectionForce : Section
	{
		public Function rollFunc;
		public Function normForce;
		public Function latForce;

		public int time;

		public SectionForce(Track _track, MNode first, float _time = 1000.0f) : base(_track, SectionType.Forced, first)
		{
			time = (int)(_time * 0.5f);
			length = 0.0f;

			argument = SectionArgument.Time;
			orientation = SectionOrientation.Quaternion;

			bSpeed = true;
			velocity = 10;

			rollFunc = new Function(0.0f, 1.0f, 0.0f, 0.0f, this, FunctionType.Roll);
			normForce = new Function(0.0f, 1.0f, nodes.First().ForceNormal, nodes.First().ForceNormal, this, FunctionType.Normal);
			latForce = new Function(0.0f, 1.0f, nodes.First().ForceLateral, nodes.First().ForceLateral, this, FunctionType.Lateral);

			rollFunc.OnFunctionChanged += normForce.OnFunctionChanged += latForce.OnFunctionChanged += (Function obj) => {
				if (OnSectionChanged != null)
					OnSectionChanged(this);
			};
		}

		public override int updateSection(int node = 0)
		{
			Console.WriteLine("Called updateSection Force " + node);

			Console.WriteLine("nodes[0]" + nodes[0].Pos);

    		if(rollFunc.lockedFunction != -1)
			{
				if (glm.abs(rollFunc.subFunctions.Last().symArg) > 0.00001f && rollFunc.subFunctions.Last().minArgument * Core.Misc.F_HZ < node)
					node = (int)(Core.Misc.F_HZ * rollFunc.subFunctions.Last().minArgument - 1.5f);
			}
			if (normForce.lockedFunction != -1)
			{
				if (glm.abs(normForce.subFunctions.Last().symArg) > 0.00001f && normForce.subFunctions.Last().minArgument * Core.Misc.F_HZ < node) 
					node = (int)(Core.Misc.F_HZ * normForce.subFunctions.Last().minArgument - 1.5f);
			}
			if (latForce.lockedFunction != -1)
			{
				if (glm.abs(latForce.subFunctions.Last().symArg) > 0.00001f && latForce.subFunctions.Last().minArgument * Core.Misc.F_HZ < node) 
					node = (int)(Core.Misc.F_HZ * latForce.subFunctions.Last().minArgument - 1.5f);
			}

			if (argument == SectionArgument.Distance)
			{
				//return updateDistanceSection(node);
			}

    		node = node > nodes.Count - 2 ? nodes.Count - 2 : node;
			node = node < 0 ? 0 : node;

			int numNodes = (int)(getMaxArgument() * Core.Misc.F_HZ + 0.5);
			time = numNodes;

			if (node >= nodes.Count - 1 && node > 0) node = nodes.Count - 2;

			if (nodes.Count > 1 && track.sections.ElementAt(track.sections.Count - 1) != this)
			{
				nodes.Remove(nodes.Last()); // disjoint this section from the next one
			}

			if (node == 0)
			{
				nodes[0].updateNorm();

				float diff = nodes[0].ForceNormal;

				if (float.IsNaN(diff))
				{
					nodes.Add(nodes[0]);
					return node;
				}

				normForce.subFunctions.ElementAt(0).translateValues(diff);
				normForce.translateValues(normForce.subFunctions.ElementAt(0));

				diff = nodes[0].ForceLateral;

				if (float.IsNaN(diff))
				{
					nodes.Add(nodes[0]);
					return node;
				}
				latForce.subFunctions.ElementAt(0).translateValues(diff);
				latForce.translateValues(latForce.subFunctions.ElementAt(0));

				diff = nodes[0].RollSpeed;
				if (orientation == SectionOrientation.Quaternion)
				{
					diff += glm.dot(nodes[0].Dir, new vec3(0.0f, 1.0f, 0.0f)) * nodes[0].getYawChange();
				}
				rollFunc.subFunctions.ElementAt(0).translateValues(diff);
				rollFunc.translateValues(rollFunc.subFunctions.ElementAt(0));
			}

			float curLength = 0.0f;

			heavyChangedNodes.Clear();
			heavyChangedNodes.Add(0);

			Console.WriteLine("From Node: " + node + " To Node : " + numNodes);

			int i;
			for (i = node; i < numNodes; i++)
			{
				if (i >= nodes.Count - 1)
				{
					nodes.Add(nodes[i]);
				}

				MNode prevNode = nodes[i];
				MNode curNode = nodes[i + 1];
				curNode.Pos = prevNode.Pos;
				curNode.Velocity = prevNode.Velocity;
				curNode.Energy = prevNode.Energy;

				vec3 forceVec = -normForce.getValue((i + 1) / Core.Misc.F_HZ) * prevNode.Norm - latForce.getValue((i + 1) / Core.Misc.F_HZ) * prevNode.Lat - new vec3(0.0f, 1.0f, 0.0f);

				curNode.ForceNormal = normForce.getValue((i + 1) / Core.Misc.F_HZ);
				curNode.ForceLateral = latForce.getValue((i + 1) / Core.Misc.F_HZ);

				float nForce = -glm.dot(forceVec, glm.normalize(prevNode.Norm)) * Core.Misc.F_G;
				float lForce = -glm.dot(forceVec, glm.normalize(prevNode.Lat)) * Core.Misc.F_G;

				float estVel = glm.abs(prevNode.HeartDistFromLast) < Double.Epsilon ? prevNode.Velocity : prevNode.HeartDistFromLast * Core.Misc.F_HZ;

				curNode.Dir = glm.normalize(glm.angleAxis(nForce / Core.Misc.F_HZ / estVel, prevNode.Lat) * glm.angleAxis(-lForce / prevNode.Velocity / Core.Misc.F_HZ, prevNode.Norm) * prevNode.Dir);
				curNode.Lat = glm.normalize(glm.angleAxis(-lForce / prevNode.Velocity / Core.Misc.F_HZ, prevNode.Norm) * prevNode.Lat);

				curNode.updateNorm();

				curNode.Pos += curNode.Dir * (curNode.Velocity / (2.0f * Core.Misc.F_HZ)) + prevNode.Dir * (curNode.Velocity / (2.0f * Core.Misc.F_HZ)) + (prevNode.getPosHeart(track.heartline) - curNode.getPosHeart(track.heartline));

				curNode.RollSpeed = 0.0f;
				curNode.setRoll(rollFunc.getValue((i + 1) / Core.Misc.F_HZ) / Core.Misc.F_HZ);

				curNode = calcDirFromLast(curNode, i + 1);

				if (orientation == SectionOrientation.Euler || rollFunc.getSubfunction((i + 1) / Core.Misc.F_HZ).degree == FunctionDegree.ToZero)
				{
					curNode.setRoll(glm.dot(curNode.Dir, new vec3(0.0f, -1.0f, 0.0f)) * curNode.YawFromLast);
					curNode.RollSpeed += glm.dot(curNode.Dir, new vec3(0.0f, -1.0f, 0.0f)) * curNode.YawFromLast * Core.Misc.F_HZ;
				}

				curNode.updateNorm();

				curNode.DistFromLast = glm.distance(curNode.getPosHeart(track.heartline), prevNode.getPosHeart(track.heartline));
				curNode.TotalLength = prevNode.TotalLength + curNode.DistFromLast;
				curNode.HeartDistFromLast = glm.distance(curNode.Pos, prevNode.Pos);
				curNode.TotalHeartLength = prevNode.TotalHeartLength + curNode.HeartDistFromLast;
				curNode.RollSpeed += rollFunc.getValue((i + 1) / Core.Misc.F_HZ);

				curNode = calcDirFromLast(curNode, i + 1);

				float temp = glm.cos(glm.abs(curNode.getPitch()) * (float) Math.PI / 180.0f);
				float forceAngle = glm.sqrt(temp * temp * curNode.YawFromLast * curNode.YawFromLast + curNode.PitchFromLast * curNode.PitchFromLast);//deltaAngle;
				curNode.AngleFromLast = forceAngle;

				if (bSpeed)
				{
					curNode.Energy -= (curNode.Velocity * curNode.Velocity * curNode.Velocity / Core.Misc.F_HZ * track.resistance);
					curNode.Velocity = glm.sqrt(2.0f * (curNode.Energy - Core.Misc.F_G * (curNode.getPosHeart(track.heartline * 0.9f).y + curNode.TotalLength * track.friction)));
				}
				else {
					curNode.Velocity = velocity;
					curNode.Energy = 0.5f * velocity * velocity + Core.Misc.F_G * (curNode.getPosHeart(track.heartline * 0.9f).y + curNode.TotalLength * track.friction);
				}

				if (glm.abs(curNode.AngleFromLast) < Double.Epsilon)
				{
					forceVec = new vec3(0.0f, 1.0f, 0.0f);
				}
				else {
					float normalDAngle = (float) Math.PI / 180.0f * (-curNode.PitchFromLast * glm.cos(curNode.Roll * (float) Math.PI / 180.0f) - temp * curNode.YawFromLast * glm.sin(curNode.Roll * (float) Math.PI / 180.0f));
					float lateralDAngle = (float) Math.PI / 180.0f * (curNode.PitchFromLast * glm.sin(curNode.Roll * (float) Math.PI / 180.0f) - temp * curNode.YawFromLast * glm.cos(curNode.Roll * (float) Math.PI / 180.0f));

					forceVec = new vec3(0.0f, 1.0f, 0.0f) + lateralDAngle * curNode.Velocity * Core.Misc.F_HZ / Core.Misc.F_G * curNode.Lat + normalDAngle * curNode.HeartDistFromLast * Core.Misc.F_HZ * Core.Misc.F_HZ / Core.Misc.F_G * curNode.Norm;
				}
				curNode.ForceNormal = -glm.dot(forceVec, glm.normalize(curNode.Norm));
				curNode.ForceLateral = -glm.dot(forceVec, glm.normalize(curNode.Lat));

				nodes[i + 1] = curNode;

				if (curLength > 5.0f)
				{
					heavyChangedNodes.Add(i + 1);
					curLength = 0.0f;
				}

				curLength += curNode.DistFromLast;
			}

			while (nodes.Count > 1 + i)
			{
				nodes.RemoveAt(1 + i);
			}

			heavyChangedNodes.Add(nodes.Count - 1);

			if (nodes.Count > 0)
			{
				length = nodes.Last().TotalLength - nodes.First().TotalLength;
			}
			else {
				length = 0;
			}

			return node;
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
