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

			animation.Toggled += toggled;

			box.PackStart(animation, false, false, 0);
            PackStart(box, false, false, 0);
            
            ShowAll();
        }

		// Event triggered when the check button is toggled
		private void toggled(object o, EventArgs args) 
		{
			Console.WriteLine("Fleow ConfigurationWiget not yet fully implemented!");
		}


    }
}

// vim:ts=4:sw=4
