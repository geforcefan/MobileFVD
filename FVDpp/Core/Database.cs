using System;
using System.IO;
using SQLite;

namespace FVD.Core
{
	static public class Database
	{
		public static SQLiteConnection currentConnection = null;

		public static SQLiteConnection OpenConnection(String dbPath)
		{
			if (Database.currentConnection == null)
			{
				Database.currentConnection = new SQLiteConnection(dbPath);
			}

			return Database.currentConnection;
		}

		public static SQLiteConnection getDB()
		{
			if (Database.currentConnection == null)
				throw new Exception("No connection is established");
			
			return Database.currentConnection;
		}
	}
}
