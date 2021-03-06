using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNASystem.Interfaces;
using XNASystem.ShooterGame;

namespace XNASystem.Shooter
{
	class ShooterShip : IGameObject
	{

		private float _xPosition;
		private float _yPosition;
		private int _hitPoints;
		private const int Width = 45;
		private const int Height = 45;
		private int _xMin;
		private int _xMax;
		private bool _isDying = false;
		private ShooterProjectile _shot;

		//animation stuff
		private float timer = 0f;
		private float interval = 1000f / 7f;
		private int frameCount;
		private int currentFrame = 0;
		private String _currentSprite;
		private List<String> _standardSprites = new List<String> {"Ship"};
		private List<String> _shootSprites = new List<String> { "ShipAlt" };
		private List<String> _deadSprites = new List<String> { "BeginExplosionShip1", "BeginExplosionShip2", "BeginExplosionShip3", "Explode1", "Explode2", "Explode3", "Explode4", "Explode5", "Explode6", "Explode7", "Explode8", "ShipDead" };
		//private List<int> _explodeSprites = new List<int> {18, 19, 20, 21, 22};
		private List<String> _currentSprites;
		private Rectangle _collisionBox;



		public ShooterShip(int xMin, int xMax)
		{
			_xMin = xMin;
			_xMax = xMax;
			_xPosition = _xMax/2;
			_yPosition = SystemMain.Height-Height-5;
			_currentSprite = "Ship";
			_currentSprites = _standardSprites;
			frameCount = _currentSprites.Count;
			_collisionBox = new Rectangle((int)_xPosition, (int)_yPosition, 50, 50);
		}

		public void UpdatePostion(float x, float y)
		{
			_xPosition += 7*x;
			if(_xPosition <= _xMin)
			{
				_xPosition = (float) 0.1;
			}
			if(_xPosition >= ( _xMax - Width ) )
			{
				_xPosition = (float) (_xMax - (Width + 0.1));
			}

			_collisionBox.Location = new Point((int)_xPosition, (int)_yPosition);
		}

		public void UpdateProjectile()
		{
			if (_shot != null)
			{
				_shot.UpdatePostion(0, 0);

				if (_shot.GetY() < 0)
				{
					KillProjectile();
				}
			}
		}

		public Rectangle GetCollisionBox()
		{
			return _collisionBox;
		}

		public Rectangle GetShotCollisionBox()
		{
			return _shot.GetCollisionBox();
		}

		public void Shoot()
		{
			if (!IsDying() && (_shot == null))
			{
				SystemMain.SoundShootInstance.Volume = 1.0f;
				SystemMain.SoundShootInstance.Play();
				_currentSprites = _shootSprites;
				frameCount = _currentSprites.Count;
				currentFrame = 0;

				_shot = new ShooterProjectile(_xPosition + Width/2*1 - 5, _yPosition - 7, 10, 10, 0, 15, Color.White);
			}
		}

		public void Reload()
		{
			if (!IsDying())
			{
				_currentSprites = _standardSprites;
				frameCount = _currentSprites.Count;
				currentFrame = 0;
			}
		}

		public ShooterProjectile GetShot()
		{
			return _shot;
		}

		public void KillProjectile()
		{
			_shot = null;
		}

		public void Kill()
		{
			_isDying = true;
			_currentSprites = _deadSprites;
			frameCount = _currentSprites.Count;
			currentFrame = 0;
			SystemMain.SoundBoom.Play();
		}

		public bool IsDead()
		{
			bool isSprites = _currentSprites.Equals(_deadSprites);
			bool isCount = (currentFrame >= _currentSprites.Count - 1);
			return isSprites && isCount;
		}

		public bool IsDying()
		{
			return _isDying;
		}

		public void AnimateSprite(GameTime gameTime)
		{
			if (!IsDead())
			{
				timer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;

				if (timer > interval)
				{
					currentFrame++;
					if (currentFrame > (frameCount - 1))
					{
						currentFrame = 0;
					}
					timer = 0f;
				}

				_currentSprite = _currentSprites[currentFrame];
			}
		}

		public void Draw()
		{
			SystemMain.GameSpriteBatch.Draw(SystemMain.TexturePackage[_currentSprite], new Vector2(_xPosition, _yPosition), Color.White);

			if(_shot != null)
			{
				_shot.Draw();
			}
		}

		public float GetX()
		{
			return _xPosition;
		}

		public float GetY()
		{
			return _yPosition;
		}

		public int GetWidth()
		{
			return Width;
		}

		public int GetHeight()
		{
			return Height;
		}
	}
}
