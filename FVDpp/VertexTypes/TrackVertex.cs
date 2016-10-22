using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;

namespace FVD.VertexTypes
{
	public struct TrackVertex
	{
		public GlmNet.vec4 Position;
		public float Velocity;
		public float RollSpeed;
		public float ForceNormal;
		public float ForceLateral;
		public float Flexion;
		public int SectionIndex;
		public int IsHeartline;

		static public List<AttributePointers> GetAttributePointers()
		{
			return new List<AttributePointers>()
			{
				new AttributePointers { Location=0, Size=4, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=1, Size=1, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=2, Size=1, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=3, Size=1, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=4, Size=1, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=5, Size=1, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=6, Size=1, TypeSize = sizeof(int), Type=VertexAttribPointerType.Int},
				new AttributePointers { Location=7, Size=1, TypeSize = sizeof(int), Type=VertexAttribPointerType.Int},
			};
		}
	}
}
