using System;

using SharpDX;
using SharpDX.Toolkit;

namespace VoxelSeeds
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;

    /// <summary>
    /// Simple MiniCube application using SharpDX.Toolkit.
    /// The purpose of this application is to show a rotating cube using <see cref="BasicEffect"/>.
    /// </summary>
    class VoxelSeeds : Game
    {
        private GraphicsDeviceManager _graphicsDeviceManager;

        VoxelRenderer _voxelRenderer;

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
        }

        protected override void LoadContent()
        {
            _voxelRenderer = new VoxelRenderer(GraphicsDevice);

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "MiniCube demo";

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // Rotate the cube.
          //  var time = (float)gameTime.TotalGameTime.TotalSeconds;
          //  basicEffect.World = Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f);

            // add/remove voxels from voxelrenderer
            _voxelRenderer.Update();

            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // rendererererererererer
            _voxelRenderer.Draw(GraphicsDevice);

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
