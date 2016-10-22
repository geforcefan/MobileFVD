using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;
using GlmNet;

namespace FVD.Renderer
{
	public class Sky : Core.VertexObject
	{
		public Core.Texture SkyTexture;

		public Sky()
		{
			List<VertexTypes.SkyVertex> vertices = new List<VertexTypes.SkyVertex>() {
				new VertexTypes.SkyVertex { Position = new vec4( 1.0f, 1.0f, 0.9999f, 1.0f) },
				new VertexTypes.SkyVertex { Position = new vec4( 1.0f,-1.0f, 0.9999f, 1.0f) },
				new VertexTypes.SkyVertex { Position = new vec4(-1.0f, 1.0f, 0.9999f, 1.0f) },
				new VertexTypes.SkyVertex { Position = new vec4(-1.0f,-1.0f, 0.9999f, 1.0f) }
			};

			SetVertexData<VertexTypes.SkyVertex>(
				vertices,
				VertexTypes.SkyVertex.GetAttributePointers(),
				System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexTypes.SkyVertex))
			);

			SkyTexture = new Core.Texture(
				"Textures/negx.jpg",
				"Textures/negy.jpg",
				"Textures/negz.jpg",
				"Textures/posx.jpg",
				"Textures/posy.jpg",
				"Textures/posz.jpg"
			);
		}

		public void Render(Core.ShaderProgram Shader, Camera.Camera Cam)
		{
			Shader.Use();
			mat4 inverse = glm.inverse(Cam.ProjectionModelMatrix);

			vec4 topLeft = inverse * new vec4(-1.0f, 1.0f, 1.0f, 1.0f);

			topLeft /= topLeft.w;
			topLeft = new vec4(glm.normalize(new vec3(topLeft) - Cam.cameraPos), 0);
			topLeft.x = -topLeft.x;
			topLeft.y = -topLeft.y;
			topLeft.z = -topLeft.z;
			topLeft.w = -topLeft.w;

			vec4 topRight = inverse * new vec4(1.0f, 1.0f, 1.0f, 1.0f);
			topRight /= topRight.w;
			topRight = new vec4(glm.normalize(new vec3(topRight) - Cam.cameraPos), 0);
			topRight.x = -topRight.x;
			topRight.y = -topRight.y;
			topRight.z = -topRight.z;
			topRight.w = -topRight.w;

			vec4 bottomLeft = inverse * new vec4(-1.0f, -1.0f, 1.0f, 1.0f);
			bottomLeft /= bottomLeft.w;
			bottomLeft = new vec4(glm.normalize(new vec3(bottomLeft) - Cam.cameraPos), 0);
			bottomLeft.x = -bottomLeft.x;
			bottomLeft.y = -bottomLeft.y;
			bottomLeft.z = -bottomLeft.z;
			bottomLeft.w = -bottomLeft.w;

			vec4 bottomRight = inverse * new vec4(1.0f, -1.0f, 1.0f, 1.0f);
			bottomRight /= bottomRight.w;
			bottomRight = new vec4(glm.normalize(new vec3(bottomRight) - Cam.cameraPos), 0);
			bottomRight.x = -bottomRight.x;
			bottomRight.y = -bottomRight.y;
			bottomRight.z = -bottomRight.z;
			bottomRight.w = -bottomRight.w;

			Shader.Uniform("TopLeft", topLeft.x, topLeft.y, topLeft.z);
			Shader.Uniform("TopRight", topRight.x, topRight.y, topRight.z);
			Shader.Uniform("BottomLeft", bottomLeft.x, bottomLeft.y, bottomLeft.z);
			Shader.Uniform("BottomRight", bottomRight.x, bottomRight.y, bottomRight.z);
			Shader.Uniform("SkyTexture", SkyTexture.getID());

			Render();
		}

		override public void Render()
		{
			GL.BindVertexArray(vertexArrayID);

			GL.EnableVertexAttribArray(0);
			GL.DrawArrays(BeginMode.TriangleStrip, 0, 4);
			GL.DisableVertexAttribArray(0);

			GL.BindVertexArray(0);
		}
	}
}

