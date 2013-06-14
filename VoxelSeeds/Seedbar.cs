using System;
using System.Windows.Input;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Content;

namespace VoxelSeeds
{
    class Seedbar : Game
    {
        private ContentManager barContentManager;
        private GraphicsDeviceManager barGraphicsDeviceManager;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Texture2D[] textures = new Texture2D[10];
        private Vector2[] positions = new Vector2[10];
       
        public Seedbar()
        {
            for(int i = 0; i < 10; i++)
            {
                positions[i] = new Vector2(i*32+5*i+5,5);
            }
        }

        public void LoadContent(GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
        {
            barGraphicsDeviceManager = graphicsDeviceManager;
            barContentManager = contentManager;
            spriteBatch = new SpriteBatch(barGraphicsDeviceManager.GraphicsDevice);
            font = barContentManager.Load<SpriteFont>("Arial16.tkfnt");
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = barContentManager.Load<Texture2D>("balls.dds");
            }
        }

        public void Update(GameTime gameTime)
        {
            double mouseX = Mouse.GetPosition(null).X;
            double mouseY = Mouse.GetPosition(null).Y;


        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font,"Hello Seeds",new Vector2(5,42),Color.White);
            for (int i = 0; i < 10; i++)
            {
                spriteBatch.Draw(textures[i], positions[i], new DrawingRectangle(0, 0, 32, 32), Color.White);
            }
            spriteBatch.End();
        }

    }
}
