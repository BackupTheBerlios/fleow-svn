using System;
using Tao.OpenGl;
using Tao.DevIl;

using gl=Tao.OpenGl.Gl;
using glu=Tao.OpenGl.Glu;

namespace Banshee.Plugins.Fleow
{
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
}
