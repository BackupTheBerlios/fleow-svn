using System;
using Mono.Unix;
 
using Banshee.Base;
using Banshee.Sources;
using Banshee.MediaEngine;
 
namespace Banshee.Plugins.Fleow
{
	public class FleowPlugin : Banshee.Plugins.Plugin
	{
		protected override string ConfigurationName { get { return "Fleow"; } }
		public override string DisplayName { get { return "Fleow"; } }

		public override string Description {
		get
			{
				return Catalog.GetString(
					"A Banshee plugin letting you to view, manage &" +
					" select albums from the library in the 3D mode."
				);
			}
		}
        
		public override string [] Authors 
		{
			get
			{
				return new string []{
					"Lukasz Wisniewski <vishna@gazeta.pl>",
					"Dariusz Lisowski"
				};
			}
		}
 
		// --------------------------------------------------------------- //
        
		private FleowPane fleow_pane;
       
		protected override void PluginInitialize()
		{
			Console.WriteLine("Initializing Fleow Plugin");
			PlayerEngineCore.EventChanged += OnPlayerEngineEventChanged;	
		}
        
		protected override void PluginDispose()
		{
			Console.WriteLine("Disposing Fleow Plugin");
			PlayerEngineCore.EventChanged -= OnPlayerEngineEventChanged;
		}

		// --------------------------------------------------------------- //

		private void InstallInterfaceElements()
		{
			fleow_pane = new FleowPane();
			InterfaceElements.MainContainer.PackEnd(fleow_pane, false, false, 0);
		}

		// --------------------------------------------------------------- //

		private void OnPlayerEngineEventChanged (object o, PlayerEngineEventArgs args)
		{
			lock(this) 
			{
				if(fleow_pane == null)
				{
					InstallInterfaceElements();
				}
			}

			switch (args.Event)
			{
				case PlayerEngineEvent.StartOfStream:
					fleow_pane.myEngine.MoveToCover(PlayerEngineCore.CurrentTrack.DisplayArtist,PlayerEngineCore.CurrentTrack.DisplayAlbum);
				break;
			}
		}
	}

}
