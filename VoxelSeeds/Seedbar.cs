using System;
using System.Windows.Input;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Diagnostics;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Content;

namespace VoxelSeeds
{
    class Seedbar
    {
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

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);
            font = contentManager.Load<SpriteFont>("Arial16.tkfnt");
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = contentManager.Load<Texture2D>("balls.dds");
            }
        }

        public void Update(GameTime gameTime)
        {
            // hi pat - nice try, but now I know better - this will not work most likely :(
            // (don't know exactly why...)
            // if it happens as feared, look at the handler registrations in the ctor of Camera
            // hf ;)

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
