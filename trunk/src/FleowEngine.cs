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
	//Camera class and its static members for view manipulation
	public class Cam
	{
		static float cam_zdistance = -4.0f;
		static float cam_ydistance = -1.23f;
		static float cam_angle = 18;

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
	
	//openGL data of a cover
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

		public void SetPos(float x, float y, float z, float angle)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.angle = angle;
		}
	}

	//inheriting Coverlist and adding all three dimensional stuff
	public class GLCoverList : CoverList
	{
		public GLCoverList() : base()
		{
			current = Count/2;
			AlignToGrid();
		}

		public void AlignToGrid()
		{
			for(int i=0;i<Count;i++)
			{
				if(i<current)
					item(i).SetPos(-1.2f-0.2f*(current-i),0,-0.5f,60f);
				else
					item(i).SetPos(1.2f+.2f*(i-current),0,-0.5f,-60f);
			}

			//center
			item(current).SetPos(0,0,0,0);
		}

		public void Diagnoze()
		{
			Console.Write("Fleow Diagnoze\n");
			for(int i=0;i<Count;i++)
				Console.Write(item(i).image+":"+item(i).texture+":"+item(i).x+":"+item(i).y+":"+item(i).z+":"+item(i).angle+"\n");
		}
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

		public GLCoverList myCovers;		//covers gabbed from banshee database

		//class constructor
		public Engine() : base(attrlist)
		{
			// Set some event handlers
			this.ExposeEvent += OnExposed;
			this.Realized += OnRealized;
			this.Unrealized += OnUnrealized;
			this.ConfigureEvent += OnConfigure;
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
			gl.glLoadIdentity();
			
			// Camera set view
			Cam.SetView();

			if(myCovers!=null)
			{
				for(int i=0;i<myCovers.Count;i++)
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

			gl.glBindTexture(gl.GL_TEXTURE_2D, cover.texture);
			gl.glBegin(gl.GL_QUADS);

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

			myCovers = new GLCoverList();
		}

		// --------------------------------------------------------------- //

		void OnUnrealized (object o, EventArgs e)
		{
			Application.Quit();
		}

		// --------------------------------------------------------------- //

		public void OnRotRPress (object o, System.EventArgs e)
		{
			if((myCovers.current+1)<myCovers.Count)
			{
				myCovers.current++;
				myCovers.AlignToGrid();
		
				this.QueueDraw();
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
				myCovers.current--;
				myCovers.AlignToGrid();

				this.QueueDraw();
			}		
		}

		public void OnRotRRelease (object o, EventArgs e)
		{
			//Console.WriteLine("Halting Right Rotation!");
			//doRotate = false;
		}

		public void MoveToCover (string artist, string albumtitle)
		{
			myCovers.current = myCovers.Search(artist,albumtitle);
			myCovers.AlignToGrid();
			this.QueueDraw();
		}

	}
}
