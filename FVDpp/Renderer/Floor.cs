using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;
using GlmNet;

namespace FVD.Renderer
{
	public class Floor : Core.VertexObject
	{
		public int numPlanes = 300;
		public int planeSize = 10;
		public int planeSubDivision = 1;

		FloorSegments mainSegments = new FloorSegments();
		FloorSegments subSegments = new FloorSegments();
		FloorGround ground = new FloorGround();

		public Floor()
		{
			int totalSize = numPlanes * planeSubDivision;
			float halfSize = totalSize / 2.0f;

			int verticesIndex = 0;

			vec4 mainColor = new vec4(0.7f, 0.7f, 0.7f, 1.0f);
			vec4 subColor = new vec4(0.95f, 0.95f, 0.95f, 1.0f);
			vec4 groundColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);

			for (int z = 0; z <= numPlanes; z++)
			{
				for (int x = 0; x <= numPlanes; x++)
				{
					VertexTypes.FloorVertex vertex = new VertexTypes.FloorVertex()
					{
						Position = new vec4(halfSize - (x * planeSubDivision), 0.0f, halfSize - (z * planeSubDivision), 1.0f)
					};

					vertex.Color = mainColor;
					mainSegments.vertices.Add(vertex);

					vertex.Color = subColor;
					subSegments.vertices.Add(vertex);

					vertex.Color = groundColor;
					vertex.Position.y -= 0.01f;
					ground.vertices.Add(vertex);

					if (x > 0 && z > 0)
					{
						uint a = (uint)(verticesIndex - 1 - (numPlanes + 1));
						uint b = (uint)(verticesIndex - 1);
						uint c = (uint)(verticesIndex);
						uint d = (uint)(verticesIndex - (numPlanes + 1));

						ground.indices.Add(a);
						ground.indices.Add(b);
						ground.indices.Add(c);

						ground.indices.Add(a);
						ground.indices.Add(c);
						ground.indices.Add(d);
					}

					if (x > 0)
					{
						if (z % planeSize != 0)
						{
							subSegments.indices.Add((uint)verticesIndex - 0);
							subSegments.indices.Add((uint)verticesIndex - 1);
						}
						else {
							mainSegments.indices.Add((uint)verticesIndex - 0);
							mainSegments.indices.Add((uint)verticesIndex - 1);
						}
					}

					if (z > 0)
					{	
						if (x % planeSize != 0)
						{
							subSegments.indices.Add((uint)verticesIndex - 0);
							subSegments.indices.Add((uint)(verticesIndex - 0 - (numPlanes + 1)));
						}
						else {
							mainSegments.indices.Add((uint)verticesIndex - 0);
							mainSegments.indices.Add((uint)(verticesIndex - 0 - (numPlanes + 1)));
						}
					}

					verticesIndex++;
				}
			}

			mainSegments.lineWidth = 2;
			mainSegments.Build();

			subSegments.Build();
			ground.Build();
		}

		public void Render(Core.ShaderProgram Shader, Camera.Camera Cam)
		{
			Shader.Use();
			Shader.Uniform("ProjectionMatrix", Cam.ProjectionMatrix);
			Shader.Uniform("ModelMatrix", Cam.ModelMatrix);
			Shader.Uniform("EyePos", Cam.cameraPos);

			mainSegments.Render();
			subSegments.Render();
			ground.Render();
		}

		override public void Render() {}

		class FloorSegments : Core.VertexObject
		{
			public List<VertexTypes.FloorVertex> vertices = new List<VertexTypes.FloorVertex>();
			public List<uint> indices = new List<uint>();

			public int lineWidth = 1;

			public void Build()
			{
				SetVertexIndexData<VertexTypes.FloorVertex>(
					vertices,
					indices.ToArray(),
					VertexTypes.FloorVertex.GetAttributePointers(),
					System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexTypes.FloorVertex))
				);
			}

			override public void Render()
			{
				GL.BindVertexArray(vertexArrayID);

				GL.LineWidth(lineWidth);

				GL.EnableVertexAttribArray(0);
				GL.EnableVertexAttribArray(1);
				GL.DrawElements(BeginMode.Lines, indices.Count, DrawElementsType.UnsignedInt, 0);
				GL.DisableVertexAttribArray(1);
				GL.DisableVertexAttribArray(0);

				GL.BindVertexArray(0);
			}
		}

		class FloorGround : Core.VertexObject
		{
			public List<VertexTypes.FloorVertex> vertices = new List<VertexTypes.FloorVertex>();
			public List<uint> indices = new List<uint>();

			public int lineWidth = 1;

			public void Build()
			{
				SetVertexIndexData<VertexTypes.FloorVertex>(
					vertices,
					indices.ToArray(),
					VertexTypes.FloorVertex.GetAttributePointers(),
					System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexTypes.FloorVertex))
				);
			}

			override public void Render()
			{
				GL.BindVertexArray(vertexArrayID);

				GL.LineWidth(lineWidth);

				GL.EnableVertexAttribArray(0);
				GL.EnableVertexAttribArray(1);
				GL.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
				GL.DisableVertexAttribArray(1);
				GL.DisableVertexAttribArray(0);

				GL.BindVertexArray(0);
			}
		}
	}
}