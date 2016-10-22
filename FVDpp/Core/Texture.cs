using System;
using OpenTK.Graphics.ES20;
using System.Collections.Generic;

#if __ANDROID__
using Android.Graphics;
#else 
using Foundation;
using CoreGraphics;
using UIKit;
#endif

namespace FVD.Core
{
	public class Texture
	{
		private int mId;
		private uint handle;
		private int iType;

		static private List<bool> usedIDs = null;

		public Texture(String filename, int mode)
		{
			mId = Texture.getFreeID();
			Texture.usedIDs[mId] = true;
			GL.ActiveTexture(TextureUnit.Texture0 + mId);

			GL.GenTextures(1, out handle);
			GL.BindTexture(TextureTarget.Texture2D, handle);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);

			if (mode == 0)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
			}
			else if (mode == 1)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
			}
			else {
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.MirroredRepeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.MirroredRepeat);
			}

			Texture.getAndBindImage(filename, TextureTarget.Texture2D);

			GL.GenerateMipmap(TextureTarget.Texture2D);

			iType = 0;
		}

		public Texture(byte[] imageData, int width, int height, int mode)
		{
			mId = Texture.getFreeID();
			Texture.usedIDs[mId] = true;
			GL.ActiveTexture(TextureUnit.Texture0 + mId);

			GL.GenTextures(1, out handle);
			GL.BindTexture(TextureTarget.Texture2D, handle);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);

			if (mode == 0)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
			}
			else if (mode == 1)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
			}
			else {
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.MirroredRepeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.MirroredRepeat);
			}

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)width, (int)height, 0, OpenTK.Graphics.ES20.PixelFormat.Rgba, PixelType.UnsignedByte, imageData);

			GL.GenerateMipmap(TextureTarget.Texture2D);

			iType = 0;
		}	

		public Texture(String negX, String negY, String negZ, String posX, String posY, String posZ)
		{
			mId = Texture.getFreeID();
			Texture.usedIDs[mId] = true;
			GL.ActiveTexture(TextureUnit.Texture0 + mId);

			GL.GenTextures(1, out handle);
			GL.BindTexture(TextureTarget.TextureCubeMap, handle);

			Texture.getAndBindImage(negZ, TextureTarget.TextureCubeMapPositiveX);
			Texture.getAndBindImage(posZ, TextureTarget.TextureCubeMapNegativeX);

			Texture.getAndBindImage(negY, TextureTarget.TextureCubeMapPositiveY);
			Texture.getAndBindImage(posY, TextureTarget.TextureCubeMapNegativeY);

			Texture.getAndBindImage(posX, TextureTarget.TextureCubeMapPositiveZ);
			Texture.getAndBindImage(negX, TextureTarget.TextureCubeMapNegativeZ);

			GL.GenerateMipmap(TextureTarget.TextureCubeMap);

			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);

			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
			//GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);

			iType = 2;
		}

#if __ANDROID__
		public static void getAndBindImage(String filename, TextureTarget target)
		{
		}
#else 
		public static void getAndBindImage(String filename, TextureTarget target)
		{
			string path = NSBundle.MainBundle.ResourcePath + "/" + filename;

			NSData texData = NSData.FromFile(path);

			UIImage image = UIImage.LoadFromData(texData);
			if (image == null)
				throw new Exception("Texture '" + filename + "' could not found");

			nint width = image.CGImage.Width;
			nint height = image.CGImage.Height;

			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
			byte[] imageData = new byte[height * width * 4];
			CGContext context = new CGBitmapContext(imageData, width, height, 8, 4 * width, colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);

			context.TranslateCTM(0, height);
			context.ScaleCTM(1, -1);

			colorSpace.Dispose();

			context.ClearRect(new CGRect(0, 0, width, height));
			context.DrawImage(new CGRect(0, 0, width, height), image.CGImage);

			GL.TexImage2D(target, 0, PixelInternalFormat.Rgba, (int)width, (int)height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageData);

			context.Dispose();
		}
#endif

		public int getID()
		{
			return mId;
		}

		public uint getHandle()
		{
			return handle;
		}

		public static int getFreeID()
		{
			for (int i = 0; i < Texture.usedIDs.Count; i++)
			{
				if (!Texture.usedIDs[i]) return i;
			}
			return -1;
		}

		public static void init()
		{
			if (Texture.usedIDs == null)
			{
				int maxIDs;
				GL.GetInteger(GetPName.MaxCombinedTextureImageUnits, out maxIDs);

				Texture.usedIDs = new List<bool>();

				for (int i = 0; i < maxIDs; i++)
				{
					Texture.usedIDs.Add(false);
				}
			}
		}
	}
}
