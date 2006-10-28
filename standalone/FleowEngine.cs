using System;
using System.Timers;
using System.Collections;

using Gtk;
using GtkSharp;

using GtkGL;

using Tao.OpenGl;
using Tao.DevIl;

using gl=Tao.OpenGl.Gl;
using glu=Tao.OpenGl.Glu;


namespace Banshee.Plugins.Fleow
{
	//class describing cover position in 3D space
	public struct VCover
	{
		public float x;
		public float angle;
	}	
	
	//widget inherited from GLArea, therefore can be easilly added as widget in gtk apps
	public class Engine : GLArea
	{
		/*	attrList
		 *      Specifies a list of Boolean attributes and enum/integer
		 *      attribute/value pairs. The last attribute must be zero or
		 *      _GDK_GL_CONFIGS.None.
		 *      
		 *      See glXChooseVisual man page for explanation of
		 *      attrList.
		 *      
		 *      http://www.xfree86.org/4.4.0/glXChooseVisual.3.html
		 */
		   
		static System.Int32[] attrlist = {
			(int)GtkGL._GDK_GL_CONFIGS.Rgba,
	   	(int)GtkGL._GDK_GL_CONFIGS.RedSize,1,
	   	(int)GtkGL._GDK_GL_CONFIGS.GreenSize,1,
	   	(int)GtkGL._GDK_GL_CONFIGS.BlueSize,1,
	   	(int)GtkGL._GDK_GL_CONFIGS.DepthSize,1,
			(int)GtkGL._GDK_GL_CONFIGS.Doublebuffer,
			(int)GtkGL._GDK_GL_CONFIGS.None,
	  	};

		private CoverList myCovers;		//covers gabbed from xml file
		private int current;					//currently selected cover from CoverList
		private int flank_cap = 3;			//storage capability (one flank), total = 3 + 1 + 3 (2 hidden)
		private Hashtable coverhash;		//hashtable used to bind myCovers indexes with right texture ones
		private VCover[] covcor;			//cover cooridnates
		private bool motion = false;		//do we animate the scene anyhow?
		private int movdir = 0;				//movement direction -1:0:1
		private float time = 0;				//some time declaration for application animation ;)


		//class constructor
		public Engine() : base(attrlist)
		{
			// Set some event handlers
			this.ExposeEvent += OnExposed;
			this.Realized += OnRealized;
			this.Unrealized += OnUnrealized;
			this.ConfigureEvent += OnConfigure;

			//connecting with cover database
			myCovers = new CoverList("covers.xml");
			current = myCovers.Count/2;

			//creating Hashtable instance
			coverhash = new Hashtable();

			//creating coordinates
			covcor = new VCover[flank_cap*2+1];
			covcor_static();
		}

		// this method makes covcor static
		void covcor_static()
		{
			//central cover cordinates
			covcor[flank_cap].x = 0;
			covcor[flank_cap].angle = 0;

			for(int i=1;i<flank_cap;i++)
			{
				//left ones
				covcor[flank_cap-i].x = -1.0f-0.2f*i;
				covcor[flank_cap-i].angle = 60;
				//right ones
				covcor[flank_cap+i].x = 1.0f+0.2f*i;
				covcor[flank_cap+i].angle = -60f;
			}
		}

		//moves cover from centre to left|right
		private bool covcor_animate()
		{
			time += movdir*0.1f;
			
			// phase 1 - moving center cover to left||right flank
			if(Math.Abs(time)<=0.5f) 
			{
				//simple position time dependency sin(PI*t)
				covcor[flank_cap].x=-1.0f*(float)Math.Sin(time*Math.PI);
				covcor[flank_cap].angle=60*(float)Math.Sin(time*Math.PI);				
			}
			//phase 2 - moving flanks
			else if(Math.Abs(time)<=1.0f)
			{
				for(int i=1;i<2*flank_cap;i++)
					covcor[i].x -= movdir*0.04f;	// distance step = "distance" divided by "how many time steps";
			}
			//phase 3 - centring a new cover
			else if(Math.Abs(time)<=1.5f)
			{
				//simple position time dependency cos(PI*t)
				covcor[flank_cap+(int)movdir].x=movdir*1.0f-1.0f*(float)Math.Sin((time-1.0f)*Math.PI);
				covcor[flank_cap+(int)movdir].angle=-movdir*60+60*(float)Math.Sin((time-1.0f)*Math.PI);
			}
			//stop animating
			else if(Math.Abs(time)>=1.6f)
			{
				//must load & unload some covers here
				time = 0;

				int to_add = reminder(current+(int)movdir*(flank_cap+1),myCovers.Count);
				int to_del = reminder(current-(int)movdir*(flank_cap),myCovers.Count);
				//load new texture
				coverhash.Add(to_add, Ilut.ilutGLLoadImage(myCovers.item[to_add].image));
				//unload unnecessary one and remove from hash table
				gl.glDeleteTextures(1, new int[] { (int)coverhash[to_del] } ); 
				coverhash.Remove(to_del);
				
				//changes focused cover
				current+=(int)movdir;
				//normalize to prevent out of index case :)
				current=current%myCovers.Count;
				
				covcor_static();
				motion = false;
			}
			
			if( this.MakeCurrent() == 0) return true;
			
			this.QueueDraw();
			
			return motion;
		}
		
		// This method is called "ReSizeGLScene" in the NeHe lessons
		void OnConfigure (object o, EventArgs e)
		{
			if( this.MakeCurrent() == 0) return;
			
			int height = this.Allocation.Height,
				width  = this.Allocation.Width;
				
			if (height==0)									// Prevent A Divide By Zero By
			{
				height=1;									// Making Height Equal One
			}
				
			gl.glViewport (0, 0, width, height);
			
			gl.glMatrixMode(gl.GL_PROJECTION);				// Select The Projection Matrix
			gl.glLoadIdentity();							// Reset The Projection Matrix

			// Calculate The Aspect Ratio Of The Window
			glu.gluPerspective(45.0f,(float)width/(float)height,0.1f,100.0f);

			gl.glMatrixMode(gl.GL_MODELVIEW);				// Select The Modelview Matrix
			gl.glLoadIdentity();		
		}

		bool InitGL()
		{
			if (this.MakeCurrent() == 0)
				return false;

			gl.glShadeModel(gl.GL_SMOOTH);						// Enables Smooth Shading
			gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);			// Black Background
			gl.glClearDepth(1.0f);								// Depth Buffer Setup
			gl.glEnable(gl.GL_DEPTH_TEST);						// Enables Depth Testing
			gl.glDepthFunc(gl.GL_LEQUAL);						// The Type Of Depth Test To Do
			// Really Nice Perspective Calculations
			gl.glHint(gl.GL_PERSPECTIVE_CORRECTION_HINT, gl.GL_NICEST);	
			
			return true;
		}
		
		// The correct time to init the gl window is at Realize time
		void OnRealized (object o, EventArgs e)
		{
			if(!InitGL())
				Console.WriteLine("Couldn't InitGL()");

			else LoadTextures();
		}

		// This method is called "DrawGLScene" in NeHe tutorials	
		void OnExposed (object o, EventArgs e)
		{
		
			if (this.MakeCurrent() == 0)
				return;
			
			// Clear The Screen And The Depth Buffer
			gl.glClear(gl.GL_COLOR_BUFFER_BIT | gl.GL_DEPTH_BUFFER_BIT);

			for(int i=1;i<flank_cap*2;i++)
				DrawCover(i);
			
			this.SwapBuffers ();	// Show the newly displayed contents
		}

		void DrawCover (int index)
		{
			//Draw Cover wireframe + texture
			gl.glLoadIdentity();
			gl.glTranslatef(covcor[index].x,0.0f,-4.0f);
			gl.glRotatef(covcor[index].angle,0.0f,1.0f,0.0f);

			gl.glBindTexture(gl.GL_TEXTURE_2D, (int)coverhash[reminder(current+index-flank_cap,myCovers.Count)]);
			gl.glBegin(gl.GL_QUADS);

				gl.glTexCoord2f(-1, 1); gl.glVertex3f( -1.0f, -1.0f, 0.0f);
				gl.glTexCoord2f(0, 1); gl.glVertex3f( 1.0f, -1.0f, 0.0f);
				gl.glTexCoord2f(0, 0); gl.glVertex3f( 1.0f, 1.0f, 0.0f);
				gl.glTexCoord2f(-1, 0); gl.glVertex3f( -1.0f, 1.0f, 0.0f);
			
			gl.glEnd();
		}
		
		void OnUnrealized (object o, EventArgs e)
		{
			Application.Quit();
		}

		private void LoadTextures()
		{

			if(flank_cap!=0 && myCovers.Count!=0)
			{
				//devil initializations ];>
				Il.ilInit();
				Ilut.ilutInit();
				
				//here comes hashing, it took me a while to understand what i did
				//call (int)coverhash[cover_index] to get approprioate index of loaded texture
				coverhash.Add(current, Ilut.ilutGLLoadImage(myCovers.item[current].image));
				for(int i=1;i<flank_cap+1;i++)
				{
					coverhash.Add(current+i, Ilut.ilutGLLoadImage(myCovers.item[current+i].image));
					coverhash.Add(current-i, Ilut.ilutGLLoadImage(myCovers.item[current-i].image));
				}
			}
		}

		public void OnRotRPress (object o, System.EventArgs e)
		{
			if(time==0)
			{
				motion = true;
				movdir = -1;
				
				GLib.Timeout.Add (50, new GLib.TimeoutHandler (this.covcor_animate));
			}
		}

		public void OnRotLRelease (object o, EventArgs e)
		{
			//Console.WriteLine("Halting Left Rotation!");
			//doRotate = false;
		}

		public void OnRotLPress (object o, System.EventArgs e)
		{
			if(time==0)
			{
				motion = true;
				movdir = 1;
				
				GLib.Timeout.Add (50, new GLib.TimeoutHandler (this.covcor_animate));
			}
		}

		public void OnRotRRelease (object o, EventArgs e)
		{
			//Console.WriteLine("Halting Right Rotation!");
			//doRotate = false;
		}

		//used instead of C# % modulo op as it gives negative values
		int reminder(int a, int b)
		{
			a = a%b;
			if(a<0)return a+b;
			return a;
		}


	}
}
