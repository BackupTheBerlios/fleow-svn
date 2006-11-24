using System;
using Gtk;
using Gdk;
//using GConf;
using Mono.Unix;


namespace Banshee.Plugins.Fleow 
{ 

    public class FleowConfigPage : VBox 
	{
		
	
		private CheckButton animation;
		private CheckButton lights;

        private FleowPlugin plugin;

        public FleowConfigPage(FleowPlugin plugin) : base() 
		{
            this.plugin = plugin;
            BuildWidget();
        }
        
		// Builds all the widgets and creates the layout
        private void BuildWidget() {
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

		// Event triggered when the check button is toggled
		private void lightstoggled(object o, EventArgs args) 
		{
			if (((ToggleButton) o).Active)
				Lights.On();
			else
				Lights.Off();
		}


    }
}

// vim:ts=4:sw=4
