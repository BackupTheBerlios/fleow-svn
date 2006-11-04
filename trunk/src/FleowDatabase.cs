using System;
using System.Collections;
using System.Data;

namespace Banshee.Plugins.Fleow
{
	//contains info of a cover
	public class Cover : GLCover
	{
		public string artist;
		public string albumtitle;
		public string image;

		public Cover(){}

		public Cover(string ASIN, string artist, string albumtitle)
		{
				image = System.Environment.GetEnvironmentVariable("HOME") + "/.gnome2/banshee/covers/" + ASIN + ".jpg";
				LoadTexture(image);
				this.artist = artist;
				this.albumtitle = albumtitle;
		}
	}
	
	//container for covers, grabs info from sql banshee database
	public class CoverList : ArrayList
	{
		public int current;					//currently selected cover from CoverList

		public CoverList()
		{
			IDataReader reader = Banshee.Base.Globals.Library.Db.Query("SELECT DISTINCT ASIN,Artist,AlbumTitle FROM Tracks ORDER BY Artist");
        	while(reader.Read()) 
			{
				string tmp=(string)reader["ASIN"];
				if(tmp!="NOTFOUND" && !string.IsNullOrEmpty(tmp) )
				{
					Add(new Cover(tmp,(string)reader["Artist"],(string)reader["AlbumTitle"]));
				}
			}	
		}

		public Cover item(int index)
		{
			return (Cover)this[index];
		}

		public int Search(string artist,string albumtitle)
		{
			for(int i=0;i<Count;i++)
				if(item(i).artist==artist && item(i).albumtitle==albumtitle) return i;
			return current;
		}
	}

}
