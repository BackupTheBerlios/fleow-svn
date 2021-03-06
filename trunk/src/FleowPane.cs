using System;

using Gtk;
using GtkSharp;

namespace Banshee.Plugins.Fleow
{
	/// <summary>
	/// This class is main interface element containing buttons and cover visualisation widget
	/// </summary>
	public class FleowPane : Frame
	{
		/// <summary>
		/// OpenGL Engine class containing all render scene functions, initalizations & bindings
		/// </summary>
		public Engine myEngine;
		
		/// <summary>
		/// Default class constructor
		/// </summary>
		public FleowPane()
		{
			//CreateWidget();

			// Create openGL widget
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
		
			// Bind the key press event to the OnPlayPress method
			btn.Pressed += myEngine.OnPlayPress;

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
						
			// Pack the VBox into the frame
			Add (vb);

			// Show all of win's contained widgets
			ShowAll ();
		}
				
	}
}
