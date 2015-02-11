using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Game
{
	public class SpinObstacle
	{
		
		//Private variables.
		private 	SpriteUV[] 	spinSprite;
		
		private 	SpriteUV[] 	pivSprite;
		private 	TextureInfo	textureSpinObstacle;
		private 	TextureInfo	textureSpinPiv;
		
		private bool			stop;
		private bool			on;
		
		public float beamWidth;
		public float beamHeight;
		
		public int 	 numberOfObstacles = 3;
		
		public Vector2 GetPosition1 { get { return spinSprite[0].Position; }}
		public Vector2 GetPosition2 { get { return spinSprite[1].Position; }}
		public Vector2 GetPosition3 { get { return spinSprite[2].Position; }}
		public float GetSpringWidth { get { return beamWidth; }}
		
	
		
		
		//Public functions.
		public SpinObstacle (Scene scene, Vector2 position)
		{
			textureSpinObstacle     = new TextureInfo("/Application/textures/firebeam.png");
			textureSpinPiv     		= new TextureInfo("/Application/textures/piv.png");
			
			
			stop  = false;
			on = 	false; 
		
			pivSprite	= new SpriteUV[numberOfObstacles];
			spinSprite	= new SpriteUV[numberOfObstacles];
			
			for (int i = 0; i < numberOfObstacles; i++)
			{
				pivSprite[i]			= new SpriteUV(textureSpinPiv);	
				pivSprite[i].Quad.S 	= textureSpinPiv.TextureSizef;
				pivSprite[i].CenterSprite();
				pivSprite[i].Position = new Vector2(position.X +(i *200.0f),Director.Instance.GL.Context.GetViewport().Height*0.5f);
				
				spinSprite[i]			= new SpriteUV(textureSpinObstacle);	
				spinSprite[i].Quad.S 	= textureSpinObstacle.TextureSizef;
				spinSprite[i].CenterSprite();
				
				
				
				spinSprite[i].Position = pivSprite[i].Position;
				
				//scene.AddChild(pivSprite[i]);
				scene.AddChild(spinSprite[i]);
			}
			spinSprite[0].Rotate(-0.9f);
			spinSprite[1].Rotate(0.9f);
			spinSprite[2].Rotate(-0.9f);
		}
		
		public void Dispose()
		{
			textureSpinObstacle.Dispose();
			textureSpinPiv.Dispose();
		}
		
		public void Update(float deltaTime, float t)
		{			
			for (int i = 0; i < numberOfObstacles; i++)
			{
				pivSprite[i].Position += new Vector2(-t, 0.0f);
				spinSprite[i].Position = pivSprite[i].Position;
			}
		}

		public void Right()
			
		{
			
			spinSprite[0].Rotate(0.060f);
			spinSprite[1].Rotate(0.060f);
			spinSprite[2].Rotate(0.060f);
			
			
		}
		
		public void Stop()
			
		{
			
			spinSprite[0].Rotate(0.00f);
			spinSprite[1].Rotate(0.00f);
			spinSprite[2].Rotate(0.00f);
			
		}
		
		public void Left()
			
		{
			
			spinSprite[0].Rotate(-0.060f);
			spinSprite[1].Rotate(-0.060f);
			spinSprite[2].Rotate(-0.060f);
			
			
		}
		
		public void Reset()
		{
			for (int i = 0; i < numberOfObstacles; i++)
				pivSprite[i].Position = new Vector2(spinSprite[i].Position.X+2500,spinSprite[i].Position.Y);
		}
		
		
		public bool HasCollidedWith(SpriteUV sprite)
		{
			//beam1 bounds
			Bounds2 beam1 = spinSprite[0].GetlContentLocalBounds();
			spinSprite[0].GetContentWorldBounds(ref beam1);
			
			//beam2 bounds
			Bounds2 beam2 = spinSprite[1].GetlContentLocalBounds();
			spinSprite[1].GetContentWorldBounds(ref beam2);
			
			//beam3 bounds
			Bounds2 beam3 = spinSprite[2].GetlContentLocalBounds();
			spinSprite[2].GetContentWorldBounds(ref beam3);
			
			//player bounds
			Bounds2 player = sprite.GetlContentLocalBounds();
			sprite.GetContentWorldBounds(ref player);
			
			if (player.Overlaps(beam1))
			{
				return true; 
			}
			
			if (player.Overlaps(beam2))
			{
				return true; 
			}
			
			if (player.Overlaps(beam3))
			{
				return true; 
			}
			
			else 
			{
				return false;
			}
		}
	}
}

