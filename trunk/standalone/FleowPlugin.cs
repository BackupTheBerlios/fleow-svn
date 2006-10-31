using System;

using Gtk;
using GtkSharp;

namespace Banshee.Plugins.Fleow
{
	public class Fleow
	{
		//OpenGL _Engine_ class containing all render scene functions, initalizations & bindings
		
		Engine myEngine;
		
		public static int Main (string[] argc)
		{
			Gtk.Application.Init ();
			
			new Fleow();

			// Go, dog, go!
			Gtk.Application.Run ();

			return 1;
		}
		
		public Fleow()
		{
			//Create openGL widget
			myEngine = new Engine ();
			myEngine.SetSizeRequest (600, 300);

			// Create a new Vertical Box that the Engine can live in
			VBox vb = new VBox (false, 0);
			
			// Pack the Engine widget into the VBox
			vb.PackStart (myEngine, true, true, 0);

			// Create a horizontal box that holds <, exit and > buttons
			HBox hb = new HBox (false, 0);
		
			// This button will rotate the scene left
			Button btnRotL = new Button(" < ");
			
			// Attach some event handlers to the left rotation button
			btnRotL.Pressed += myEngine.OnRotLPress;
			btnRotL.Released += myEngine.OnRotLRelease;
						
			// Pack the left rotation button into the horizontal box
			hb.PackStart (btnRotL, false, false, 0);
		
			// This is the exit button
			// This button quits the program
			Button btn = new Button ("Play");		
		
			// Bind the key press event to the OnKeyPress method
			//btn.KeyPressEvent += OnKeyPress;
			//btn.Clicked += OnUnrealized;

			hb.PackStart (btn, true, true, 0);
		
			// This button will rotate the scene right
			Button btnRotR = new Button(" > ");
			
			// Attach some event handlers to the right rotation button
			btnRotR.Pressed += myEngine.OnRotRPress;
			btnRotR.Released += myEngine.OnRotRRelease;

			// Pack the right rotation button into the horizontal box
			hb.PackStart (btnRotR, false, false, 0);
				
			// put the hbox in the vbox
			vb.PackStart (hb);
						
			// Create a new window and name it appropriately
			Window win = new Window ("fleow-standalone");
			
			// Pack the VBox into the window
			win.Add (vb);

			// Show all of win's contained widgets
			win.ShowAll ();
		}
				
	}
}
