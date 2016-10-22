using System;
using System.Collections.Generic;
using System.Linq;
using GlmNet;

namespace FVD.Model
{
	public abstract class Section
	{
		public List<MNode> nodes { get; set; } = new List<MNode>();
		public float length { get; set; }
		public bool bSpeed { get; set; }
		public float velocity { get; set; }

		public SectionOrientation orientation { get; set; }
		public SectionArgument argument { get; set; }

		public SectionType type { get; }

		protected Track track;

		public List<int> heavyChangedNodes = new List<int>();
		public Action<Section> OnSectionChanged;

		public Section(Track _track, SectionType _type, MNode first)
		{
			type = _type;
			track = _track;
			nodes.Add(first);

			Console.WriteLine("Num of nodes at init: " + nodes.Count);
		}

		public float speed
		{
			get
			{
				if (bSpeed)
				{
					return nodes.Last().Velocity;
				}
				else {
					return velocity;
				}
			}
		}

		public MNode calcDirFromLast(MNode node, int i)
		{
			if (i == 0 || i >= nodes.Count())
			{
				return node;
			}

			vec3 diff = nodes[i].Dir - nodes[i - 1].Dir;

			if (glm.length(diff) <= Double.Epsilon)
			{
				node.DirFromLast = 0.0f;
				node.PitchFromLast = 0.0f;
				node.YawFromLast = 0.0f;
			}
			else {
				float y = -glm.dot(diff, nodes[i - 1].Norm);
				float x = -glm.dot(diff, nodes[i - 1].Lat);
				float angle = glm.atan(x, y) * 180.0f / (float)Math.PI;

				node.DirFromLast = angle;
				node.PitchFromLast = nodes[i].getPitch() - nodes[i - 1].getPitch();
				node.YawFromLast = nodes[i].getDirection() - nodes[i - 1].getDirection();
				node.DirFromLast = glm.atan(nodes[i].YawFromLast, nodes[i].PitchFromLast) * 180.0f / (float)Math.PI - nodes[i].Roll;
			}

			vec3 curDirHeart = nodes[i].getDirHeart(track.heartline);
			vec3 prevDirHeart = nodes[i - 1].getDirHeart(track.heartline);

			float fTrackPitchFromLast = 180.0f / (float)Math.PI * (glm.asin(curDirHeart.y) - glm.asin(prevDirHeart.y));
			float fTrackYawFromLast = 180.0f / (float)Math.PI * (glm.atan(-curDirHeart.x, -curDirHeart.z) - glm.atan(-prevDirHeart.x, -prevDirHeart.z));

			float temp = glm.cos(glm.abs(glm.asin(curDirHeart.y)));
			node.TrackAngleFromLast = glm.sqrt(temp * temp * fTrackYawFromLast * fTrackYawFromLast + fTrackPitchFromLast * fTrackPitchFromLast);

			if (nodes[i].YawFromLast > 270.0f)
			{
				node.YawFromLast -= 360.0f;
			}
			else if (nodes[i].YawFromLast < -270.0f)
			{
				node.YawFromLast += 360.0f;
			}
			return node;
		}

		public abstract int updateSection(int node = 0);
		public abstract bool isLockable();
		public abstract float getMaxArgument();
		public abstract bool isInFunction(int index, SubFunction subFunction);
	}
}
