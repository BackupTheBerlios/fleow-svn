using System;
using Tao.OpenGl;

using gl=Tao.OpenGl.Gl;
using glu=Tao.OpenGl.Glu;

namespace Banshee.Plugins.Fleow
{
	/// <summary>
	/// Camera View Manipulation Class (static members)
	/// </summary>
	public static class Cam
	{
		static float eye_x = 0;
		static float eye_y = 1.69f;
		static float eye_z = 3.62f;
		static float a = 0;
		static float b = 25;
		static float r = 4;

		/// <summary>
		/// Pass for example mouse position offset as argument and camera will move accordingly
		/// </summary>
		/// <param name="da">First offset variable</param>
		/// <param name="db">Second offset variable</param>
		public static void Offset(float da, float db)
		{
			SetPos(a-da*0.01f,b+db*0.01f,r);
		}

		/// <summary>
		/// Sets camera position based on taken arguments (spheric cooridnates)
		/// </summary>
		/// <param name="a">First angle (horizontal)</param>
		/// <param name="b">Second angle (vertical)</param>
		/// <param name="r">Distance from the observed point</param>
		public static void SetPos(float a, float b, float r)
		{
			Cam.a=a;
			Cam.b=b;
			Cam.r=r;
			eye_y = r*(float)Math.Sin(b);
			float rp = r*(float)Math.Cos(b);
			eye_x = rp*(float)Math.Sin(a);
			eye_z = rp*(float)Math.Cos(a);
		}

		/// <summary>
		/// Applies camera settings to GL scene
		/// </summary>
		public static void SetView()
		{
			glu.gluLookAt(eye_x,eye_y,eye_z,0,0,0,0,1,0);
		}
	}
}
