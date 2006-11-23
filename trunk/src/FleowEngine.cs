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

using Banshee.Base;

namespace Banshee.Plugins.Fleow
{
	//Camera View Manipulation Class (static members)
	public static class Cam
	{
		static float cam_zdistance = -4.0f;
		static float cam_ydistance = -1.23f;
		static float cam_angle = 18;

		public static void Offset(float xang, float yang)
		{
			SetPos(cam_angle+yang,cam_zdistance);
		}

		public static void SetPos(float _cam_angle,float _cam_distance)
		{
			cam_angle = _cam_angle;
			cam_zdistance = _cam_distance;
			cam_ydistance = (float)Math.Sin(cam_angle*0.0174)*cam_zdistance;
		}

		public static void SetView()
		{
			gl.glRotatef(cam_angle,1.0f,0.0f,0.0f);
			gl.glTranslatef(0.0f,cam_ydistance,cam_zdistance);
		}
	}

	//Scene Lights
	public static class Lights
	{
		static float[] LightAmbient= { 0.2f, 0.2f, 0.2f, 1.0f };
		static float[] LightDiffuse= { 1.0f, 1.0f, 1.0f, 1.0f }; 
		static float[] LightPosition= { 0.0f, 0.0f, 0.75f, 1.0f };

		static public void On()
		{
			gl.glLightfv(gl.GL_LIGHT1, gl.GL_AMBIENT, LightAmbient);	
			gl.glLightfv(gl.GL_LIGHT1, gl.GL_DIFFUSE, LightDiffuse);
			gl.glLightfv(gl.GL_LIGHT1, gl.GL_POSITION,LightPosition);
			gl.glEnable(gl.GL_LIGHT1);
			gl.glEnable(gl.GL_LIGHTING);
		}

	}
	
	//OpenGL Cover Data Class
	public class GLCover
	{
		public float x,y,z;
		public float angle;
		public int texture;
		public bool hidden;

		public void LoadTexture(string filename)
		{
			texture = Ilut.ilutGLLoadImage(filename);
		}

		public void UnloadTexture()
		{
			if(texture!=-1)
			{
				gl.glDeleteTextures(1, new int[] { (int)texture } ); 
				texture = -1;
			}
		}

		~GLCover()
		{
			UnloadTexture();
		}

		public void SetPos(float x, float y, float z, float angle)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.angle = angle;
		}
	}

	//OpenGL Cover List Class
	public class GLCoverList : CoverList
	{
		int target = 0;			// target index
		public int offset = 0;		// target index
		static float c = 1.2f;		// center distance
		static float d = 0.2f;		// cover  distance
		static float angle = 60;	// default angle
		static float depth = 0.5f;	// default depth
		static int spf = 25;		// steps per flip
		static int step = 0;		// step

		public GLCoverList() : base()
		{
			current = Count/2;
			AlignToGrid();
		}

		public void MakeNewTarget(int offset)
		{
			target = current + offset;
			if(target<0) target = 0;
			else if(target>=Count) target = (Count-1);
			this.offset = target - current;
		}

		public void AlignToGrid()
		{
			for(int i=0;i<Count;i++)
			{
				if(i<current)
					item(i).SetPos(-c-d*(current-i),0,-depth,angle);
				else
					item(i).SetPos(c+d*(i-current),0,-depth,-angle);
			}

			item(current).SetPos(0,0,depth,0);
		}

		//postitions cover depending on animation step, should be followed by MakeNewTarget
		public bool AlignByStep()
		{
			if((step/spf)!=1)
			{
				for(int i=0;i<Count;i++)
					NewPos(item(i));
				step++;
			}
			else
			{
				current = target;
				AlignToGrid();
				step=0;
				return false;
			}
			return true;
		}

		public void NewPos(GLCover cov)
		{
			int speed = Math.Abs(offset);
			float dir = -Math.Sign(offset);
			float move = speed*dir*d/(float)spf;
			float mov_angle = 0;
			float mov_z = 0;
			if(Math.Abs(cov.x)<(c+d))
			{
				move *= (c+d)/d;
				mov_angle = -speed*dir*angle/(float)spf;
				mov_z = -2*speed*depth/(float)spf;
				if(cov.x*dir<0)mov_z=-mov_z;
			}
			cov.x += move;
			cov.z += mov_z;
			cov.angle += mov_angle;
		}
	}
	
	//Class inherited from GLArea, therefore can be easilly added as widget in gtk apps
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

		public GLCoverList myCovers;		//covers gabbed from banshee database
		double beginX = 0;
		double beginY = 0;
		bool button1Pressed = false;

		//class constructor
		public Engine() : base(attrlist)
		{
			this.Events |=
	    		Gdk.EventMask.Button1MotionMask |
		    	Gdk.EventMask.Button2MotionMask |
	    		Gdk.EventMask.ButtonPressMask |
	    		Gdk.EventMask.ButtonReleaseMask |
	    		Gdk.EventMask.VisibilityNotifyMask |
	    		Gdk.EventMask.PointerMotionMask |
	    		Gdk.EventMask.PointerMotionHintMask ;

			// Set some event handlers
			this.ExposeEvent += OnExposed;
			this.Realized += OnRealized;
			this.Unrealized += OnUnrealized;
			this.ConfigureEvent += OnConfigure;

			this.ButtonPressEvent += OnButtonPress;
			this.ButtonReleaseEvent += OnButtonRelease;
			this.MotionNotifyEvent += OnMotionNotify;
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
			gl.glEnable(gl.GL_POLYGON_SMOOTH);					// Enables Antialiasing for polygons
			gl.glDepthFunc(gl.GL_LEQUAL);						// The Type Of Depth Test To Do
			// Really Nice Perspective Calculations
			gl.glHint(gl.GL_PERSPECTIVE_CORRECTION_HINT, gl.GL_NICEST);
			gl.glHint(gl.GL_POLYGON_SMOOTH_HINT, gl.GL_NICEST);
	
			//Lights.On();										// Lights on

			//gl.glEnable(gl.GL_BLEND);
			//gl.glDisable(gl.GL_DEPTH_TEST);
			//gl.glBlendFunc(gl.GL_SRC_COLOR,gl.GL_ONE_MINUS_SRC_COLOR);						// Set The Blending Function For Translucency
			
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
		
			if (this.MakeCurrent() == 0) return;
			
			// Clear The Screen And The Depth Buffer
			gl.glClear(gl.GL_COLOR_BUFFER_BIT | gl.GL_DEPTH_BUFFER_BIT);
			gl.glLoadIdentity();
			
			// Camera set view
			Cam.SetView();

			if(myCovers!=null)
			{
				for(int i=0;i<myCovers.current;i++)
					DrawCover(myCovers.item(i));
				for(int i=myCovers.Count-1;i>=myCovers.current;i--)
					DrawCover(myCovers.item(i));
			}
			
			this.SwapBuffers ();	// Show the newly displayed contents
		}

		void DrawCover (Cover cover)
		{
			//Draw Cover wireframe + texture
			gl.glPushMatrix();
			gl.glTranslatef(cover.x,cover.y,cover.z);
			gl.glRotatef(cover.angle,0.0f,1.0f,0.0f);

			float br = 1;
			if(Math.Abs(cover.x)>2.0f)
			{
				br = (float)Math.Exp(-4*(Math.Abs(cover.x)-2.0f));
			}
			gl.glColor3f(br, br, br);							// Manipulate Brightness
			gl.glBindTexture(gl.GL_TEXTURE_2D, cover.texture);
			gl.glBegin(gl.GL_QUADS);

				gl.glNormal3f( 0.0f, 0.0f, 1.0f);
				gl.glTexCoord2f(-1, 1); gl.glVertex3f( -1.0f, -1.0f, 0.0f);
				gl.glTexCoord2f(0, 1); gl.glVertex3f( 1.0f, -1.0f, 0.0f);
				gl.glTexCoord2f(0, 0); gl.glVertex3f( 1.0f, 1.0f, 0.0f);
				gl.glTexCoord2f(-1, 0); gl.glVertex3f( -1.0f, 1.0f, 0.0f);
			
			gl.glEnd();
			gl.glPopMatrix();
		}

		private void LoadTextures()
		{
			//devil initializations ];>
			Il.ilInit();
			Ilut.ilutInit();

			myCovers = new GLCoverList();	//this needs probably backgrounding+optimization (as takes some time to create)
		}

		// --------------------------------------------------------------- //

		void OnUnrealized (object o, EventArgs e)
		{
			Application.Quit();
		}

		// --------------------------------------------------------------- //

		private bool Flip()
		{
			bool ret = myCovers.AlignByStep();
			this.QueueDraw();
			return ret;
		}

		public void OnRotRPress (object o, System.EventArgs e)
		{
			if((myCovers.current+1)<myCovers.Count)
			{
				myCovers.MakeNewTarget(1);
				GLib.Timeout.Add (10, new GLib.TimeoutHandler (this.Flip));
			}
		}

		public void OnRotLRelease (object o, EventArgs e)
		{
			//Console.WriteLine("Halting Left Rotation!");
			//doRotate = false;
		}

		public void OnRotLPress (object o, System.EventArgs e)
		{
			if(myCovers.current>0)
			{
				myCovers.MakeNewTarget(-1);
				GLib.Timeout.Add (10, new GLib.TimeoutHandler (this.Flip));
			}		
		}

		public void OnRotRRelease (object o, EventArgs e)
		{
			//Console.WriteLine("Halting Right Rotation!");
			//doRotate = false;
		}

		public void OnPlayPress (object o, EventArgs e)
		{
			int track = CoverList.GetTrackId(myCovers.item(myCovers.current));
			if(track>0)PlayerEngineCore.OpenPlay(Globals.Library.GetTrack(track));
		}

		public void MoveToCover (string artist, string albumtitle)
		{
			int offset = myCovers.Search(artist,albumtitle) - myCovers.current;
			if(offset!=0)
			{
				myCovers.MakeNewTarget(offset);
				//uint period = (Math.Abs(offset)>5) ? 2 : (uint)Math.Abs(10/offset);
				GLib.Timeout.Add (10, new GLib.TimeoutHandler (this.Flip));
			}
		}

		// Mouse Events
		public void OnMotionNotify (object o, Gtk.MotionNotifyEventArgs e)
		{
			int ix, iy;
			double x, y;
			Gdk.ModifierType m;
			
			// Find the current mouse X and Y positions
			if (e.Event.IsHint) 
			{
				e.Event.Window.GetPointer(out ix, out iy, out m);
				x = (double)ix;
				y = (double)iy;
			} 
			else 
			{
	    		x = e.Event.X;
	    		y = e.Event.Y;
	  		}
			if(button1Pressed)
			{
				Cam.Offset((float)(x-beginX),(float)(y-beginY));
				this.QueueDraw();
			}
			beginX = x;
			beginY = y;
			
		}

		void OnButtonPress (object o, Gtk.ButtonPressEventArgs e)
		{
			if(e.Event.Button == 1)
			{
				button1Pressed = true;
				
				/* potential beginning of drag, reset mouse position */
				beginX = e.Event.X;
				beginY = e.Event.Y;
				return;
			}
		}

		void OnButtonRelease (object o, Gtk.ButtonReleaseEventArgs e)
		{
			if(e.Event.Button == 1)
			{
				button1Pressed = false;
			}
		}
		// End of Mouse Events
	}
}
