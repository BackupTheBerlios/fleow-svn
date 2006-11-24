using System;
using Tao.OpenGl;

using gl=Tao.OpenGl.Gl;
using glu=Tao.OpenGl.Glu;

namespace Banshee.Plugins.Fleow
{
	//Scene Lights
	public static class Lights
	{
		static float[] LightAmbient= { 0.2f, 0.2f, 0.2f, 1.0f };
		static float[] LightDiffuse= { 1.0f, 1.0f, 1.0f, 1.0f }; 
		static float[] LightPosition= { 0.0f, 0.0f, 0.75f, 1.0f };

		static public void On()
		{
			//gl.glLightfv(gl.GL_LIGHT1, gl.GL_AMBIENT, LightAmbient);	
			gl.glLightfv(gl.GL_LIGHT1, gl.GL_DIFFUSE, LightDiffuse);
			gl.glLightfv(gl.GL_LIGHT1, gl.GL_POSITION,LightPosition);
			gl.glEnable(gl.GL_LIGHT1);
			gl.glEnable(gl.GL_LIGHTING);
		}

		static public void Off()
		{
			gl.glDisable(gl.GL_LIGHT1);
			gl.glDisable(gl.GL_LIGHTING);
		}

	}
}
