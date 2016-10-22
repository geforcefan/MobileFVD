using System;
using System.Collections.Generic;
using OpenTK.Graphics.ES30;
using GlmNet;
using System.Linq;

namespace FVD.Renderer
{
	public class TrackSectionPicker : Core.VertexObject
	{
		public Model.Track track;
		private bool isInit = false;
		public bool drawBoundingBox = false;

		List<uint> indices;
		Core.ShaderProgram Shader;

		public TrackSectionPicker()
		{
			Shader = new Core.ShaderProgram("TrackSectionPickerVertex", "TrackSectionPickerFragment");
		}

		public void InitMesh(int node)
		{
			List<VertexTypes.TrackSectionPickerVertex> vertices = new List<VertexTypes.TrackSectionPickerVertex>();
			indices = new List<uint>();

			for (int i = 0; i < track.sections.Count; i++)
			{
				Model.Section section = track.sections[i];

				int r = (i & 0x000000FF) >> 0;
				int g = (i & 0x0000FF00) >> 8;
				int b = (i & 0x00FF0000) >> 16;

				for (var j = 0; j < section.heavyChangedNodes.Count; j++)
				{
					Model.MNode mnode = section.nodes[section.heavyChangedNodes[j]];

					VertexTypes.TrackSectionPickerVertex[] trackVertices = {
						new VertexTypes.TrackSectionPickerVertex { Position = new vec4(mnode.getRelPos(-track.heartline + 1.0f,  0.0f, 0.0f), 1.0f) },
						new VertexTypes.TrackSectionPickerVertex { Position = new vec4(mnode.getRelPos(-track.heartline - 1.0f,  0.0f, 0.0f), 1.0f) },
						new VertexTypes.TrackSectionPickerVertex { Position = new vec4(mnode.getRelPos(-track.heartline + 0.0f,  1.0f, 0.0f), 1.0f) },
						new VertexTypes.TrackSectionPickerVertex { Position = new vec4(mnode.getRelPos(-track.heartline - 0.0f, -1.0f, 0.0f), 1.0f) },
					};

					vec4 pickingColor = new vec4(r / 255.0f, g / 255.0f, b / 255.0f, 1.0f);

					for (int k = 0; k < 4; k++)
					{
						trackVertices[k].PickingColor = pickingColor;
					}

					int index = vertices.Count;

					vertices.Add(trackVertices[0]);
					vertices.Add(trackVertices[1]);
					vertices.Add(trackVertices[2]);
					vertices.Add(trackVertices[3]);

					uint a1 = (uint)index + 0 - 0;
					uint b1 = (uint)index + 1 - 0;

					uint a2 = (uint)index + 2 - 0;
					uint b2 = (uint)index + 3 - 0;

					if (j > 0)
					{
						uint c1 = (uint)index + 0 - 4;
						uint d1 = (uint)index + 1 - 4;

						uint c2 = (uint)index + 2 - 4;
						uint d2 = (uint)index + 3 - 4;

						indices.Add(a1);
						indices.Add(b1);
						indices.Add(c1);

						indices.Add(b1);
						indices.Add(c1);
						indices.Add(d1);

						indices.Add(a2);
						indices.Add(b2);
						indices.Add(c2);

						indices.Add(b2);
						indices.Add(c2);
						indices.Add(d2);
					}
				}
			}

			SetVertexIndexData<VertexTypes.TrackSectionPickerVertex>(
				vertices.ToArray(),
				indices.ToArray(),
				VertexTypes.TrackSectionPickerVertex.GetAttributePointers(),
				System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexTypes.TrackSectionPickerVertex))
			);

			isInit = true;
		}

		public void Render(Core.ShaderProgram Shader, Camera.Camera Cam)
		{
			if (!isInit)
				return;

			Shader.Use();

			mat4 anchorBase = glm.translate(mat4.identity(), track.startPos) * glm.rotate(glm.radians(track.startYaw - 90.0f), new vec3(0.0f, 1.0f, 0.0f));

			Shader.Uniform("ProjectionMatrix", Cam.ProjectionMatrix);
			Shader.Uniform("ModelMatrix", Cam.ModelMatrix);
			Shader.Uniform("EyePos", Cam.cameraPos);
			Shader.Uniform("AnchorBase", anchorBase);

			Render();
		}

		override public void Render()
		{
			GL.BindVertexArray(vertexArrayID);

			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);
			GL.EnableVertexAttribArray(3);
			GL.EnableVertexAttribArray(4);
			GL.EnableVertexAttribArray(5);
			GL.EnableVertexAttribArray(6);
			GL.EnableVertexAttribArray(7);
			GL.DrawElements(BeginMode.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);
			GL.EnableVertexAttribArray(3);
			GL.EnableVertexAttribArray(4);
			GL.EnableVertexAttribArray(5);
			GL.EnableVertexAttribArray(6);
			GL.EnableVertexAttribArray(7);

			GL.BindVertexArray(0);
		}

		public Model.Section pickSection(Model.Track _track, vec2 point, Camera.Camera Cam)
		{
			track = _track;
			InitMesh(0);

			GL.Viewport(0, 0, (int)Cam.viewPortWidth, (int)Cam.viewPortHeight);

			GL.ClearColor(1.0f, 0, 0, 1.0f);
			GL.Clear((ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit));

			Cam.Update();

			Render(Shader, Cam);

			GL.Flush();
			GL.Finish();

			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
			byte[] buffer = new byte[4];
			GL.ReadPixels((int)point.x * 2, (int)(Cam.viewPortHeight / 2 - point.y) * 2, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, buffer);

			int pickedID =
				buffer[0] +
				buffer[1] * 256 +
				buffer[2] * 256 * 256;

			if (pickedID == 0x00ffffff || pickedID >= track.sections.Count || pickedID < 0)
			{
				return null;
			}
			else {
				return track.sections[pickedID];
			}
		}
	}
}