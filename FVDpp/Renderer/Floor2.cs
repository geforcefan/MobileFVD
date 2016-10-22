using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;
using GlmNet;

namespace FVD.Renderer
{
	public class Floor2 : Core.VertexObject
	{
		private float a = 500.0f;
		private float b = 100.0f;
		private int rasterImageSize = 1200;

		public bool drawBorder { get; set; } = false;
		public bool drawGrid { get; set; } = true;
		public float opacity { get; set; } = 1.0f;

		Core.Texture rasterTexture = null;
		Core.Texture floorTexture = null;

		public Floor2()
		{
			/*List<VertexTypes.FloorVertex> vertices = new List<VertexTypes.FloorVertex>() {
				new VertexTypes.FloorVertex { Position = new vec4(-a, 0.0f, +a, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-a, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-b, 0.0f, +a, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-b, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+b, 0.0f, +a, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+b, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+a, 0.0f, +a, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+a, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+a, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+b, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },

				new VertexTypes.FloorVertex { Position = new vec4(+a, 0.0f, -b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+b, 0.0f, -b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+a, 0.0f, -a, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+b, 0.0f, -a, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+b, 0.0f, -b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },

				new VertexTypes.FloorVertex { Position = new vec4(-b, 0.0f, -a, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-b, 0.0f, -b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-a, 0.0f, -a, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-a, 0.0f, -b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-a, 0.0f, -b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-b, 0.0f, -b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },

				new VertexTypes.FloorVertex { Position = new vec4(-a, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-b, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-b, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-b, 0.0f, -b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(0.0f, 0.0f, 0.0f, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+b, 0.0f, -b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(0.0f, 0.0f, 0.0f, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(+b, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(0.0f, 0.0f, 0.0f, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) },
				new VertexTypes.FloorVertex { Position = new vec4(-b, 0.0f, +b, 1.0f), Normal = new vec4(0.0f, 0.5f, 0.0f, 1.0f) }
			};*/

			/*SetVertexData<VertexTypes.FloorVertex>(
				vertices,
				VertexTypes.FloorVertex.GetAttributePointers(),
				System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexTypes.FloorVertex))
			);*/

			floorTexture = new Core.Texture("Textures/floor.png", 2);
			rasterTexture = generateRasterTexture();
		}

		private Core.Texture generateRasterTexture()
		{
			byte[] imageData = new byte[rasterImageSize * rasterImageSize * 4];

			for (int i = 0; i < rasterImageSize; i++)
			{
				for (int j = 0; j < rasterImageSize; j++)
				{
					double major = Math.Min(Math.Abs(i - rasterImageSize / 2.0), Math.Abs(j - rasterImageSize / 2.0)) / (rasterImageSize / 2.0);
					double minor = Math.Min(Math.Min(Math.Abs(((i + (rasterImageSize / 20.0)) % (rasterImageSize / 10.0) - (rasterImageSize / 20.0))), Math.Abs(((j + (rasterImageSize / 20.0)) % (rasterImageSize / 10.0) - (rasterImageSize / 20.0)))), 1.0);

					major = 1.0 - major;
					minor = 1.0 - minor;

					for (int k = 0; k < 9; ++k)
					{
						major *= major;
						minor *= minor;
					}

					major = 1.0 - major;
					minor = 1.0 - 0.5 * minor;

					major *= 255.0;
					minor *= 255.0;

					double col = Math.Min(major, minor);

					byte[] intBytes = BitConverter.GetBytes(0xff000000 + ((uint)col << 16) + ((uint)col << 8) + ((uint)col << 0));

					if (BitConverter.IsLittleEndian)
						Array.Reverse(intBytes);

					int index = (4 * (i + j * rasterImageSize)) - 1;
					imageData[++index] = intBytes[1];
					imageData[++index] = intBytes[2];
					imageData[++index] = intBytes[3];
					imageData[++index] = intBytes[0];
				}
			}
			return new Core.Texture(imageData, rasterImageSize, rasterImageSize, 2);
		}

		public void Render(Core.ShaderProgram Shader, Camera.Camera Cam)
		{
			Shader.Use();
			Shader.Uniform("ProjectionMatrix", Cam.ProjectionMatrix);
			Shader.Uniform("ModelMatrix", Cam.ModelMatrix);
			Shader.Uniform("EyePos", Cam.cameraPos);

			Shader.Uniform("Border", drawBorder);
			Shader.Uniform("Grid", drawGrid);
			Shader.Uniform("Opacity", opacity);

			//Shader.Uniform("RasterTexture", rasterTexture.getID());
			//Shader.Uniform("FloorTexture", floorTexture.getID());

			Render();
		}

		override public void Render()
		{
			GL.BindVertexArray(vertexArrayID);

			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.DrawArrays(BeginMode.TriangleStrip, 0, 31);
			GL.DisableVertexAttribArray(1);
			GL.DisableVertexAttribArray(0);

			GL.BindVertexArray(0);
		}
	}
}