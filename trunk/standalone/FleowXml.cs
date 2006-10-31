using System;
using System.Xml;

namespace Banshee.Plugins.Fleow
{
	//contains info of a cover
	public class Cover
	{
		public string artist;
		public string title;
		public string image;

		public Cover(){}

		public Cover(XmlNode xnode)
		{
				XmlAttributeCollection xAttr = xnode.Attributes;

				//xAttr[0].Value defines if cover was grabbed by banshee or was set locally
				if(xAttr[0].Value == "Banshee")
				{
					image = System.Environment.GetEnvironmentVariable("HOME") + "/.gnome2/banshee/covers/" + xnode.SelectNodes("image")[0].InnerText + ".jpg";
				}

				artist = xnode.SelectNodes("artist")[0].InnerText;
				title = xnode.SelectNodes("title")[0].InnerText;

		}
	}
	
	//container for covers, grabs info from xml source
	public class CoverList
	{
		public int Count;
		public Cover[] item;

		public CoverList(string filename)
		{
			//loading xml file and selecting right nodes
			XmlDocument xDoc = new XmlDocument();
			xDoc.Load(filename);
			XmlNodeList album = xDoc.GetElementsByTagName("album");

			Count = album.Count;
			item = new Cover[Count];
			
			//insert all albums from xml file to linkedlist
			for(int i=0;i<Count;i++)
			{
				item[i] = new Cover(album[i]);
				
				//In the future some sorting methods should be added
				//somwhere around, most probably here during loading
				//just to have xml file organized later on.
				
			}
		}
	}

}
