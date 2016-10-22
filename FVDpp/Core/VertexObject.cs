using System;
using OpenTK.Graphics.ES30;
using System.Collections.Generic;

namespace FVD.Core
{
	public abstract class VertexObject
	{
		protected int vertexArrayID;

		private int vertexBuffer, indexBuffer;

		protected void SetVertexIndexData<T>(
			List<T> vertices,
			uint[] indices,
			List<VertexTypes.AttributePointers> attributePointers,
			int vertexSize
			) where T : struct
		{
			GL.GenVertexArrays(1, out vertexArrayID);
			GL.BindVertexArray(vertexArrayID);

			GL.GenBuffers(1, out vertexBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexSize * vertices.Count), vertices.ToArray(), BufferUsage.StaticDraw);

			GL.GenBuffers(1, out indexBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsage.StaticDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

			int currentOffset = 0;
			foreach (VertexTypes.AttributePointers attributePointer in attributePointers)
			{
				GL.EnableVertexAttribArray(attributePointer.Location);
				GL.VertexAttribPointer(
					attributePointer.Location,
					attributePointer.Size,
					attributePointer.Type,
					false,
					vertexSize,
					currentOffset
				);
				GL.DisableVertexAttribArray(attributePointer.Location);

				currentOffset += attributePointer.TypeSize * attributePointer.Size;
			}
		}

		protected void SetVertexIndexData<T>(
			T[] vertices,
			uint[] indices,
			List<VertexTypes.AttributePointers> attributePointers,
			int vertexSize
			) where T : struct
		{
			GL.GenVertexArrays(1, out vertexArrayID);
			GL.BindVertexArray(vertexArrayID);

			GL.GenBuffers(1, out vertexBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexSize * vertices.Length), vertices, BufferUsage.StaticDraw);

			GL.GenBuffers(1, out indexBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsage.StaticDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

			int currentOffset = 0;
			foreach (VertexTypes.AttributePointers attributePointer in attributePointers)
			{
				GL.EnableVertexAttribArray(attributePointer.Location);
				GL.VertexAttribPointer(
					attributePointer.Location,
					attributePointer.Size,
					attributePointer.Type,
					false,
					vertexSize,
					currentOffset
				);
				GL.DisableVertexAttribArray(attributePointer.Location);

				currentOffset += attributePointer.TypeSize * attributePointer.Size;
			}
		}

		protected void SetVertexData<T>(
			List<T> vertices,
			List<VertexTypes.AttributePointers> attributePointers,
			int vertexSize
			) where T: struct
		{
			GL.GenVertexArrays(1, out vertexArrayID);
			GL.BindVertexArray(vertexArrayID);

			GL.GenBuffers(1, out vertexBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexSize * vertices.Count), vertices.ToArray(), BufferUsage.StaticDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

			int currentOffset = 0;
			foreach (VertexTypes.AttributePointers attributePointer in attributePointers)
			{
				GL.EnableVertexAttribArray(attributePointer.Location);
				GL.VertexAttribPointer(
					attributePointer.Location,
					attributePointer.Size,
					attributePointer.Type,
					false,
					vertexSize,
					currentOffset
				);
				GL.DisableVertexAttribArray(attributePointer.Location);

				currentOffset += attributePointer.TypeSize * attributePointer.Size;
			}
		}

		abstract public void Render();
	}
}

