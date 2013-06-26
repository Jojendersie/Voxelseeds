using System;

using SharpDX;
using SharpDX.Toolkit;

namespace VoxelSeeds
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using System.Collections.Generic;

    /// <summary>
    /// A simulation game based on voxels.
    /// </summary>
    class VoxelSeeds : Game
    {
        private GraphicsDeviceManager _graphicsDeviceManager;

        Audio _audio;
        VoxelRenderer _voxelRenderer;
        Camera _camera;
        Seedbar _seedbar;
        Level _currentLevel;
        int _resourcesFromLastLevel;
        Background _background;
        double _cumulatedFrameTime;

        SpriteBatch _spriteBatch;
        SpriteFont _largeFont;
        Texture2D _pixTexture;

        /// <summary>
        /// current picked pos
        /// </summary>
        Int3 _pickedPos;
        /// <summary>
        /// true if there is a valid _pickedPos
        /// </summary>
        bool _pickPosAvailable;
        bool _pickPosSeedable;


        bool _gamePaused = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoxelSeeds" /> class.
        /// </summary>
        public VoxelSeeds()
        {
            Random.InitRandom((uint)System.DateTime.Today.Ticks);

            _audio = new Audio("ergon.wav");
            _audio.startSound();

            // Creates a graphics manager. This is mandatory.
            _graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // load settings
            Settings settings = new Settings();
            settings.ReadSettings();
            _graphicsDeviceManager.PreferredBackBufferWidth = settings.ResolutionX;
            _graphicsDeviceManager.PreferredBackBufferHeight = settings.ResolutionY;
            _graphicsDeviceManager.PreferredDepthStencilFormat = DepthFormat.Depth32;
            if (!System.Diagnostics.Debugger.IsAttached)    // fullscreen not allowed if debugger attached
                _graphicsDeviceManager.IsFullScreen = settings.Fullscreen;
            else
                _graphicsDeviceManager.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            Window.Title = "Voxel Seeds";

            base.Initialize();

            IsMouseVisible = true;

            var windowControl = Window.NativeWindow as System.Windows.Forms.Control;
            System.Diagnostics.Debug.Assert(windowControl != null);
            _camera = new Camera((float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, (float)Math.PI * 0.35f, 1.0f, 1000.0f, windowControl);

            
            var mainThreadDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            windowControl.MouseClick += (object sender, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    mainThreadDispatcher.BeginInvoke(new Action(() =>
                        {
                            if (_pickPosAvailable &&
                                _pickPosSeedable &&
                                TypeInformation.GetPrice(_seedbar.GetSeedInfo()._type) <= _currentLevel.Resources)
                            {
                                _currentLevel.Resources -= TypeInformation.GetPrice(_seedbar.GetSeedInfo()._type);
                                _currentLevel.InsertSeed(_pickedPos.X, _pickedPos.Y, _pickedPos.Z, _seedbar.GetSeedInfo()._type);
                            }
                        }));
            };
        }


        protected override void LoadContent()
        {
            _voxelRenderer = new VoxelRenderer(GraphicsDevice, Content);
            _currentLevel = new Level1(_voxelRenderer);
            _resourcesFromLastLevel = _currentLevel.Resources;
            
            _seedbar = new Seedbar(Window.NativeWindow as System.Windows.Forms.Control);
            _seedbar.LoadContent(GraphicsDevice, Content);

            _background = new Background(GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _largeFont = Content.Load<SpriteFont>("largefont.tkfnt");
            _pixTexture = Content.Load<Texture2D>("pixel.png");

            base.LoadContent();
        }

        void NextLevel()
        {
            // Take resources from last level to the next one
            _resourcesFromLastLevel = _currentLevel.Resources + 100;
            if(_currentLevel.GetType() == typeof(Level1))
                _currentLevel = new Level2(_voxelRenderer);
            else if (_currentLevel.GetType() == typeof(Level2))
                _currentLevel = new Level3(_voxelRenderer);
            else if (_currentLevel.GetType() == typeof(Level3))
                _currentLevel = new Level4(_voxelRenderer);
            else if (_currentLevel.GetType() == typeof(Level4))
                _currentLevel = new Level5(_voxelRenderer);
            else
                _currentLevel = new Level1(_voxelRenderer);
            _currentLevel.Resources = _resourcesFromLastLevel;
        }

        bool wasCheatPlusPressed = false;
        protected override void Update(GameTime gameTime)
        {
            if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Escape))
                Exit();

            // move camera
            _camera.Update(gameTime);

            if (!_gamePaused)
            {

                // Simulate level if 0.25f seconds passed.
                _cumulatedFrameTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (_cumulatedFrameTime > 0.25)
                {
                    _cumulatedFrameTime -= 0.25;
                    _currentLevel.Tick();
                }

                _seedbar.Update();

                // the permanent picking
                Pick();

                // stop game?
                if (_currentLevel.IsVictory() || _currentLevel.IsLost())
                {
                    _gamePaused = true;
                }

                // next/prev level cheat
                if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.OemPlus) && !wasCheatPlusPressed)
                {
                    NextLevel();
                    wasCheatPlusPressed = true;
                }
                wasCheatPlusPressed = !System.Windows.Input.Keyboard.IsKeyUp(System.Windows.Input.Key.OemPlus);
            }
            else
            {
                if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Enter))
                {
                    if (_currentLevel.IsVictory())
                        NextLevel();
                    else if (_currentLevel.IsLost())
                    {
                        _currentLevel = (Level)_currentLevel.GetType().GetConstructor(new[] { typeof(VoxelRenderer) }).Invoke(new object[] { (object)_voxelRenderer });
                        _currentLevel.Resources = _resourcesFromLastLevel;
                    }
                    _gamePaused = false;
                }
            }

            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // rendererererererererer
            _voxelRenderer.Draw(_camera, GraphicsDevice);

            // Draw a gost circle with the size of the reauired space
            if (_pickPosAvailable)
            {
                // Increase transparency if no correct seeding position or not enough resources
                bool bTooExpensive = TypeInformation.GetPrice(_seedbar.GetSeedInfo()._type) > _currentLevel.Resources;
                float transparency = (_pickPosSeedable) ? 0.75f : 0.25f;
                int radius = TypeInformation.GetRequiredSpace(_seedbar.GetSeedInfo()._type);
                int radiusSquare = radius * radius;
                for (int w = -radius; w <= radius; ++w)
                    for (int u = -radius; u <= radius; ++u) if(_currentLevel.GetMap().IsInside(_pickedPos.X + u, _pickedPos.Y, _pickedPos.Z + w))
                    {
                        Int32 pos = _currentLevel.GetMap().EncodePosition(_pickedPos.X + u, _pickedPos.Y, _pickedPos.Z + w);
                        if (((u * u + w * w) <= radiusSquare) && _currentLevel.GetMap().IsEmpty(pos))
                            // Use different types to show the reason for seeding problems
                            _voxelRenderer.DrawGhost(_camera, GraphicsDevice, bTooExpensive ? VoxelType.OAK_WOOD : VoxelType.TEAK_WOOD, pos, transparency);
                    }
            }

            // Handle base.Draw
            base.Draw(gameTime);

            _background.Draw(_camera, _currentLevel.LightDirection);

            _seedbar.Draw(_currentLevel, gameTime);

            if(_gamePaused)
            {
                _spriteBatch.Begin();
                string text1, text2;
                Vector2 stringSize1, stringSize2;
                Vector2 textPos1, textPos2;
                Color color1, color2;
                if (_currentLevel.IsLost())
                {
                    text1 = "You Lost against the Rottenness!";
                    stringSize1 = _largeFont.MeasureString(text1);
                    text2 = "Press Enter to restart";
                    stringSize2 = _largeFont.MeasureString(text2);
                    textPos1 = (new Vector2(GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height - stringSize1.Y) - stringSize1) * 0.5f;
                    textPos2 = (new Vector2(GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height + stringSize2.Y) - stringSize2) * 0.5f;
                    color1 = Color.DarkGreen;
                    color2 = Color.Black;
                }
                else// if (_currentLevel.IsVictory())
                {
                    text1 = "Your seeds prevailed against the Rottenness!";
                    stringSize1 = _largeFont.MeasureString(text1);
                    text2 = "Press Enter to continue to the next level";
                    stringSize2 = _largeFont.MeasureString(text2);
                    textPos1 = (new Vector2(GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height - stringSize1.Y) - stringSize1) * 0.5f;
                    textPos2 = (new Vector2(GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height + stringSize2.Y) - stringSize2) * 0.5f;
                    color1 = Color.DeepSkyBlue;
                    color2 = Color.Black;
                }

                _spriteBatch.Draw(_pixTexture, new DrawingRectangle((int)System.Math.Min(textPos1.X, textPos2.X) - 25, (int)textPos1.Y - 25,
                (int)System.Math.Max(stringSize1.X, stringSize2.X) + 50, (int)(stringSize1.Y + stringSize2.Y) + 50), Color.White * 0.6f);
           //     _spriteBatch.Draw(_pixTexture, new DrawingRectangle(0, (int)textPos1.Y - 25,
            //        GraphicsDevice.BackBuffer.Width, (int)(stringSize1.Y + stringSize2.Y) + 50), Color.White * 0.6f);
                _spriteBatch.DrawString(_largeFont, text1, textPos1, color1);
                _spriteBatch.DrawString(_largeFont, text2, textPos2, color2);
                _spriteBatch.End();
            }
        }

        private void Pick()
        {
            var ray = _camera.GetPickingRay(GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height);
            _pickPosAvailable = this._currentLevel.GetMap().PickPosition(ray, out _pickedPos);

            // Disable if it is not possible to seed at the picked position
            if (_currentLevel.GetMap().IsInside(_pickedPos.X, _pickedPos.Y, _pickedPos.Z))
            {
                _pickPosAvailable &= _seedbar.GetSelected() != -1;
                _pickPosAvailable &= _currentLevel.GetMap().Get(new Int3(_pickedPos.X, _pickedPos.Y - 1, _pickedPos.Z)) == VoxelType.GROUND;
                if (_pickPosAvailable)
                    _pickPosSeedable = GamePlayUtils.IsThereEnoughSpaceFor(_currentLevel.GetMap(), _seedbar.GetSeedInfo()._type, _pickedPos.X, _pickedPos.Y, _pickedPos.Z);
            }
            else _pickPosAvailable = false;
        }
    }
}
