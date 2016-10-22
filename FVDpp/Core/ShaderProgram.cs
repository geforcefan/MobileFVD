using System;
using OpenTK.Graphics.ES20;
using System.IO;
using OpenTK;
using System.Reflection;

namespace FVD.Core
{
	public class ShaderProgram
	{
		public uint ProgramHandle { get; private set; }

		public ShaderProgram(String VertexName, String FragmentName)
		{
			uint vertexShader = CompileShader(VertexName, ShaderType.VertexShader);
			uint fragmentShader = CompileShader(FragmentName, ShaderType.FragmentShader);

			ProgramHandle = (uint)GL.CreateProgram();

			if (ProgramHandle == 0)
				throw new InvalidOperationException("Unable to create program");

			GL.AttachShader(ProgramHandle, vertexShader);
			GL.AttachShader(ProgramHandle, fragmentShader);

			GL.BindAttribLocation(ProgramHandle, 0, "vPosition");
			GL.LinkProgram(ProgramHandle);
		}

		public void Use()
		{
			GL.UseProgram(ProgramHandle);
		}

		public static uint CompileShader(string shaderName, ShaderType shaderType)
		{
			string prefix;

#if __IOS__
			prefix = "FVDpp.iOS.Shaders.";
#endif
#if __ANDROID__
			prefix = "FVDpp.Droid.Shaders.";
#endif

			var assembly = typeof(App).GetTypeInfo().Assembly;

			Stream stream = assembly.GetManifestResourceStream(prefix + shaderName + ".glsl");

			string shaderString;

			using (var reader = new StreamReader(stream))
			{
				shaderString = reader.ReadToEnd();
			}

			uint shaderHandle = (uint)GL.CreateShader(shaderType);
			GL.ShaderSource((int)shaderHandle, shaderString);
			GL.CompileShader(shaderHandle);

			Console.WriteLine("Compile Result of " + prefix + shaderName + ".glsl: " + GL.GetShaderInfoLog((int)shaderHandle));

			return shaderHandle;
		}

		public void UniformMatrix4(int location, Matrix4 value)
		{
			GL.UniformMatrix4(location, 1, false, ref value.Row0.X);
		}

		public void Uniform(String name, int val)
		{
			GL.Uniform1(GL.GetUniformLocation(ProgramHandle, name), val);
		}

		public void Uniform(String name, bool val)
		{
			GL.Uniform1(GL.GetUniformLocation(ProgramHandle, name), (val?1:0));
		}

		public void Uniform(String name, float x, float y, float z)
		{
			GL.Uniform3(GL.GetUniformLocation(ProgramHandle, name), x, y, z);
		}

		public void Uniform(String name, float x)
		{
			GL.Uniform1(GL.GetUniformLocation(ProgramHandle, name), x);
		}

		public void Uniform(String name, GlmNet.mat4 value)
		{
			GL.UniformMatrix4(GL.GetUniformLocation(ProgramHandle, name), 1, false, value.to_array());
		}

		public void Uniform(String name, GlmNet.vec3 value)
		{
			GL.Uniform3(GL.GetUniformLocation(ProgramHandle, name), value.x, value.y, value.z);
		}
	}
}

