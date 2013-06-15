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
        private Texture2D topbar;
        private int selected = 0;
        private bool picking;
        private System.Drawing.Point mousePosition;
        private int windowHeigth;
        private int windowWidth;

        private const int barLength = 10;

        public Seedbar(System.Windows.Forms.Control inputControlElement)
        {       

            // input handling...
            inputControlElement.MouseDown += (object sender, System.Windows.Forms.MouseEventArgs e) =>
                picking = e.Button == System.Windows.Forms.MouseButtons.Left;
            inputControlElement.MouseUp += (object sender, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (picking && e.Button == System.Windows.Forms.MouseButtons.Left)
                    picking = false;
            };
            inputControlElement.MouseMove += (object sender, System.Windows.Forms.MouseEventArgs e) =>
                mousePosition = e.Location;
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);
            windowHeigth = spriteBatch.GraphicsDevice.BackBuffer.Height;
            windowWidth = spriteBatch.GraphicsDevice.BackBuffer.Width;
            font = contentManager.Load<SpriteFont>("Arial16.tkfnt");
            helix = contentManager.Load<Texture2D>("balls.dds");
           // topbar = contentManager.Load<Texture2D>("dummy1.png");
            for (int i = 0; i < barLength; i++)
            {
                seeds[i] = new SeedInfo();
                seeds[i]._position = new Vector2(i * (windowWidth - 40) / 10 + 5, 5);
                seeds[i]._type = VoxelType.WOOD;
                textures[i] = contentManager.Load<Texture2D>("balls.dds");
            }
        }

        private bool MouseOver(Vector2 pos, double width, double heigth)
        {
            // hi pat - nice try, but now I know better - this will not work most likely :(
            // (don't know exactly why...)
            // if it happens as feared, look at the handler registrations in the ctor of Camera
            // hf ;)

            return (mousePosition.X >= pos.X && mousePosition.X <= pos.X + width && mousePosition.Y >= pos.Y && mousePosition.Y < pos.Y + heigth);
        }

        public void Update()
        {
            for (int i = 0; i < barLength; i++)
            {
                if (MouseOver(seeds[i]._position, 32, 32) && picking)
                {
                    selected = i+1;
                }
            }  
        }

        public void Draw()
        {       
            int soll = 200;
            int progress = windowHeigth * Map.getGoodVoxels()/soll;

            spriteBatch.Begin();

           // spriteBatch.Draw(topbar)
            spriteBatch.Draw(helix, new DrawingRectangle(windowWidth, windowHeigth, 30, progress), null, Color.White, (float)Math.PI, new Vector2(0, 0), SpriteEffects.None, 0);
            if (selected > 0) spriteBatch.DrawString(font, selected.ToString(), new Vector2(200, 100), Color.White);

            for (int i = 0; i < barLength; i++)
            {
                //draw curency
                spriteBatch.Draw(helix, new DrawingRectangle((int)seeds[i]._position.X + 37, (int)seeds[i]._position.Y+5, 10, 20), Color.White);
                spriteBatch.DrawString(font, TypeInformation.getPrice(seeds[i]._type).ToString(), new Vector2(seeds[i]._position.X + 45, seeds[i]._position.Y+5), Color.White); 
                //draw Icons
                spriteBatch.Draw(textures[(int)seeds[i]._type], seeds[i]._position, new DrawingRectangle(0, 0, 32, 32), Color.White);
            }

            for (int i = 0; i < barLength; i++)
            {
                if (MouseOver(seeds[i]._position, 32, 32))
                {
                    spriteBatch.DrawString(font, "test" + i.ToString(), new Vector2(mousePosition.X+10, mousePosition.Y+10), Color.White);
                }
            }
            spriteBatch.End();
        }

    }
}
