
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNASystem.Interfaces;
using XNASystem.QuizArch;
using XNASystem.SystemMenus;
using XNASystem.Utils;
using Microsoft.Xna.Framework.Audio;

// Enumeration that specifies various Menu Actions
enum MenuAction
{
	ShowMain,
	ShowOptions,
	ShowQuiz,
	ShowGame,
	ShowScores,
	Return
}

public enum ActivityType
{
	Game,
	Quiz
}
// Enumeration that specifies the status of a quiz/booklet
public enum Status
{
	NotStarted,
	InProgress,
	Completed,
	Error
}

namespace XNASystem
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class SystemMain : Game
	{
		#region variable creation
		public static InputHandler GetInput = new InputHandler();
		public static SpriteBatch GameSpriteBatch;
		public static int Game;
		public static int Height { get; set; }
		public static int Width { get; set; }
		private Score _score;
		// graphics variables
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

        // initialize a list of fonts
        public static Dictionary<String, SpriteFont> FontPackage;

        // initilize a list of textures
        public static Dictionary<String, Texture2D> TexturePackage;
        
		// initialize the Global GameTime Variable
		public static GameTime CurrentGameTime;

		// initialize the menustack
		private Stack<IScreen> _menuStack;

        // initialize the DataManager
        private DataManager _dataManager;

		public static SoundEffect SoundShoot;
		public static SoundEffectInstance SoundShootInstance;

		public static SoundEffect SoundBoom;
		public static SoundEffectInstance SoundBoomInstance;

		public static List<SoundEffect> SoundsBackground;
		public static List<SoundEffectInstance> SoundsBackgroundInstance;

		public static SoundEffect SoundBackground;
		public static SoundEffectInstance SoundBackgroundInstance;

		public static SoundEffect SoundOuch;
		public static SoundEffectInstance SoundOuchInstance;

		public static SoundEffect SoundOof;
		public static SoundEffectInstance SoundOofInstance;

		public static SoundEffect SoundNoooo;
		public static SoundEffectInstance SoundNooooInstance;

		public static SoundEffect SoundQuizBg;
		public static SoundEffectInstance SoundQuizBgInstance;

		public static SoundEffect SoundShootEnemy;
		
		public static SoundEffectInstance SoundShootEnemyInstance;
		//initialize a new list of booklets
    	public static List<Booklet> Booklets;

		//initializes holders for the current booklets and quizzes
    	private Booklet _currentBooklet;
    	private Quiz _currentQuiz;

		public static DrawHelper Drawing;
		//public static List<ButtonAlias> PressedButtons = new List<ButtonAlias>();
		#region old possibly unneed variables
		/*        // stack of menus being drawn
        readonly List<Menu> _menuList = new List<Menu>();

        // Set of variables to initialize button debounce
        // these are required so when a user preses enter
        // only one menu choice gets pressed
        private int _up = 1;
        private int _down = 1;
        private int _enter = 1;

        // Current Choice in a menu, gets cached for users
        private int _choice;
*/
        // Question loader that will preload all quizzes in
        // a booklet
        private QuestionLoader _qLoad;

        // The target booklet to dump data into
        private Booklet _booklet;

		#endregion

		#endregion

		#region constructor
		// System Constructor, performs initialization
		public SystemMain()
		{
			Height = 720; Width = 1280;
			//Height = 600; Width = 800;
			
			// graphics initializer Also initialize the height and width to 720p
			//_graphics = new GraphicsDeviceManager(this);
			_graphics = new GraphicsDeviceManager(this) {PreferredBackBufferWidth = Width, PreferredBackBufferHeight = Height};
			//content location
			Content.RootDirectory = "Content";

            GamerServicesDispatcher.Initialize(Services);

			// initialize font package and texture package
			FontPackage = new Dictionary<String, SpriteFont>();
			TexturePackage = new Dictionary<String, Texture2D>();

			// create the stack
			_menuStack = new Stack<IScreen>();

            // create the DataManager and load name list
            _dataManager = new DataManager();

			// create a list of booklets the system can run off of
            Booklets = _dataManager.LoadBooklets(0);
        }

		public static int SelectedBooklet;

		#endregion

        #region Load and Unload
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to Draw textures.
			GameSpriteBatch = new SpriteBatch(GraphicsDevice);
			Drawing = new DrawHelper(GameSpriteBatch);

			SoundShoot = Content.Load<SoundEffect>("Audio\\Waves\\shoot");
			SoundShootInstance = SoundShoot.CreateInstance();

			SoundBoom = Content.Load<SoundEffect>("Audio\\Waves\\baboom");
			SoundBoomInstance = SoundBoom.CreateInstance();

			SoundShootEnemy = Content.Load<SoundEffect>("Audio\\Waves\\shoot_enemy");
			SoundShootEnemyInstance = SoundShootEnemy.CreateInstance();

			SoundOuch = Content.Load<SoundEffect>("Audio\\Waves\\ouch");
			SoundOuchInstance = SoundOuch.CreateInstance();

			SoundOof = Content.Load<SoundEffect>("Audio\\Waves\\oof");
			SoundOofInstance = SoundOof.CreateInstance();

			SoundNoooo = Content.Load<SoundEffect>("Audio\\Waves\\no");
			SoundNooooInstance = SoundNoooo.CreateInstance();

			SoundQuizBg = Content.Load<SoundEffect>("Audio\\Waves\\glow");
			SoundQuizBgInstance = SoundQuizBg.CreateInstance();

			SoundsBackground = new List<SoundEffect>();
			SoundsBackgroundInstance = new List<SoundEffectInstance>();
			for (var i = 1; i <= 12; i++)
			{
				var path = "Audio\\Waves\\groove" + i;
				SoundsBackground.Add(Content.Load<SoundEffect>(path));
				SoundsBackgroundInstance.Add(SoundsBackground[i-1].CreateInstance()); 
			}
			for (var i = 1; i <= 4; i++)
			{
				var path = "Audio\\Waves\\jgroove" + i;
				SoundsBackground.Add(Content.Load<SoundEffect>(path));
				SoundsBackgroundInstance.Add(SoundsBackground[i+12-1].CreateInstance()); 
			}

				// load the fonts
			FontPackage.Add("Arial",Content.Load<SpriteFont>("Fonts//Arial"));
			FontPackage.Add("Main", Content.Load<SpriteFont>("Fonts//Main"));
			FontPackage.Add("Title", Content.Load<SpriteFont>("Fonts//Title"));

            //load texture package
			TexturePackage.Add("Wall", Content.Load<Texture2D>("Sprites//wall"));
			TexturePackage.Add("Background", Content.Load<Texture2D>("Sprites//xnaGamesBackground_"+_graphics.PreferredBackBufferWidth+"_"+_graphics.PreferredBackBufferHeight));
			TexturePackage.Add("Ceiling", Content.Load<Texture2D>("Sprites//ceiling"));
			TexturePackage.Add("BreakoutPaddle", Content.Load<Texture2D>("Sprites//BreakoutGame//Breakout_Paddle"));
			TexturePackage.Add("HilightCenter", Content.Load<Texture2D>("Sprites//Hilight_center"));
			TexturePackage.Add("BreakoutBall", Content.Load<Texture2D>("Sprites//BreakoutGame//BreakoutBall"));
			TexturePackage.Add("BreakoutBlockTemplate", Content.Load<Texture2D>("Sprites//BreakoutGame//Breakout_Block_Template"));
			TexturePackage.Add("Ship",Content.Load<Texture2D>("Sprites//ShooterGame//ship"));
			TexturePackage.Add("ShipAlt",Content.Load<Texture2D>("Sprites//ShooterGame//ship_alternate"));
			TexturePackage.Add("BeginExplosionShip1",Content.Load<Texture2D>("Sprites//ShooterGame//beginexplosion_ship_1"));
			TexturePackage.Add("BeginExplosionShip2",Content.Load<Texture2D>("Sprites//ShooterGame//beginexplosion_ship_2"));
			TexturePackage.Add("BeginExplosionShip3",Content.Load<Texture2D>("Sprites//ShooterGame//beginexplosion_ship_3"));
			TexturePackage.Add("EnemyBasic",Content.Load<Texture2D>("Sprites//ShooterGame//shooterenemybasic"));
			TexturePackage.Add("EnemyBasicAlt",Content.Load<Texture2D>("Sprites//ShooterGame//shooterenemybasic_alternate"));
			TexturePackage.Add("BeginExplode1",Content.Load<Texture2D>("Sprites//ShooterGame//beginexplosion_1b"));
			TexturePackage.Add("BeginExplode2",Content.Load<Texture2D>("Sprites//ShooterGame//beginexplosion_2b"));
			TexturePackage.Add("BeginExplode3",Content.Load<Texture2D>("Sprites//ShooterGame//beginexplosion_3b"));
			for (int i = 1; i <= 8; i++)
			{
				TexturePackage.Add("Explode"+i, Content.Load<Texture2D>("Sprites//ShooterGame//explosion_"+i));
			}
			TexturePackage.Add("ShipDead",Content.Load<Texture2D>("Sprites//ShooterGame//dead_ship"));
			TexturePackage.Add("Bullet",Content.Load<Texture2D>("Sprites//ShooterGame//projectile"));
			TexturePackage.Add("EnemyBasicPain",Content.Load<Texture2D>("Sprites//ShooterGame//shooterenemybasic_pain"));
			TexturePackage.Add("Boss",Content.Load<Texture2D>("Sprites//ShooterGame//shooterboss"));
			TexturePackage.Add("BossAlt",Content.Load<Texture2D>("Sprites//ShooterGame//shooterboss_alternative"));
			TexturePackage.Add("BossPain",Content.Load<Texture2D>("Sprites//ShooterGame//shooterboss_pain"));
			TexturePackage.Add("ShooterBossExplosion1A", Content.Load<Texture2D>("Sprites//ShooterGame//ShooterBoss_explosion_1_a"));
        	for (int i = 3; i < 34; i++)
        	{
				TexturePackage.Add("ShooterBossExplosion"+i+"A", Content.Load<Texture2D>("Sprites//ShooterGame//ShooterBoss_explosion_"+i+"_a"));
        	}
			TexturePackage.Add("HilightLeft", Content.Load<Texture2D>("Sprites//Hilight_left"));
			TexturePackage.Add("HilightRight", Content.Load<Texture2D>("Sprites//Hilight_right"));
			TexturePackage.Add("UIBorder", Content.Load<Texture2D>("Sprites//UI//UIBorder"));
			TexturePackage.Add("UIFill", Content.Load<Texture2D>("Sprites//UI//UIFill"));
			TexturePackage.Add("UICorner", Content.Load<Texture2D>("Sprites//UI//UICorner"));
			TexturePackage.Add("BreakoutBallBlock", Content.Load<Texture2D>("Sprites//BreakoutGame//Breakout_BallBlock"));
			TexturePackage.Add("BreakoutBlockMetal", Content.Load<Texture2D>("Sprites//BreakoutGame//Breakout_Block_Meta_newl"));

			TexturePackage.Add("A", Content.Load<Texture2D>("Sprites//UI//A"));
			TexturePackage.Add("B", Content.Load<Texture2D>("Sprites//UI//B"));
			TexturePackage.Add("X", Content.Load<Texture2D>("Sprites//UI//X"));
			TexturePackage.Add("Y", Content.Load<Texture2D>("Sprites//UI//Y"));

            // give the stack the main menu
            _menuStack.Push(new MainMenu(_menuStack, this));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion

        #region update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
        	CurrentGameTime = gameTime;
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				Exit();
            // the keyState the keyboard is in right now
			GetInput.SetInputs(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
			_menuStack.Peek().Update();
			base.Update(gameTime);
            
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should Draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _menuStack.Peek().Draw();
            base.Draw(gameTime);
        }
        #endregion

        public void SetStack(Stack<IScreen> stack)
        {
            _menuStack = stack;
        }

		public List<Booklet> GetBookletList()
		{
			return Booklets;
		}

		public List<Quiz> GetQuizList()
		{
			return _currentBooklet.GetAsList();
		}

		public int GetCurrentBooklet()
		{
			return Booklets.IndexOf(_currentBooklet);
		}

		public int GetCurrentQuiz()
		{
			return _currentBooklet.GetAsList().IndexOf(_currentQuiz);
		}

		public void SetCurrentBooklet(int index)
		{
			_currentBooklet = Booklets[index];
			_currentQuiz = _currentBooklet.GetAsList().Count == 0 ? null : _currentBooklet.GetSpecificQuiz(0);
		}

		public void SetCurrentQuiz(int index)
		{
			_currentQuiz = _currentBooklet.GetSpecificQuiz(index);
		}

		public void CreateBooklet(string name)
		{
			Booklets.Add(new Booklet(name));
		}

		public void CreateQuiz(int bookletIndex, string name)
		{
			Booklets[bookletIndex].AddItem(new Quiz(name));
		}

		public void CreateQuestion(int bookletIndex, int quizIndex, string question, List<Answer> answers)
		{
			Booklets[bookletIndex].GetSpecificQuiz(quizIndex).AddItem(new Question(question, answers));
			//_booklets[bookletIndex].AddQuestionToQuiz(quizIndex, new Question(question, answers));
		}

		public void ReportScore(Score s)
		{
			_score = s;
		}

        public void Close()
        {
            foreach (Booklet booklet in Booklets)
            {
                _dataManager.SaveBooklet(0, booklet);
            }
            this.Exit();
        }
	}
}

