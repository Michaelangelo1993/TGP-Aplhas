using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Game
{
	public class Spring : Obstacle
	{
		private Trap trap;
		private bool missedSpring;
		
		private bool ready;
		private bool beingPushed;
		private bool springReleased;
		private SpriteUV springTopSprite;
		private TextureInfo springTopTextureInfo;
		public float springTopHeight;
		public float springTopWidth;
		
		private SpriteUV springSprite;
		private SpriteUV springSprite2;
		private TextureInfo springTextureInfo;
		private TextureInfo spring2TextureInfo;
		private float springOriginalHeight;
		private float springCurrentHeight;
		public float springWidth;
		
		public bool IsReady { get { return ready; }}
		public bool SetReady { set { ready = value; }}
		
		private Vector2		_min, _max;
		private Bounds2		_box;
				
		public float GetOriginalHeight { get { return springOriginalHeight; }}
		public bool BeingPushed { get { return beingPushed; }}
		public bool MissedSpring { get { return missedSpring; }}
		public bool IsReleased { get { return springReleased; }}
		public Vector2 GetPosition { get { return springSprite.Position; }}
		public float GetSpringWidth { get { return springWidth; }}
		public float GetTop { get { return (springTopSprite.Position.Y + springTopHeight); }}
		public Bounds2 GetBox() { return _box; }
		
		override public float GetEndPosition() { return (trap.GetEndPosition()); }
		
		public Spring (Scene scene, Vector2 position)
		{
			springReleased = false;
			missedSpring = false;
			beingPushed = false;
			
			// Initialise spring texture and sprite, get bounds and set position minus height offset
			springTextureInfo 		= new TextureInfo("/Application/textures/Spring.png");
			spring2TextureInfo		= new TextureInfo("/Application/textures/Spring.png");
			
			springSprite 			= new SpriteUV(springTextureInfo);
			springSprite2			= new SpriteUV(spring2TextureInfo);
			
			springSprite.Quad.S 	= springTextureInfo.TextureSizef;
			springSprite2.Quad.S 	= spring2TextureInfo.TextureSizef;
			
			Bounds2 springBounds 	= springSprite.Quad.Bounds2 ();
			springWidth 			= springBounds.Point10.X;
			springOriginalHeight 	= springBounds.Point01.Y;
			springCurrentHeight 	= springBounds.Point01.Y;	
			
			
			// Initialise spring texture and sprite, get bounds and set position minus height offset
			springTopTextureInfo = new TextureInfo("/Application/textures/SpringTop.png");
			springTopSprite = new SpriteUV(springTopTextureInfo);
			springTopSprite.Quad.S = springTopTextureInfo.TextureSizef;
			Bounds2 springTopBounds = springTopSprite.Quad.Bounds2 ();
			springTopHeight = springTopBounds.Point01.Y;
			springTopWidth = springTopBounds.Point10.X;
			float sizeDifference = 12; // Spring width is 100, spring top is 76, half it for x offset
			
			springSprite.Position 	= new Vector2(position.X, position.Y);
			springSprite2.Position  = springSprite.Position + new Vector2(58.0f, 48.0f); // Offset second spring for 3d effect
			
			trap = new Trap(scene, new Vector2((position.X + 125 + sizeDifference), 60));	
			springTopSprite.Position = new Vector2(position.X + sizeDifference, springSprite.Position.Y + springBounds.Point01.Y - 20);
			
			
			// Add sprites to scene
			
			scene.AddChild(springSprite);
			scene.AddChild(springSprite2);
			scene.AddChild(springTopSprite);
		}
		
		override public void Dispose()
		{
			
		}
		
		public void WindSpring(float gameSpeed)
		{
			if(!springReleased)
			{
				if(springCurrentHeight > 10)
				{
					beingPushed = true;
					springTopSprite.Position = new Vector2(springTopSprite.Position.X, springTopSprite.Position.Y-2*gameSpeed);
					springCurrentHeight-=2*gameSpeed;
					springSprite.Scale = new Vector2(springSprite.Scale.X, springCurrentHeight/springOriginalHeight);
					springSprite2.Scale = new Vector2(springSprite2.Scale.X, springCurrentHeight/springOriginalHeight);
				}
				else
				{
					// uncomment for auto release when fully pushed
					//springReleased = true;	
				}
			}
		}
		
		override public void ReleaseSpring(bool b)
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
		
		override public void Update(float speed)
		{			
			springSprite.Position = new Vector2(springSprite.Position.X - speed, springSprite.Position.Y);
			springSprite2.Position = new Vector2(springSprite2.Position.X - speed, springSprite2.Position.Y);
			springTopSprite.Position = new Vector2(springTopSprite.Position.X - speed, springTopSprite.Position.Y);
			
			trap.Update(speed);
			
			if(springReleased)
			{		
				// Spring can move too fast for collisions, split it up
				int iterations = (int)FMath.Ceiling(speed/3.0f);
				float speedPerCycle = speed/iterations;
				for(int i=0;i<iterations;i++)
				{
					// Update collision box
					_min.X			= springTopSprite.Position.X ;
					_min.Y			= springTopSprite.Position.Y ;
					_max.X			= springTopSprite.Position.X + springTopTextureInfo.TextureSizef.X;
					_max.Y			= springTopSprite.Position.Y + springTopTextureInfo.TextureSizef.Y;
					_box.Min 		= _min;			
					_box.Max 		= _max;
				
					// Check for collision with player
					if(AppMain.GetPlayer().GetBottomBox().Overlaps(_box))
						AppMain.GetPlayer().DoJump();
					
					// Update spring height
					if(springCurrentHeight < springOriginalHeight)
					{
						springTopSprite.Position = new Vector2(springTopSprite.Position.X, springTopSprite.Position.Y+(speedPerCycle*5));
						springCurrentHeight+=(speedPerCycle*5);
						springSprite.Scale = new Vector2(springSprite.Scale.X, springCurrentHeight/springOriginalHeight);
						springSprite2.Scale = new Vector2(springSprite2.Scale.X, springCurrentHeight/springOriginalHeight);
					}
					else
						springReleased = false;	
				}
				
			}
			else if(beingPushed)
			{
				WindSpring(speed);
			}
			
	
			Vector2 touchPos = AppMain.GetTouchPosition();
				
			if((touchPos.X-100 < springSprite.Position.X) &&
			   (touchPos.X+125 > springSprite2.Position.X + springWidth) &&
			   (touchPos.Y < springOriginalHeight+200)) // Touching spring
			{
				PushSpring();
			}
			
			if(Touch.GetData(0).ToArray().Length <= 0)
				ReleaseSpring(true);
		}
		
		override public void Reset(float x)
		{
			springReleased = true;
			missedSpring = false;
			beingPushed = false;
			
			float sizeDifference = (springTopWidth - springWidth)/2;
			springSprite.Position = new Vector2(x, springSprite.Position.Y);
			springSprite2.Position = springSprite.Position + new Vector2(58.0f, 48.0f);
			trap.SetXPos(x + 125 + sizeDifference);
			springTopSprite.Position = new Vector2(x + sizeDifference, springTopSprite.Position.Y);
		}
	}
}