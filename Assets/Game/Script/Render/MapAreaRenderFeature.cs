using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.Map;
using Game.Script.Setting;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Game.Script.Render
{
    public class MapAreaRenderFeature : ScriptableRendererFeature
    {
    
        class CustomRenderPass : ScriptableRenderPass
        {
            // This method is called before executing the render pass.
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target setup and clearing happens in a performant manner.
        
            private Mesh _gridMesh;
            private Mesh _blockMesh;
            public Material gridMaterial;
            public Material blockMaterial;
            public float lineSize = 0.1f;
            private int _curGridX = 0;
            private int _curGridY = 0;
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
            }
            

            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            
            MapBk GetMapBk(Scene scene, CameraType cameraType)
            {

                var bk = MapBkManager.Instance.FindMapBk(scene);
                
                return bk;
            }
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                MapBk mapBk = GetMapBk(renderingData.cameraData.camera.scene, renderingData.cameraData.cameraType);

                if (mapBk == null)
                {
                    return;
                }

                if (renderingData.cameraData.cameraType == CameraType.Preview)
                {
                    return;
                }
                

                if (renderingData.cameraData.camera.cullingMask == 32)
                {
                    return;
                }
                
        
                if (null != gridMaterial && GameSetting.Instance.ShowGrid)
                {
                    CreateGridMesh(mapBk);

                    if (null != _gridMesh)
                    {
                        CommandBuffer cmd = CommandBufferPool.Get();
                
                        cmd.DrawMesh(_gridMesh, Matrix4x4.identity, gridMaterial, 0);
                        context.ExecuteCommandBuffer(cmd);
                        CommandBufferPool.Release(cmd);
                    }
       
                }

                if (null != blockMaterial && GameSetting.Instance.ShowBlock)
                {
                    CreatBlockMesh(mapBk);
                    if (null != _blockMesh)
                    {
                        CommandBuffer cmd = CommandBufferPool.Get();
                
                        cmd.DrawMesh(_blockMesh, Matrix4x4.identity, blockMaterial, 0);
                        context.ExecuteCommandBuffer(cmd);
                        CommandBufferPool.Release(cmd);
                    }
                    
                }
           
            
            }

            void CreatBlockMesh(MapBk bk)
            {
                bool useBk = true;
                if (Application.isPlaying)
                {
                    if (Common.Game.Instance.Mode == GameMode.Hall
                        || Common.Game.Instance.Mode == GameMode.Client
                        || Common.Game.Instance.Mode == GameMode.Host
                       )
                    {
                        useBk = false;
                    }
                }

                if (useBk)
                {
                    CreateMapBkBlockMesh(bk);
                }
                else
                {
                    CreateMapAreaBlockMesh(bk);
                }
            }

            void CreateMapAreaBlockMesh(MapBk bk)
            {
                if (null == _blockMesh)
                {
                    _blockMesh = new Mesh();
                }

                var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
                _blockMesh.Clear();
                int index = 0;
                List<int> indices = new();
                List<Vector3> vertices = new();
                foreach (var areaMap in mapSubsystem.Areas)
                {
                    var area = areaMap.Value;

                    if (!area.Blocked)
                    {
                        continue;
                    }
                    var p = mapSubsystem.AreaIndexToGrid(areaMap.Key);
                    var position = GameUtil.ConvertPointToWorldPosition(p, bk.Offset, bk.xGridSize, bk.zGridSize);
                    var v0 = position - new Vector3( bk.xGridSize * 0.5f, 0, bk.zGridSize * 0.5f);
                    vertices.Add(v0);
                    indices.Add(index);
                    index++;
                    
                    var v1 = position + new Vector3( bk.xGridSize * 0.5f, 0, -bk.zGridSize * 0.5f);
                    vertices.Add(v1);
                    indices.Add(index);
                    index++;
                    
                    var v2 = position + new Vector3(bk.xGridSize * 0.5f, 0,  bk.zGridSize * 0.5f);
                    vertices.Add(v2);
                    indices.Add(index);
                    index++;
                
                    var v3 = position + new Vector3(-bk.xGridSize * 0.5f , 0, bk.zGridSize * 0.5f);
                    vertices.Add(v3);
                    indices.Add(index);
                    index++;
                }
                _blockMesh.SetVertices(vertices);
               
                _blockMesh.SetIndices(indices, MeshTopology.Quads, 0);
            }

            void CreateMapBkBlockMesh(MapBk bk)
            {
                if (null == _blockMesh)
                {
                    _blockMesh = new Mesh();
                }
                _blockMesh.Clear();
                int index = 0;
                List<int> indices = new();
                List<Vector3> vertices = new();
                foreach (var block in bk.blocks)
                {
                    var p = bk.BlockToGrid(block);
                    var position = GameUtil.ConvertPointToWorldPosition(p, bk.Offset, bk.xGridSize, bk.zGridSize);
                    var v0 = position - new Vector3( bk.xGridSize * 0.5f, 0, bk.zGridSize * 0.5f);
                    vertices.Add(v0);
                    indices.Add(index);
                    index++;
                    
                    var v1 = position + new Vector3( bk.xGridSize * 0.5f, 0, -bk.zGridSize * 0.5f);
                    vertices.Add(v1);
                    indices.Add(index);
                    index++;
                    
                    var v2 = position + new Vector3(bk.xGridSize * 0.5f, 0,  bk.zGridSize * 0.5f);
                    vertices.Add(v2);
                    indices.Add(index);
                    index++;
                
                    var v3 = position + new Vector3(-bk.xGridSize * 0.5f , 0, bk.zGridSize * 0.5f);
                    vertices.Add(v3);
                    indices.Add(index);
                    index++;
                }
                _blockMesh.SetVertices(vertices);
               
                _blockMesh.SetIndices(indices, MeshTopology.Quads, 0);
            }
            void CreateGridMesh(MapBk bk)
            {
                if (null == _gridMesh)
                {
                    _gridMesh = new Mesh();
                }
                _gridMesh.Clear();
                

                if (_curGridX == bk.xGridNum && _curGridY == bk.zGridNum)
                {
                    return;
                }
            
                int index = 0;
                Vector3 start = bk.transform.position;
                List<int> indices = new();
                List<Vector3> vertices = new();
                for (int x = 0; x <= bk.xGridNum; x++)
                {
                    var v0 = start + new Vector3(x * bk.xGridSize, 0, 0);
                    vertices.Add(v0);
                    indices.Add(index);
                    index++;
                    
                    var v1 = start + new Vector3(x * bk.xGridSize, 0, bk.zGridNum * bk.zGridSize);
                    vertices.Add(v1);
                    indices.Add(index);
                    index++;
                    
                    var v2 = start + new Vector3(x * bk.xGridSize + lineSize, 0, bk.zGridNum * bk.zGridSize);
                    vertices.Add(v2);
                    indices.Add(index);
                    index++;
                
                    var v3 = start + new Vector3(x * bk.xGridSize + lineSize, 0, 0);
                    vertices.Add(v3);
                    indices.Add(index);
                    index++;
                
          
                }
            
                for (int y = 0; y <= bk.zGridNum; y++)
                {
                    var v0 = start + new Vector3(0, 0, y * bk.zGridSize);
                    vertices.Add(v0);
                    indices.Add(index);
                    index++;
                    
                    var v1 = start + new Vector3(bk.xGridNum * bk.xGridSize , 0, y * bk.xGridSize);
                    vertices.Add(v1);
                    indices.Add(index);
                    index++;

                    
                    var v2 = start + new Vector3(bk.xGridNum * bk.xGridSize , 0, y * bk.zGridSize + lineSize );
                    vertices.Add(v2);
                    indices.Add(index);
                    index++;
                
                    var v3 = start + new Vector3(0, 0, y * bk.zGridSize + lineSize);
                    vertices.Add(v3);
                    indices.Add(index);
                    index++;
          
                }

                _gridMesh.SetVertices(vertices);
               
                _gridMesh.SetIndices(indices, MeshTopology.Quads, 0);


            }
        

            // Cleanup any allocated resources that were created during the execution of this render pass.
            public override void OnCameraCleanup(CommandBuffer cmd)
            {
            }
        }

        CustomRenderPass _mScriptablePass;

       public Material gridMaterial;
       public Material blockMaterial;
        public float lineSize = 0.1f;
        /// <inheritdoc/>
        public override void Create()
        {
            _mScriptablePass = new CustomRenderPass
            {
                gridMaterial = gridMaterial,
                blockMaterial = blockMaterial,
                lineSize = lineSize,
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
        }



        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_mScriptablePass);
        }
    }
}


