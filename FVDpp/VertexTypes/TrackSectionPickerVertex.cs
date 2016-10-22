using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;

namespace FVD.VertexTypes
{
	public struct TrackSectionPickerVertex
	{
		public GlmNet.vec4 Position;
		public GlmNet.vec4 PickingColor;

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
