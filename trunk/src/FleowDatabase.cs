using System;
using System.Collections;
using System.Data;

namespace Banshee.Plugins.Fleow
{
	//contains info of a cover
	public class Cover
	{
		public string artist;
		public string title;
		public string image;

		public Cover(){}

		public Cover(string ASIN)
		{
				image = System.Environment.GetEnvironmentVariable("HOME") + "/.gnome2/banshee/covers/" + ASIN + ".jpg";
		}
	}
	
	//container for covers, grabs info from sql banshee database
	public class CoverList : ArrayList
	{
		public CoverList()
		{
			IDataReader reader = Banshee.Base.Globals.Library.Db.Query("SELECT DISTINCT ASIN,Artist FROM Tracks ORDER BY Artist");
        	while(reader.Read()) 
			{
				Add(new Cover((string)reader["ASIN"]));
			}	
		}
	}

}
