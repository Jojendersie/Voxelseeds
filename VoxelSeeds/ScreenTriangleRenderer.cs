using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class ScreenTriangleRenderer
    {
        /// <summary>
        /// singleton instance for all rendering processes
        /// </summary>
        public static readonly ScreenTriangleRenderer Instance = new ScreenTriangleRenderer();

        private bool initalised = false;
        private ScreenTriangleRenderer() { }

        Buffer<Vector2> _screenTriangle;
        VertexInputLayout _vertexInputLayout;
        RasterizerState _noneCullingState;

        private void Init(GraphicsDevice graphicsDevice)
        {
            _screenTriangle = SharpDX.Toolkit.Graphics.Buffer.Vertex.New(graphicsDevice, new[] {
                new Vector2(-1.0f, -1.0f),
                new Vector2(3.0f, -1.0f),
                new Vector2(-1.0f, 3.0f)
            }, SharpDX.Direct3D11.ResourceUsage.Immutable);

            _vertexInputLayout = VertexInputLayout.New(
                        VertexBufferLayout.New(0, new VertexElement[]{new VertexElement("POSITION", 0, SharpDX.DXGI.Format.R32G32_Float, 0)}, 0));

            var rasterizerStateDesc = SharpDX.Direct3D11.RasterizerStateDescription.Default();
            rasterizerStateDesc.CullMode = SharpDX.Direct3D11.CullMode.None;
            _noneCullingState = RasterizerState.New(graphicsDevice, "CullModeNone", rasterizerStateDesc);

            initalised = true;
        }

        /// <summary>
        /// draws a screen aligned triangle
        /// </summary>
        public void DrawScreenAlignedTriangle(GraphicsDevice device)
        {
            if (!initalised)
                Init(device);

            device.SetRasterizerState(_noneCullingState);
            device.SetVertexBuffer(_screenTriangle);
            device.SetVertexInputLayout(_vertexInputLayout);
            device.Draw(PrimitiveType.TriangleList, 3);
        }
    }
}
