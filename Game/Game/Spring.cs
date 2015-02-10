using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Game
{
	public class Spring
	{
		private Trap trap;
		private bool missedSpring;
		private bool beingPushed;
		private bool springReleased;
		private SpriteUV springTopSprite;
		private TextureInfo springTopTextureInfo;
		public float springTopHeight;
		public float springTopWidth;
		
		private SpriteUV springSprite;
		private TextureInfo springTextureInfo;
		private float springOriginalHeight;
		private float springCurrentHeight;
		public float springWidth;
		
		private static Vector2		_min, _max;
		private static Bounds2		_box;
				
		public float GetOriginalHeight { get { return springOriginalHeight; }}
		public bool BeingPushed { get { return beingPushed; }}
		public bool MissedSpring { get { return missedSpring; }}
		public bool IsReleased { get { return springReleased; }}
		public Vector2 GetPosition { get { return springSprite.Position; }}
		public float GetSpringWidth { get { return springWidth; }}
		public float GetTop { get { return (springTopSprite.Position.Y + springTopHeight); }}
		public Bounds2 GetBox() { return _box; }
		
		public Spring (Scene scene, Vector2 position)
		{
			springReleased = false;
			missedSpring = false;
			beingPushed = false;
			
			// Initialise spring texture and sprite, get bounds and set position minus height offset
			springTextureInfo = new TextureInfo("/Application/textures/Spring.png");
			springSprite = new SpriteUV(springTextureInfo);
			springSprite.Quad.S = springTextureInfo.TextureSizef;
			Bounds2 springBounds = springSprite.Quad.Bounds2 ();
			springSprite.Position = new Vector2(position.X, position.Y);
			springWidth = springBounds.Point10.X;
			springOriginalHeight = springBounds.Point01.Y;
			springCurrentHeight = springBounds.Point01.Y;
			
			trap = new Trap(scene, new Vector2((position.X + 125), (position.Y )));			
			
			// Initialise spring texture and sprite, get bounds and set position minus height offset
			springTopTextureInfo = new TextureInfo("/Application/textures/SpringTop.png");
			springTopSprite = new SpriteUV(springTopTextureInfo);
			springTopSprite.Quad.S = springTopTextureInfo.TextureSizef;
			Bounds2 springTopBounds = springTopSprite.Quad.Bounds2 ();
			springTopHeight = springTopBounds.Point01.Y;
			springTopWidth = springTopBounds.Point10.X;
			float sizeDifference = (springTopWidth - springWidth)/2;
			springTopSprite.Position = new Vector2(position.X - sizeDifference, springSprite.Position.Y + springBounds.Point01.Y);
			
			// Add sprites to scene
			scene.AddChild(springSprite);
			scene.AddChild(springTopSprite);
		}
		
		public void Dispose()
		{
			
		}
		
		public void WindSpring()
		{
			if(!springReleased)
			{
				if(springCurrentHeight > 55)
				{
					beingPushed = true;
					springTopSprite.Position = new Vector2(springTopSprite.Position.X, springTopSprite.Position.Y-3);
					springCurrentHeight-=3;
					springSprite.Scale = new Vector2(springSprite.Scale.X, springCurrentHeight/springOriginalHeight);
				}
				else
				{
					// uncomment for auto release when fully pushed
					//springReleased = true;	
				}
			}
		}
		
		public void ReleaseSpring(bool b)
		{ 
			springReleased = b;
		}
		
		public void MissSpring()
		{
			missedSpring = true;
		}
		
		public void PushSpring()
		{
			beingPushed = true;
		}
		
		public void Update(float deltaTime, float speed)
		{
			springSprite.Position = new Vector2(springSprite.Position.X - speed, springSprite.Position.Y);
			springTopSprite.Position = new Vector2(springTopSprite.Position.X - speed, springTopSprite.Position.Y);
			trap.Update(deltaTime, speed);
			
			if(springReleased)
			{
				if(springCurrentHeight < springOriginalHeight)
				{
					springTopSprite.Position = new Vector2(springTopSprite.Position.X, springTopSprite.Position.Y+15);
					springCurrentHeight+=15;
					springSprite.Scale = new Vector2(springSprite.Scale.X, springCurrentHeight/springOriginalHeight);
				}
				else
				{
					springReleased = false;	
				}
			}
			else if(beingPushed)
			{
				WindSpring();
			}
			
			_min.X			= springTopSprite.Position.X ;
			_min.Y			= springTopSprite.Position.Y ;
			_max.X			= springTopSprite.Position.X + springTopTextureInfo.TextureSizef.X;
			_max.Y			= springTopSprite.Position.Y + springTopTextureInfo.TextureSizef.Y;
			_box.Min 		= _min;			
			_box.Max 		= _max;
		}
		
		public void Reset()
		{
			springReleased = false;
			missedSpring = false;
			beingPushed = false;
			springSprite.Position = new Vector2((springSprite.Position.X + 2500), (springSprite.Position.Y));
			trap.Reset();
			springTopSprite.Position += new Vector2(2500, 0);
		}
	}
}