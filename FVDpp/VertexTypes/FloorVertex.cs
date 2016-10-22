using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;
using GlmNet;

namespace FVD.VertexTypes
{
	public struct FloorVertex
	{
		public vec4 Position;
		public vec4 Color;

		static public List<AttributePointers> GetAttributePointers()
		{
			return new List<AttributePointers>()
			{
				new AttributePointers { Location=0, Size=4, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=1, Size=4, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float}
			};
		}
	}
}
