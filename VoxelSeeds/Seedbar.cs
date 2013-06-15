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
 class SeedInfo
        {
            public VoxelType _type;
            public Vector2 _position;
            /*
            public SeedInfo(VoxelType type, Texture2D texture, Vector2 position)
            {
                _type = type;
                _texture = texture;
                _position = position;
            }
            */
        }
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private SeedInfo[] seeds = new SeedInfo[10];
        private Texture2D[] textures = new Texture2D[10];
        private Texture2D helix;

        private const int barLength = 10;

        public Seedbar()
        {
            for(int i = 0; i < barLength; i++)
            {
                seeds[i] = new SeedInfo();
                seeds[i]._position = new Vector2(i*32+15*i+5,5);
                seeds[i]._type = VoxelType.WOOD;
            }

        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {

            spriteBatch = new SpriteBatch(graphicsDevice);
            font = contentManager.Load<SpriteFont>("Arial16.tkfnt");

            spriteBatch = new SpriteBatch(graphicsDevice);
            font = contentManager.Load<SpriteFont>("Arial16.tkfnt");
            helix = contentManager.Load<Texture2D>("balls.dds");
            for (int i = 0; i < barLength; i++)
            {
                textures[i] = contentManager.Load<Texture2D>("balls.dds");
            }
        }

        private bool MouseOver(Vector2 pos, double width, double heigth)
        {
            // hi pat - nice try, but now I know better - this will not work most likely :(
            // (don't know exactly why...)
            // if it happens as feared, look at the handler registrations in the ctor of Camera
            // hf ;)

            double mouseX = Mouse.GetPosition(null).X;
            double mouseY = Mouse.GetPosition(null).Y;

            return(mouseX >= pos.X && mouseX <= pos.X + width && mouseY >= pos.Y && mouseY < pos.Y + heigth);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < barLength; i++)
            {
                if (MouseOver(seeds[i]._position, 32, 32) && Mouse.LeftButton == MouseButtonState.Pressed)
                { 
                    //TODO:MOUSE_CHange_TO_VOXEL
                }
            }  
        }

        public void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            for (int i = 0; i < barLength; i++)
            {
                spriteBatch.Draw(textures[(int)seeds[i]._type], new DrawingRectangle((int)seeds[i]._position.X, (int)seeds[i]._position.Y + 37, 10, 20), Color.White);
                spriteBatch.Draw(textures[(int)seeds[i]._type], seeds[i]._position, new DrawingRectangle(0, 0, 32, 32), Color.White);
                spriteBatch.DrawString(font, TypeInformation.getPrice(seeds[i]._type).ToString(), new Vector2(seeds[i]._position.X+7,seeds[i]._position.Y + 36), Color.White);
            }
            spriteBatch.End();
        }

    }
}
