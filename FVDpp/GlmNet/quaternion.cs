using System;

namespace GlmNet
{
	public struct quat
	{
        public float x;
		public float y;
		public float z;
		public float w;

        public float this[int index]
		{
			get
			{
				if (index == 0) return x;
				else if (index == 1) return y;
				else if (index == 2) return z;
				else if (index == 3) return w;
				else throw new Exception("Out of range.");
			}
			set
			{
				if (index == 0) x = value;
				else if (index == 1) y = value;
				else if (index == 2) z = value;
				else if (index == 3) w = value;
				else throw new Exception("Out of range.");
			}
		}

		public quat(float s)
		{
			x = y = z = w = s;
		}

		public quat(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public quat(vec4 v)
		{
			this.x = v.x;
			this.y = v.y;
			this.z = v.z;
			this.w = v.w;
		}

		public quat(vec3 xyz, float w)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
			this.w = w;
		}

		public static quat operator *(quat p, quat q)
		{
			return new quat(
				p.w * q.x + p.x * q.w + p.y * q.z - p.z * q.y,
				p.w * q.y + p.y * q.w + p.z * q.x - p.x * q.z,
				p.w * q.z + p.z * q.w + p.x * q.y - p.y * q.x,
				p.w * q.w - p.x * q.x - p.y * q.y - p.z * q.z
			);
		}
	}

	public static partial class glm
	{
		public static quat angleAxis(float angle, vec3 v)
		{
			quat result = new quat();

			float a = angle;
			float s = sin(a * 0.5f);

			result.w = cos(a * 0.5f);
			result.x = v.x * s;
			result.y = v.y * s;
			result.z = v.z * s;

			return result;
		}
	}
}

