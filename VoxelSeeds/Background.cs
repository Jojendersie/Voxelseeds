using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelSeeds
{
    class Background
    {
        Effect _effect;
      //  DepthStencilState _depthStencilDrawWhereNothing;

        public Background(GraphicsDevice graphicsDevice)
        {
     /*       var depthStencilStateDesc = SharpDX.Direct3D11.DepthStencilStateDescription.Default();
            depthStencilStateDesc.IsDepthEnabled = false;
            depthStencilStateDesc.DepthWriteMask = SharpDX.Direct3D11.DepthWriteMask.Zero;
            _noDepthState = DepthStencilState.New(graphicsDevice, "NoZBuffer", depthStencilStateDesc);*/

            EffectCompilerFlags compilerFlags = EffectCompilerFlags.None;
#if DEBUG
            compilerFlags |= EffectCompilerFlags.Debug;
#endif
            var shaderCompileResult = EffectCompiler.CompileFromFile("Content/sky.fx", compilerFlags);
            if (shaderCompileResult.HasErrors)
            {
                System.Console.WriteLine(shaderCompileResult.Logger.Messages);
                System.Diagnostics.Debugger.Break();
            }
            _effect = new SharpDX.Toolkit.Graphics.Effect(graphicsDevice, shaderCompileResult.EffectData);

        }
        
        public void Draw(Camera camera, Vector3 lightDirection)
        {
            var graphicsDevice = _effect.GraphicsDevice;
            //graphicsDevice.SetDepthStencilState(_noDepthState);

            var viewProjection = camera.ViewMatrix * camera.ProjectionMatrix;
            var inverseViewProjection = viewProjection; inverseViewProjection.Invert();
            _effect.Parameters["InverseViewProjection"].SetValue(inverseViewProjection);
            _effect.Parameters["LightDirection"].SetValue(-lightDirection);

            _effect.CurrentTechnique.Passes[0].Apply();
            ScreenTriangleRenderer.Instance.DrawScreenAlignedTriangle(graphicsDevice);
        }
    }
}
