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
    /// Simple MiniCube application using SharpDX.Toolkit.
    /// The purpose of this application is to show a rotating cube using <see cref="BasicEffect"/>.
    /// </summary>
    class VoxelSeeds : Game
    {
        private GraphicsDeviceManager _graphicsDeviceManager;

        VoxelRenderer _voxelRenderer;
        Camera _camera;
        Seedbar _seedbar;
        Level _currentLevel;
        Background _background;
        double _cumulatedFrameTime;

        /// <summary>
        /// current picked pos
        /// </summary>
        Int3 _pickedPos;
        /// <summary>
        /// true if there is a valid _pickedPos
        /// </summary>
        bool _pickPosAvailable;


        bool _gamePaused = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoxelSeeds" /> class.
        /// </summary>
        public VoxelSeeds()
        {
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
            Window.Title = "MiniCube demo";

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
                                _seedbar.GetSelected() >= 0 &&
                                _currentLevel.GetMap().IsInside(_pickedPos.X, _pickedPos.Y, _pickedPos.Z) &&
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
            
            _seedbar = new Seedbar(Window.NativeWindow as System.Windows.Forms.Control);
            _seedbar.LoadContent(GraphicsDevice, Content);


            _background = new Background(GraphicsDevice);

            base.LoadContent();
        }

        void NextLevel()
        {
        //    if(_currentLevel.GetType() == typeof(Level1))
            _currentLevel = new Level2(_voxelRenderer);
        }

        protected override void Update(GameTime gameTime)
        {
   

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
                var ray = _camera.GetPickingRay(GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height);
                _pickPosAvailable = this._currentLevel.GetMap().PickPosition(ray, out _pickedPos);

                // stop game?
                if (_currentLevel.IsVictory() || _currentLevel.IsLost())
                {
                    _gamePaused = true;
                    // TODO: Display message
                }
            }
            else
            {
                if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Enter))
                {
                    if (_currentLevel.IsVictory())
                        NextLevel();
                    else if (_currentLevel.IsLost())
                        _currentLevel = (Level)_currentLevel.GetType().GetConstructor(new[] { typeof(VoxelRenderer) }).Invoke(new object[] { (object)_voxelRenderer} );
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

            if (_pickPosAvailable && _seedbar.GetSelected() != -1 && _currentLevel.GetMap().IsInside(_pickedPos.X,_pickedPos.Y, _pickedPos.Z))
                _voxelRenderer.DrawGhost(_camera, GraphicsDevice, _seedbar.GetSeedInfo()._type, _currentLevel.GetMap().EncodePosition(_pickedPos));

            // Handle base.Draw
            base.Draw(gameTime);

            _background.Draw(_camera);

            _seedbar.Draw(_currentLevel, gameTime);
        }
    }
}
