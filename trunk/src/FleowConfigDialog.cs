using System;
using Gtk;
using Gdk;
//using GConf;
using Mono.Unix;


namespace Banshee.Plugins.Fleow 
{ 
	/// <summary>
	/// This class provides config interface of fleow plugin
	/// </summary>
    public class FleowConfigPage : VBox 
	{
		/// <summary>
		/// CheckButton for cover animation support
		/// </summary> 
		private CheckButton animation;
		/// <summary>
		/// CheckButton for lights support
		/// </summary> 
		private CheckButton lights;

		/// <summary>
		/// Reference to parent plugin
		/// </summary> 
		private FleowPlugin plugin;

		/// <summary>
		/// Default constructor
		/// </summary> 
		/// <param name="plugin">A parent that conf dialog will referee to</param>
		public FleowConfigPage(FleowPlugin plugin) : base() 
		{
			this.plugin = plugin;
			BuildWidget();
		}

  		/// <summary>
		/// Builds all the widgets and creates the layout
		/// </summary>
		private void BuildWidget()
		{
			Spacing = 10;
         
			VBox box = new VBox();
			box.Spacing = 5;

			animation = new CheckButton("Disable animations");
			lights = new CheckButton("Lights");

			lights.Toggled += lightstoggled;

			box.PackStart(animation, false, false, 0);
			box.PackStart(lights, false, false, 0);
			PackStart(box, false, false, 0);
            
			ShowAll();
		}

   	/// <summary>
		/// Event triggered when the check button is toggled
		/// </summary>
		private void lightstoggled(object o, EventArgs args) 
		{
			if (((ToggleButton) o).Active)
				Lights.On();
			else
				Lights.Off();
		}


    }
}
