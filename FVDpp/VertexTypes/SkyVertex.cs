using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;

namespace FVD.VertexTypes
{
	public struct SkyVertex 
	{
		public GlmNet.vec4 Position;

		static public List<AttributePointers> GetAttributePointers()
		{
			return new List<AttributePointers>()
			{
				new AttributePointers { Location=0, Size=4, TypeSize = sizeof(float), Type=VertexAttribPointerType.Float}
			};
		}
	}
}
