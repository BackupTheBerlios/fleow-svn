using System;
using System.Collections;
using System.Data;
using Mono.Unix;

namespace Banshee.Plugins.Fleow
{
	//contains info of a cover
	public class Cover : GLCover
	{
		public string artist;
		public string albumtitle;
		public string image;

		public Cover(){}

		public Cover(string image, string artist, string albumtitle)
		{
				//image = System.Environment.GetEnvironmentVariable("HOME") + "/.gnome2/banshee/covers/" + ASIN + ".jpg";
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
			IDataReader reader = Banshee.Base.Globals.Library.Db.Query("SELECT DISTINCT Artist,AlbumTitle FROM Tracks ORDER BY Artist");
        	while(reader.Read()) 
			{				
				string artist=(string)reader["Artist"];
				string album=(string)reader["AlbumTitle"];
				string image_path="";

				IDataReader imgreader = Banshee.Base.Globals.Library.Db.Query("SELECT DISTINCT ASIN FROM Tracks WHERE Artist LIKE \""+artist+"\" AND AlbumTitle LIKE \""+album+"\"");
				while(imgreader.Read())
				{
					image_path = System.Environment.GetEnvironmentVariable("HOME") + "/.gnome2/banshee/covers/" + (string)imgreader["ASIN"] + ".jpg";
					if((new UnixFileInfo(image_path)).Exists)break;
				}


				if((new UnixFileInfo(image_path)).Exists)
				{
					Add(new Cover(image_path,artist,album));
				}
				else
				{
					string directory=(string)Banshee.Base.Globals.Library.Db.QuerySingle("SELECT Uri FROM Tracks WHERE Artist LIKE \""+artist+"\" AND AlbumTitle LIKE \""+album+"\" LIMIT 1");
					//remove (replace safer) "file://"
					directory=directory.Remove(0,7);
					//replace "%20" with white spaces
					directory=directory.Replace("%20"," ");
					directory = (new UnixFileInfo(directory)).DirectoryName;
				
					//load local cover.jpg or folder.jpg
					if((new UnixFileInfo(directory+"/cover.jpg")).Exists)
					{
						Add(new Cover(directory+"/cover.jpg",artist,album));
					}
					else if((new UnixFileInfo(directory+"/folder.jpg")).Exists)
					{
						Add(new Cover(directory+"/folder.jpg",artist,album));
					}
					
				}
			}	
		}

		public static int GetTrackId(Cover c)
        {
            object result = Banshee.Base.Globals.Library.Db.QuerySingle("SELECT TrackId FROM Tracks WHERE Artist LIKE \""+c.artist+"\" AND AlbumTitle LIKE \""+c.albumtitle+"\" LIMIT 1");
            return result == null ? -1 : (int)result;
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
