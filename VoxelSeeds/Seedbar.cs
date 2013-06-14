using System;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Content;

namespace VoxelSeeds
{
    class Seedbar
    {
        private static ContentManager barContentManager;
        private static GraphicsDeviceManager barGraphicsDeviceManager;
        private static SpriteBatch spriteBatch;
        private static SpriteFont font;
        private static Texture2D ballsTexture;
       
        public static void LoadContent(GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
        {
            barGraphicsDeviceManager = graphicsDeviceManager;
            barContentManager = contentManager;
            spriteBatch = new SpriteBatch(barGraphicsDeviceManager.GraphicsDevice);
            font = barContentManager.Load<SpriteFont>("Arial16.tkfnt");
            ballsTexture = barContentManager.Load<Texture2D>("balls.dds");
        }

        public static void Update()
        {
            
        }

        public static void Draw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font,"Hello Seeds",new Vector2(5,40),Color.White);
            spriteBatch.Draw(ballsTexture, new DrawingRectangle(5, 5, 30, 30),Color.White);
            spriteBatch.End();
        }

    }
}
