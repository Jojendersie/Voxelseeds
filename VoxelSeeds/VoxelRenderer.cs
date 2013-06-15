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
        Buffer<CubeVertex> _cubeVertexBuffer;
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

        const int MAX_NUM_VOXELS_PER_TYPE = 131072;

        Matrix _translationMatrix = Matrix.Translation(Vector3.Zero);

        /// <summary>
        /// Effect for all Voxel-Renderings
        /// </summary>
        Effect _voxelEffect;


        RasterizerState _rasterizerState;

        struct CubeVertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 Texcoord;
        };

        public VoxelRenderer(GraphicsDevice graphicsDevice)
        {
            _cubeVertexBuffer = Buffer.Vertex.New(
                graphicsDevice,
                new[]
                    {
                        // 3D coordinates              UV Texture coordinates
                        new CubeVertex() { Position = new Vector3(-1.0f, -1.0f, -1.0f), Normal = -Vector3.UnitZ, Texcoord = new Vector2(0.0f, 1.0f) }, // Front
                        new CubeVertex() { Position = new Vector3(-1.0f,  1.0f, -1.0f), Normal = -Vector3.UnitZ, Texcoord = new Vector2(0.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f,  1.0f, -1.0f ), Normal = -Vector3.UnitZ, Texcoord = new Vector2(1.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f, -1.0f, -1.0f), Normal = -Vector3.UnitZ, Texcoord = new Vector2(0.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f,  1.0f, -1.0f ), Normal = -Vector3.UnitZ, Texcoord = new Vector2(1.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f, -1.0f, -1.0f ), Normal = -Vector3.UnitZ, Texcoord = new Vector2(1.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f, -1.0f,  1.0f), Normal = Vector3.UnitZ, Texcoord = new Vector2(1.0f, 0.0f) }, // BACK
                        new CubeVertex() { Position = new Vector3(1.0f,  1.0f,  1.0f ), Normal = Vector3.UnitZ, Texcoord = new Vector2(0.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f,  1.0f,  1.0f), Normal = Vector3.UnitZ, Texcoord = new Vector2(1.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f, -1.0f,  1.0f), Normal = Vector3.UnitZ, Texcoord = new Vector2(1.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f, -1.0f,  1.0f ), Normal = Vector3.UnitZ, Texcoord = new Vector2(0.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f,  1.0f,  1.0f ), Normal = Vector3.UnitZ, Texcoord = new Vector2(0.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f, 1.0f, -1.0f ), Normal = Vector3.UnitY, Texcoord = new Vector2(0.0f, 1.0f) }, // Top
                        new CubeVertex() { Position = new Vector3(-1.0f, 1.0f,  1.0f ), Normal = Vector3.UnitY, Texcoord = new Vector2(0.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f, 1.0f,  1.0f  ), Normal = Vector3.UnitY, Texcoord = new Vector2(1.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f, 1.0f, -1.0f ), Normal = Vector3.UnitY, Texcoord = new Vector2(0.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f, 1.0f,  1.0f  ), Normal = Vector3.UnitY, Texcoord = new Vector2(1.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f, 1.0f, -1.0f  ), Normal = Vector3.UnitY, Texcoord = new Vector2(1.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f,-1.0f, -1.0f ), Normal = -Vector3.UnitY, Texcoord = new Vector2(1.0f, 0.0f) }, // Bottom
                        new CubeVertex() { Position = new Vector3(1.0f,-1.0f,  1.0f  ), Normal = -Vector3.UnitY, Texcoord = new Vector2(0.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f,-1.0f,  1.0f ), Normal = -Vector3.UnitY, Texcoord = new Vector2(1.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f,-1.0f, -1.0f ), Normal = -Vector3.UnitY, Texcoord = new Vector2(1.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f,-1.0f, -1.0f  ), Normal = -Vector3.UnitY, Texcoord = new Vector2(0.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f,-1.0f,  1.0f  ), Normal = -Vector3.UnitY, Texcoord = new Vector2(0.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f, -1.0f, -1.0f), Normal = -Vector3.UnitX, Texcoord = new Vector2(0.0f, 1.0f) }, // Left
                        new CubeVertex() { Position = new Vector3(-1.0f, -1.0f,  1.0f), Normal = -Vector3.UnitX, Texcoord = new Vector2(0.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f,  1.0f,  1.0f), Normal = -Vector3.UnitX, Texcoord = new Vector2(1.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f, -1.0f, -1.0f), Normal = -Vector3.UnitX, Texcoord = new Vector2(0.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f,  1.0f,  1.0f), Normal = -Vector3.UnitX, Texcoord = new Vector2(1.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(-1.0f,  1.0f, -1.0f), Normal = -Vector3.UnitX, Texcoord = new Vector2(1.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f, -1.0f, -1.0f ), Normal = Vector3.UnitX, Texcoord = new Vector2(1.0f, 0.0f) }, // Right
                        new CubeVertex() { Position = new Vector3(1.0f,  1.0f,  1.0f ), Normal = Vector3.UnitX, Texcoord = new Vector2(0.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f, -1.0f,  1.0f ), Normal = Vector3.UnitX, Texcoord = new Vector2(1.0f, 1.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f, -1.0f, -1.0f ), Normal = Vector3.UnitX, Texcoord = new Vector2(1.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f,  1.0f, -1.0f ), Normal = Vector3.UnitX, Texcoord = new Vector2(0.0f, 0.0f) },
                        new CubeVertex() { Position = new Vector3(1.0f,  1.0f,  1.0f ), Normal = Vector3.UnitX, Texcoord = new Vector2(0.0f, 1.0f) }
                    }, SharpDX.Direct3D11.ResourceUsage.Immutable);

            // Create an input layout from the vertices
            _vertexInputLayout = VertexInputLayout.New(
                VertexBufferLayout.New(0, new VertexElement[]{new VertexElement("POSITION_CUBE", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0),
                                                              new VertexElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float, sizeof(float) * 3),
                                                              new VertexElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, sizeof(float) * 3)}, 0),
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

            // setup culling
            var rasterizerStateDesc = SharpDX.Direct3D11.RasterizerStateDescription.Default();
            rasterizerStateDesc.CullMode = SharpDX.Direct3D11.CullMode.None;
            _rasterizerState = RasterizerState.New(graphicsDevice, "CullModeCW", rasterizerStateDesc);
        }

        private static int GetRenderingDataIndex(VoxelType voxel)
        {
            return (int)voxel - 1;
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
            for (int posCode = 0; posCode < map.SizeX * map.SizeY * map.SizeZ; ++posCode)
            {
                var voxel = map.Get(posCode);
                if (voxel != VoxelType.EMPTY)
                {
                    // occluded?
                    var pos = map.DecodePosition(posCode);
                    if (pos.X > 0 && pos.X < map.SizeX-1 &&
                        pos.Y > 0 && pos.Y < map.SizeY-1 &&
                        pos.Z > 0 && pos.Z < map.SizeZ-1)
                    {
                        if (map.Get(pos + Int3.UnitX) != VoxelType.EMPTY &&
                            map.Get(pos + Int3.UnitY) != VoxelType.EMPTY &&
                            map.Get(pos + Int3.UnitZ) != VoxelType.EMPTY &&
                            map.Get(pos - Int3.UnitX) != VoxelType.EMPTY &&
                            map.Get(pos - Int3.UnitY) != VoxelType.EMPTY &&
                            map.Get(pos - Int3.UnitZ) != VoxelType.EMPTY)
                        {
                            continue;
                        }
                    }

                    // add
                    if(_voxelTypeRenderingData[GetRenderingDataIndex(voxel)].InstanceDataRAM.Count < MAX_NUM_VOXELS_PER_TYPE-1)
                        _voxelTypeRenderingData[GetRenderingDataIndex(voxel)].InstanceDataRAM.Add(posCode);
                }
            }

            foreach(var data in _voxelTypeRenderingData)
                data.UpdateGPUInstanceBuffer();

            // set vertex scaling
            _voxelEffect.ConstantBuffers["GlobalMapInfo"].Parameters["WorldSize"].SetValue(new Int3(map.SizeX, map.SizeY, map.SizeZ));
            _voxelEffect.ConstantBuffers["GlobalMapInfo"].IsDirty = true;

            _translationMatrix = Matrix.Translation(- new Vector3(map.SizeX, 0.0f, map.SizeZ) * 0.5f);
        }

        /// <summary>
        /// updates instancebuffer for the current "voxelsituation" (what ever)
        /// </summary>
        public void Update()
        {
        }

        /// <summary>
        /// draw what else
        /// </summary>
        public void Draw(Camera camera, GraphicsDevice graphicsDevice)
        {
            _voxelEffect.Parameters["WorldViewProjection"].SetValue(_translationMatrix * camera.ViewMatrix * camera.ProjectionMatrix);

            graphicsDevice.SetRasterizerState(_rasterizerState);

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
