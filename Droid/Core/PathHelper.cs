using System;
using System.IO;

namespace FVDpp.Droid
{
	static class PathHelper
	{
		public static string GetPathOfFile(String filename)
		{
			string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var path = Path.Combine(libraryPath, filename);
			return path;
		}
	}
}

