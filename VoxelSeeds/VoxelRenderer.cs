using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Toolkit;

namespace VoxelSeeds
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;

    class VoxelRenderer
    {
        /// <summary>
        /// vertex buffer for a single cube (this one will be instanced)
        /// </summary>
        Buffer<Vector3> _cubeVertexBuffer;
        VertexInputLayout _vertexInputLayout;

        /// <summary>
        /// data structure instance
        /// </summary>
        class VoxelTypeInstanceData
        {
            public VoxelTypeInstanceData(GraphicsDevice graphicsDevice)
            {
                InstanceBuffer = Buffer.Vertex.New<Int32>(graphicsDevice, MAX_NUM_VOXELS_PER_TYPE, SharpDX.Direct3D11.ResourceUsage.Dynamic);
                InstanceDataRAM = new HashSet<Int32>();// System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(UInt32) * MAX_NUM_VOXELS_PER_TYPE);
            }
            
            public void UpdateGPUInstanceBuffer()
            {
                InstanceBuffer.SetDynamicData(InstanceBuffer.GraphicsDevice,
                    (ptr) =>
                    {
                        System.Runtime.InteropServices.Marshal.Copy(InstanceDataRAM.ToArray(), 0,
                                                                ptr, InstanceDataRAM.Count);
                    }, 0, SetDataOptions.Discard);
            }

            /// <summary>
            /// one instance buffer per VoxelType
            /// </summary>
            public Buffer<Int32> InstanceBuffer;

            /// <summary>
            /// ram copy of all instance data
            /// consisting of UInt32
            /// </summary>
            public HashSet<Int32> InstanceDataRAM;
        }
        VoxelTypeInstanceData[] _voxelTypeRenderingData;

        const int MAX_NUM_VOXELS_PER_TYPE = 8192;

        /// <summary>
        /// Effect for all Voxel-Renderings
        /// </summary>
        Effect _voxelEffect;

        public VoxelRenderer(GraphicsDevice graphicsDevice)
        {
            _cubeVertexBuffer = Buffer.Vertex.New(
                graphicsDevice,
                new[]
                    {
                        new Vector3(-0.5f, -0.5f, -0.5f), // Front
                        new Vector3(-0.5f, 0.5f, -0.5f),
                        new Vector3(0.5f, 0.5f, -0.5f),
                        new Vector3(-0.5f, -0.5f, -0.5f),
                        new Vector3(0.5f, 0.5f, -0.5f),
                        new Vector3(0.5f, -0.5f, -0.5f),
                        new Vector3(-0.5f, -0.5f, 0.5f), // BACK
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(-0.5f, 0.5f, 0.5f),
                        new Vector3(-0.5f, -0.5f, 0.5f),
                        new Vector3(0.5f, -0.5f, 0.5f),
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(-0.5f, 0.5f, -0.5f), // Top
                        new Vector3(-0.5f, 0.5f, 0.5f),
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(-0.5f, 0.5f, -0.5f),
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(0.5f, 0.5f, -0.5f),
                        new Vector3(-0.5f, -0.5f, -0.5f), // Bottom
                        new Vector3(0.5f, -0.5f, 0.5f),
                        new Vector3(-0.5f, -0.5f, 0.5f),
                        new Vector3(-0.5f, -0.5f, -0.5f),
                        new Vector3(0.5f, -0.5f, -0.5f),
                        new Vector3(0.5f, -0.5f, 0.5f),
                        new Vector3(-0.5f, -0.5f, -0.5f), // Left
                        new Vector3(-0.5f, -0.5f, 0.5f),
                        new Vector3(-0.5f, 0.5f, 0.5f),
                        new Vector3(-0.5f, -0.5f, -0.5f),
                        new Vector3(-0.5f, 0.5f, 0.5f),
                        new Vector3(-0.5f, 0.5f, -0.5f),
                        new Vector3(0.5f, -0.5f, -0.5f), // Right
                        new Vector3(0.5f, 0.5f, 0.5f),
                        new Vector3(0.5f, -0.5f, 0.5f),
                        new Vector3(0.5f, -0.5f, -0.5f),
                        new Vector3(0.5f, 0.5f, -0.5f),
                        new Vector3(0.5f, 0.5f, 0.5f)
                    }, SharpDX.Direct3D11.ResourceUsage.Immutable);

            // Create an input layout from the vertices
            _vertexInputLayout = VertexInputLayout.New(VertexBufferLayout.New(0, new VertexElement[]{new VertexElement("POSITION_CUBE", SharpDX.DXGI.Format.R32G32B32_Float)}, 0),
                                                       VertexBufferLayout.New(1, new VertexElement[]{new VertexElement("POSITION_INSTANCE", SharpDX.DXGI.Format.R32_SInt)}, 1));
                
            // Create instance buffer for every VoxelInfo
            _voxelTypeRenderingData = new VoxelTypeInstanceData[Enum.GetValues(typeof(VoxelType)).Length];
            for (int i = 0; i < _voxelTypeRenderingData.Length; ++i)
                _voxelTypeRenderingData[i] = new VoxelTypeInstanceData(graphicsDevice);

            // load shader
            EffectCompilerFlags compilerFlags = EffectCompilerFlags.None;
#if DEBUG
            compilerFlags |= EffectCompilerFlags.Debug;
#endif
            var voxelShaderCompileResult = EffectCompiler.CompileFromFile("Content/voxel.fx", compilerFlags);
            if (voxelShaderCompileResult.HasErrors)
            {
                System.Console.WriteLine(voxelShaderCompileResult.Logger.Messages);
                System.Diagnostics.Debugger.Break();
            }
            _voxelEffect = new SharpDX.Toolkit.Graphics.Effect(graphicsDevice, voxelShaderCompileResult.EffectData);
        }

        /// <summary>
        /// setups settings for a new map and clears all instance buffers
        /// </summary>
        public void Reset(Map map)
        {
            // reset lists
            for (int i = 0; i < _voxelTypeRenderingData.Length; ++i)
                _voxelTypeRenderingData[i].InstanceDataRAM.Clear();

            // add current world
            // test
            _voxelTypeRenderingData[0].InstanceDataRAM.Add(0);
            _voxelTypeRenderingData[0].InstanceDataRAM.Add(1);
            _voxelTypeRenderingData[0].InstanceDataRAM.Add(2);
            _voxelTypeRenderingData[0].InstanceDataRAM.Add(3);
            _voxelTypeRenderingData[0].UpdateGPUInstanceBuffer();

            // set vertex scaling
            _voxelEffect.ConstantBuffers["GlobalMapInfo"].Parameters["WorldSize"].SetValue(new Int3(map.SizeX, map.SizeY, map.SizeZ));
            _voxelEffect.ConstantBuffers["GlobalMapInfo"].IsDirty = true;
        }

        /// <summary>
        /// updates instancebuffer for the current "voxelsituation" (what ever)
        /// </summary>
        public void Update()
        {
        }

        public void Draw(Camera camera, GraphicsDevice graphicsDevice)
        {
            _voxelEffect.Parameters["WorldViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix);

            // Setup the vertices
            graphicsDevice.SetVertexBuffer(_cubeVertexBuffer, 0);
            graphicsDevice.SetVertexInputLayout(_vertexInputLayout);

            // render all instances
            _voxelEffect.CurrentTechnique.Passes[0].Apply();
            for (int i = 0; i < _voxelTypeRenderingData.Length; ++i)
            {
                graphicsDevice.SetVertexBuffer(1, _voxelTypeRenderingData[i].InstanceBuffer);
                graphicsDevice.DrawInstanced(PrimitiveType.TriangleList, _cubeVertexBuffer.ElementCount, _voxelTypeRenderingData[i].InstanceDataRAM.Count, 0, 0);
            }
        }
    }
}
