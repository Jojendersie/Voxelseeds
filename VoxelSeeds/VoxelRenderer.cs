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
    using SharpDX.Toolkit.Content;

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
            public VoxelTypeInstanceData(GraphicsDevice graphicsDevice, VoxelType voxel)
            {
                Voxel = voxel;
                InstanceBuffer = Buffer.Vertex.New<Int32>(graphicsDevice, TypeInformation.GetMaxNumberOfVoxels(voxel), SharpDX.Direct3D11.ResourceUsage.Dynamic);
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

            /// <summary>
            /// used texture for this instance type
            /// </summary>
            public Texture2D Texture;

            public VoxelType Voxel;
        }
        VoxelTypeInstanceData[] _voxelTypeRenderingData;

        /// <summary>
        /// Effect for all Voxel-Renderings
        /// </summary>
        Effect _voxelEffect;

        RasterizerState _backfaceCullingState;
        RasterizerState _noneCullingState;
        DepthStencilState _depthStencilStateState;
        SamplerState _pointSamplerState;
        BlendState _blendStateOpaque;
        BlendState _blendStateTransparent;

        Buffer<Int32> _singleInstanceBuffer;

        struct CubeVertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 Texcoord;
        };

        public VoxelRenderer(GraphicsDevice graphicsDevice, ContentManager contentManager)
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
                                                              new VertexElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, sizeof(float) * 6)}, 0),
                VertexBufferLayout.New(1, new VertexElement[]{new VertexElement("POSITION_INSTANCE", SharpDX.DXGI.Format.R32_SInt)}, 1));
                
            // Create instance buffer for every VoxelInfo
            _voxelTypeRenderingData = new VoxelTypeInstanceData[Enum.GetValues(typeof(VoxelType)).Length-1];
            for (int i = 0; i < _voxelTypeRenderingData.Length; ++i)
                _voxelTypeRenderingData[i] = new VoxelTypeInstanceData(graphicsDevice, (VoxelType)(i+1));
            LoadTextures(contentManager);


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

            // setup states
            var rasterizerStateDesc = SharpDX.Direct3D11.RasterizerStateDescription.Default();
            rasterizerStateDesc.CullMode = SharpDX.Direct3D11.CullMode.Back;
            _backfaceCullingState = RasterizerState.New(graphicsDevice, "CullModeBack", rasterizerStateDesc);
            rasterizerStateDesc.CullMode = SharpDX.Direct3D11.CullMode.None;
            _noneCullingState = RasterizerState.New(graphicsDevice, "CullModeNone", rasterizerStateDesc);

            var depthStencilStateDesc = SharpDX.Direct3D11.DepthStencilStateDescription.Default();
            depthStencilStateDesc.IsDepthEnabled = true;
            _depthStencilStateState = DepthStencilState.New(graphicsDevice, "NormalZBufferUse", depthStencilStateDesc);
            
            var samplerStateDesc = SharpDX.Direct3D11.SamplerStateDescription.Default();
            samplerStateDesc.AddressV = SharpDX.Direct3D11.TextureAddressMode.Mirror;
            samplerStateDesc.AddressU = SharpDX.Direct3D11.TextureAddressMode.Mirror;
            samplerStateDesc.Filter = SharpDX.Direct3D11.Filter.MinMagMipPoint;
            _pointSamplerState = SamplerState.New(graphicsDevice, "PointSampler", samplerStateDesc);
            _voxelEffect.Parameters["PointSampler"].SetResource(_pointSamplerState);

            var blendStateDesc = SharpDX.Direct3D11.BlendStateDescription.Default();
            _blendStateOpaque = BlendState.New(graphicsDevice, "Opaque", blendStateDesc);
            blendStateDesc.RenderTarget[0].IsBlendEnabled = true;
            blendStateDesc.RenderTarget[0].SourceBlend = SharpDX.Direct3D11.BlendOption.SourceAlpha;
            blendStateDesc.RenderTarget[0].DestinationBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha;
            blendStateDesc.RenderTarget[0].BlendOperation = SharpDX.Direct3D11.BlendOperation.Add;
            _blendStateTransparent = BlendState.New(graphicsDevice, "AlphaBlend", blendStateDesc);

            // vertexbuffer for a single instance
            _singleInstanceBuffer = Buffer.Vertex.New<Int32>(graphicsDevice, 1, SharpDX.Direct3D11.ResourceUsage.Dynamic);
        }

        private void LoadTextures(ContentManager contentManager)
        {
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.TEAK_LEAF)].Texture = contentManager.Load<Texture2D>("leafs.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.GROUND)].Texture = contentManager.Load<Texture2D>("ground.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.ROCK)].Texture = contentManager.Load<Texture2D>("redwood.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.BEECH_WOOD)].Texture = contentManager.Load<Texture2D>("Beech.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.NOBLEROT_FUNGUS)].Texture = contentManager.Load<Texture2D>("fungus.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.WHITEROT_FUNGUS)].Texture = contentManager.Load<Texture2D>("fungus.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.OAK_WOOD)].Texture = contentManager.Load<Texture2D>("oak.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.PINE_WOOD)].Texture = contentManager.Load<Texture2D>("Pine.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.REDWOOD)].Texture = contentManager.Load<Texture2D>("redwood.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.SPRUCE_WOOD)].Texture = contentManager.Load<Texture2D>("spruce.png");
            _voxelTypeRenderingData[GetRenderingDataIndex(VoxelType.TEAK_WOOD)].Texture = contentManager.Load<Texture2D>("teak.png");
        }

        private static int GetRenderingDataIndex(VoxelType voxel)
        {
            return (int)voxel - 1;
        }

        /// <summary>
        /// setups settings for a new map and clears all instance buffers
        /// </summary>
        public void Reset(Map map, Vector3 lightDirection)
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
                    if(_voxelTypeRenderingData[GetRenderingDataIndex(voxel)].InstanceDataRAM.Count < TypeInformation.GetMaxNumberOfVoxels(voxel)-1)
                        _voxelTypeRenderingData[GetRenderingDataIndex(voxel)].InstanceDataRAM.Add(posCode);
                }
            }

            foreach(var data in _voxelTypeRenderingData)
                data.UpdateGPUInstanceBuffer();

            // set vertex scaling
            var globalMapInfoCB = _voxelEffect.ConstantBuffers["GlobalMapInfo"];
            globalMapInfoCB.Parameters["WorldSize"].SetValue(new Int3(map.SizeX, map.SizeY, map.SizeZ));
            lightDirection.Normalize();
            globalMapInfoCB.Parameters["LightDirection"].SetValue(-lightDirection);
            globalMapInfoCB.Parameters["Translation"].SetValue(- new Vector3(map.SizeX, 0.0f, map.SizeZ) * 0.5f);
            globalMapInfoCB.IsDirty = true;
        }

        /// <summary>
        /// updates instancebuffer for the current "voxelsituation" (what ever)
        /// </summary>
        public void Update(IEnumerable<Voxel> removeList, IEnumerable<Voxel> addList)
        {
            // remove voxels
            if( removeList != null )
            foreach (Voxel voxel in removeList)
                _voxelTypeRenderingData[GetRenderingDataIndex(voxel.Type)].InstanceDataRAM.Remove(voxel.PositionCode);

            // add voxels
            if (addList != null)
            foreach (Voxel voxel in addList)
                _voxelTypeRenderingData[GetRenderingDataIndex(voxel.Type)].InstanceDataRAM.Add(voxel.PositionCode);

            // update gpu
            foreach (var data in _voxelTypeRenderingData)
                data.UpdateGPUInstanceBuffer();
        }

        /// <summary>
        /// draw what else
        /// </summary>
        public void Draw(Camera camera, GraphicsDevice graphicsDevice)
        {
            _voxelEffect.Parameters["ViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix);
            _voxelEffect.Parameters["Ambient"].SetValue(0.3f);
            _voxelEffect.Parameters["CameraPosition"].SetValue(camera.Position);

            graphicsDevice.SetRasterizerState(_backfaceCullingState);
            graphicsDevice.SetDepthStencilState(_depthStencilStateState);
            graphicsDevice.SetBlendState(_blendStateOpaque);

            // Setup the vertices
            graphicsDevice.SetVertexBuffer(_cubeVertexBuffer, 0);
            graphicsDevice.SetVertexInputLayout(_vertexInputLayout);

            // render all instances
            for (int i = 0; i < _voxelTypeRenderingData.Length; ++i)
            {
                _voxelEffect.Parameters["ScalingFactor"].SetValue(TypeInformation.GetScalingFactor(_voxelTypeRenderingData[i].Voxel) * 0.5f);
                _voxelEffect.Parameters["VoxelTexture"].SetResource(_voxelTypeRenderingData[i].Texture);


                _voxelEffect.Parameters["SpecularModifier"].SetValue(_voxelTypeRenderingData[i].Voxel == VoxelType.NOBLEROT_FUNGUS ||
                                                                     _voxelTypeRenderingData[i].Voxel == VoxelType.WHITEROT_FUNGUS);
               
                _voxelEffect.CurrentTechnique.Passes[0].Apply();
                graphicsDevice.SetVertexBuffer(1, _voxelTypeRenderingData[i].InstanceBuffer);
                graphicsDevice.DrawInstanced(PrimitiveType.TriangleList, _cubeVertexBuffer.ElementCount, _voxelTypeRenderingData[i].InstanceDataRAM.Count, 0, 0);
            }

            graphicsDevice.SetVertexBuffer<int>(1, (Buffer<int>)null);
        }

        public void DrawGhost(Camera camera, GraphicsDevice graphicsDevice, VoxelType voxel, Int32 levelPositionCode)
        {
            _voxelEffect.Parameters["ViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix);
            _voxelEffect.Parameters["VoxelTexture"].SetResource(_voxelTypeRenderingData[GetRenderingDataIndex(voxel)].Texture);
            _voxelEffect.Parameters["Transparency"].SetValue(0.7f);
            _voxelEffect.Parameters["Ambient"].SetValue(2.0f);
            _voxelEffect.Parameters["ScalingFactor"].SetValue(TypeInformation.GetScalingFactor(voxel) * 0.5f);

            _singleInstanceBuffer.SetDynamicData(graphicsDevice, (ptr) => System.Runtime.InteropServices.Marshal.Copy(
                                                                 new Int32[] { levelPositionCode }, 0, ptr, 1));

            graphicsDevice.SetRasterizerState(_noneCullingState);
            graphicsDevice.SetDepthStencilState(_depthStencilStateState);
            graphicsDevice.SetBlendState(_blendStateTransparent);

            // Setup the vertices
            graphicsDevice.SetVertexBuffer(0, _cubeVertexBuffer);
            graphicsDevice.SetVertexBuffer(1, _singleInstanceBuffer);
            graphicsDevice.SetVertexInputLayout(_vertexInputLayout);

            _voxelEffect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawInstanced(PrimitiveType.TriangleList, _cubeVertexBuffer.ElementCount, 1, 0, 0);
        }
    }
}
