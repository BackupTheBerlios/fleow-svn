using System;
using Tao.OpenGl;

using gl=Tao.OpenGl.Gl;
using glu=Tao.OpenGl.Glu;

namespace Banshee.Plugins.Fleow
{
	//Camera View Manipulation Class (static members)
	public static class Cam
	{
		static float eye_x = 0;
		static float eye_y = 1.69f;
		static float eye_z = 3.62f;
		static float a = 0;
		static float b = 25;
		static float r = 4;

		public static void Offset(float da, float db)
		{
			SetPos(a-da*0.01f,b+db*0.01f,r);
		}

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

		public static void SetView()
		{
			glu.gluLookAt(eye_x,eye_y,eye_z,0,0,0,0,1,0);
		}
	}
}
