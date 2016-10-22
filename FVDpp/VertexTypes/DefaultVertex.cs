using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;

namespace FVD.VertexTypes
{
	public struct DefaultVertex
	{
		public GlmNet.vec4 Position;
		public GlmNet.vec4 Color;
		public GlmNet.vec4 Normal;
		public GlmNet.vec2 UV;

		static public List<AttributePointers> GetAttributePointers()
		{
			return new List<AttributePointers>()
			{
				new AttributePointers { Location=0, Size=4, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=1, Size=4, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=2, Size=4, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float},
				new AttributePointers { Location=3, Size=2, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float}
			};
		}
	}
}

