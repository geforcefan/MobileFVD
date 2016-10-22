using System;
using System.Collections.Generic;

using OpenTK.Graphics.ES30;
using GlmNet;
using System.Linq;

namespace FVD.Renderer
{
	public class Track : Core.VertexObject
	{
		public Model.Track track;
		private bool isInit = false;

		private int activeSectionIndex = -1;
		private int numberOfCreatedSections = 0;

		List<uint> indices = new List<uint>();
		List<VertexTypes.TrackVertex> vertices = new List<VertexTypes.TrackVertex>();

		public Track(Model.Track _track)
		{
			track = _track;

			track.OnUpdateTrack = (int node) =>
			{
				Console.WriteLine("Track Renderer:Triggered OnUpdateTrack");
				InitMesh(node);
			};

			track.OnSectionActivated = (Model.Section section) =>
			{
				Console.WriteLine("Track Renderer: Triggered OnSectionActivated");
				ActivateSection(section);
			};
		}

		public void ActivateSection(Model.Section section)
		{
			activeSectionIndex = track.getSectionIndex(track.activeSection);

			if (numberOfCreatedSections != track.sections.Count)
			{
				// TODO: Init Mesh from the right section, dont need to regenerate everything (from 0)
				InitMesh(0);
			}
		}

		public void InitMesh(int node)
		{
			int currentIndex = 0;
			int skipTies = 0;

			float currentLengthForRail = 0.0f;
			float currentLengthForTies = 0.0f;

			vertices.Clear();
			indices.Clear();

			numberOfCreatedSections = track.sections.Count;

			for (int i = 0; i < track.sections.Count; i++)
			{
				Model.Section section = track.sections[i];

				// Rails and ties        
				for (int j = 0; j < section.nodes.Count; j++)
				{
					Model.MNode mnode = section.nodes[j];

					VertexTypes.TrackVertex[] trackVertices = {
						new VertexTypes.TrackVertex { Position = new vec4(mnode.getRelPos(-track.heartline + -0.5f, 0.0f, 0.0f), 1.0f) },
						new VertexTypes.TrackVertex { Position = new vec4(mnode.getRelPos(-track.heartline + 0.0f, 0.5f, 0.0f), 1.0f) },
						new VertexTypes.TrackVertex { Position = new vec4(mnode.getRelPos(-track.heartline + 0.0f, -0.5f, 0.0f), 1.0f) },
						new VertexTypes.TrackVertex { Position = new vec4(mnode.Pos, 1.0f) }
					};

					for (int k = 0; k < 4; k++)
					{
						trackVertices[k].SectionIndex = i;
						trackVertices[k].Flexion = mnode.Flexion;
						trackVertices[k].RollSpeed = glm.abs(mnode.RollSpeed);
						trackVertices[k].ForceNormal = mnode.ForceNormal;
						trackVertices[k].ForceLateral = mnode.ForceLateral;
						trackVertices[k].Velocity = mnode.Velocity;
					}

					trackVertices[3].IsHeartline = 1;

					// Rails
					if ((i == 0 && j == 0) || currentLengthForRail >= 0.2f)
					{
						vertices.Add(trackVertices[0]);
						vertices.Add(trackVertices[1]);
						vertices.Add(trackVertices[2]);
						vertices.Add(trackVertices[3]);

						if (currentIndex > 0)
						{
							indices.Add(((uint)currentIndex - 4 - (uint)skipTies) + 0);
							indices.Add(((uint)currentIndex + 0));

							indices.Add(((uint)currentIndex - 4 - (uint)skipTies) + 1);
							indices.Add(((uint)currentIndex + 1));

							indices.Add(((uint)currentIndex - 4 - (uint)skipTies) + 2);
							indices.Add(((uint)currentIndex + 2));

							indices.Add(((uint)currentIndex - 4 - (uint)skipTies) + 3);
							indices.Add(((uint)currentIndex + 3));

							skipTies = 0;
						}

						currentIndex += 4;
						currentLengthForRail = 0.0f;
					}

					// Ties
					if (currentLengthForTies > 1.0f)
					{
						vertices.Add(trackVertices[0]);
						vertices.Add(trackVertices[1]);
						vertices.Add(trackVertices[2]);

						indices.Add((uint)currentIndex + 0);
						indices.Add((uint)currentIndex + 1);

						indices.Add((uint)currentIndex + 1);
						indices.Add((uint)currentIndex + 2);

						indices.Add((uint)currentIndex + 2);
						indices.Add((uint)currentIndex + 0);

						currentIndex += 3;
						skipTies += 3;

						currentLengthForTies = 0.0f;
					}

					currentLengthForRail += mnode.DistFromLast;
					currentLengthForTies += mnode.DistFromLast;
				}
			}

			SetVertexIndexData<VertexTypes.TrackVertex>(
				vertices.ToArray(),
				indices.ToArray(),
				VertexTypes.TrackVertex.GetAttributePointers(),
				System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertexTypes.TrackVertex))
			);

			isInit = true;
		}

		public void Render(Core.ShaderProgram Shader, Camera.Camera Cam)
		{
			if (!isInit)
				return;

			Shader.Use();

			Shader.Uniform("ProjectionMatrix", Cam.ProjectionMatrix);
			Shader.Uniform("ModelMatrix", Cam.ModelMatrix);
			Shader.Uniform("EyePos", Cam.cameraPos);
			Shader.Uniform("AnchorBase", track.anchorBase);
			Shader.Uniform("ActiveSection", activeSectionIndex);

			Render();
		}

		override public void Render()
		{
			GL.LineWidth(2);

			GL.BindVertexArray(vertexArrayID);

			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);
			GL.EnableVertexAttribArray(3);
			GL.EnableVertexAttribArray(4);
			GL.EnableVertexAttribArray(5);
			GL.EnableVertexAttribArray(6);
			GL.EnableVertexAttribArray(7);
			GL.DrawElements(BeginMode.Lines, indices.Count, DrawElementsType.UnsignedInt, 0);
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
	}
}