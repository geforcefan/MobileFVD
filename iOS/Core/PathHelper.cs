using System;
using System.IO;

namespace FVDpp.iOS
{
	static class PathHelper
	{
		public static string GetPathOfFile(String filename)
		{
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder

				string libraryPath = Path.Combine(documentsPath, "..", "Library");
				var path = Path.Combine(libraryPath, filename);
				return path;
			}
	}
}

