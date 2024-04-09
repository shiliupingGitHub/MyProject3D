using System.Collections.Generic;
using Game.Script.Map;
using Game.Script.Setting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
            public Material drawMaterial;
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

            private MapBk _curBk = null;
            MapBk GetMapBk(Scene scene, CameraType cameraType)
            {
         
                if (null != _curBk)
                {
                    return _curBk;
                }

                if (cameraType == CameraType.Game || !scene.IsValid())
                {
                    _curBk = Object.FindObjectOfType<MapBk>();
                }
                else
                {
                    if (!scene.IsValid())
                    {
                        return null;
                    }
                    var roots = scene.GetRootGameObjects();

                    foreach (var root in roots)
                    {
                        var bk = root.GetComponent<MapBk>();

                        if (null != bk)
                        {
                            _curBk = bk;
                            break;
                        }
                    }
                }
                
                return _curBk;
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

                if (!GameSetting.Instance.ShowGrid)
                {
                    return;
                }

                if (renderingData.cameraData.camera.cullingMask == 32)
                {
                    return;
                }

                if (null != drawMaterial)
                {
                    CreateMesh(mapBk);

                    if (null != _gridMesh)
                    {
                        CommandBuffer cmd = CommandBufferPool.Get();
                
                        cmd.DrawMesh(_gridMesh, Matrix4x4.identity, drawMaterial, 0);
                        context.ExecuteCommandBuffer(cmd);
                        CommandBufferPool.Release(cmd);
                    }
       
                }
           
            
            }

            void CreateMesh(MapBk bk)
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

        public Material drawMaterial;
        public float lineSize = 0.1f;
        /// <inheritdoc/>
        public override void Create()
        {
            _mScriptablePass = new CustomRenderPass
            {
                drawMaterial = drawMaterial,
                lineSize = lineSize,
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
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


