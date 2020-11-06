using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.Experimental.Rendering.Universal
{
    internal static class ShadowRendering
    {
        private static readonly int k_LightPosID = Shader.PropertyToID("_LightPos");
        private static readonly int k_SelfShadowingID = Shader.PropertyToID("_SelfShadowing");
        private static readonly int k_ShadowStencilGroupID = Shader.PropertyToID("_ShadowStencilGroup");
        private static readonly int k_ShadowIntensityID = Shader.PropertyToID("_ShadowIntensity");
        private static readonly int k_ShadowVolumeIntensityID = Shader.PropertyToID("_ShadowVolumeIntensity");
        private static readonly int k_ShadowRadiusID = Shader.PropertyToID("_ShadowRadius");


        private static Material GetProjectedShadowMaterial(this Renderer2DData rendererData)
        {
            //if (rendererData.projectedShadowMaterial == null)
                rendererData.projectedShadowMaterial = CoreUtils.CreateEngineMaterial(rendererData.projectedShadowShader);

            return rendererData.projectedShadowMaterial;
        }

        private static Material GetStencilOnlyShadowMaterial(this Renderer2DData rendererData)
        {
            //if (rendererData.stencilOnlyShadowMaterial == null)
                rendererData.stencilOnlyShadowMaterial = CoreUtils.CreateEngineMaterial(rendererData.projectedShadowShader);

            return rendererData.stencilOnlyShadowMaterial;
        }

        private static Material GetSpriteSelfShadowMaterial(this Renderer2DData rendererData)
        {
            //if (rendererData.spriteSelfShadowMaterial == null)
            {
                Material material = CoreUtils.CreateEngineMaterial(rendererData.spriteShadowShader);
                rendererData.spriteSelfShadowMaterial = material;
            }
                
            return rendererData.spriteSelfShadowMaterial;
        }

        private static Material GetSpriteUnshadowMaterial(this Renderer2DData rendererData)
        {
            //if (rendererData.spriteUnshadowMaterial == null)
            {
                Material material = CoreUtils.CreateEngineMaterial(rendererData.spriteUnshadowShader);
                rendererData.spriteUnshadowMaterial = material;
            }

            return rendererData.spriteUnshadowMaterial;
        }


        private static void CreateShadowRenderTexture(IRenderPass2D pass, RenderingData renderingData, CommandBuffer cmd)
        {
            var renderTextureScale = Mathf.Clamp(pass.rendererData.lightRenderTextureScale, 0.01f, 1.0f);
            var width = (int)(renderingData.cameraData.cameraTargetDescriptor.width * renderTextureScale);
            var height = (int)(renderingData.cameraData.cameraTargetDescriptor.height * renderTextureScale);

            var descriptor = new RenderTextureDescriptor(width, height);
            descriptor.useMipMap = false;
            descriptor.autoGenerateMips = false;
            descriptor.depthBufferBits = 24;
            descriptor.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
            descriptor.msaaSamples = 1;
            descriptor.dimension = TextureDimension.Tex2D;

            cmd.GetTemporaryRT(pass.rendererData.shadowsRenderTarget.id, descriptor, FilterMode.Bilinear);
        }

        public static void RenderShadows(IRenderPass2D pass, RenderingData renderingData, CommandBuffer cmdBuffer, int layerToRender, Light2D light, float shadowIntensity, RenderTargetIdentifier renderTexture, RenderTargetIdentifier depthTexture)
        {
            cmdBuffer.SetGlobalFloat(k_ShadowIntensityID, 1 - light.shadowIntensity);
            cmdBuffer.SetGlobalFloat(k_ShadowVolumeIntensityID, 1 - light.shadowVolumeIntensity);

            if (shadowIntensity > 0)
            {
                CreateShadowRenderTexture(pass, renderingData, cmdBuffer);

                cmdBuffer.SetRenderTarget(pass.rendererData.shadowsRenderTarget.Identifier(), RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
                cmdBuffer.ClearRenderTarget(true, true, Color.black);

                var shadowRadius = 1.42f * light.boundingSphere.radius;

                cmdBuffer.SetGlobalVector(k_LightPosID, light.transform.position);
                cmdBuffer.SetGlobalFloat(k_ShadowRadiusID, shadowRadius);

                var spriteSelfShadowMaterial = pass.rendererData.GetSpriteSelfShadowMaterial();
                var spriteUnshadowMaterial = pass.rendererData.GetSpriteUnshadowMaterial();
                var projectedShadowMaterial = pass.rendererData.GetProjectedShadowMaterial();
                var stencilOnlyShadowMaterial = pass.rendererData.GetStencilOnlyShadowMaterial();

                var shadowCasterGroups = ShadowCasterGroup2DManager.shadowCasterGroups;
                if (shadowCasterGroups != null && shadowCasterGroups.Count > 0)
                {
                    for (var group = 0; group < shadowCasterGroups.Count; group++)
                    {
                        var shadowCasterGroup = shadowCasterGroups[group];
                        var shadowCasters = shadowCasterGroup.GetShadowCasters();

                        if (shadowCasters != null)
                        {
                            // Draw the projected shadows
                            for(int shadowCasterIndex=0; shadowCasterIndex < shadowCasters.Count; shadowCasterIndex++)
                            {
                                var shadowCaster = shadowCasters[shadowCasterIndex];
                                if(shadowCaster.castsShadows)
                                {
                                    // If we cast shadows draw the projected shadows
                                    cmdBuffer.DrawMesh(shadowCaster.mesh, shadowCaster.transform.localToWorldMatrix, projectedShadowMaterial, 0, 0);
                                }

                            }

                            // Draw the sprites
                            for (int shadowCasterIndex = 0; shadowCasterIndex < shadowCasters.Count; shadowCasterIndex++)
                            {
                                var shadowCaster = shadowCasters[shadowCasterIndex];
                                if (shadowCaster.useRendererSilhouette)
                                {
                                    var renderer = shadowCaster.GetComponent<Renderer>();
                                    if (renderer != null)
                                    {
                                        if (shadowCaster.selfShadows)
                                            cmdBuffer.DrawRenderer(renderer, spriteSelfShadowMaterial);
                                        else
                                            cmdBuffer.DrawRenderer(renderer, spriteUnshadowMaterial);
                                        
                                    }
                                }
                                else
                                {
                                    if (shadowCaster.selfShadows)
                                        cmdBuffer.DrawMesh(shadowCaster.mesh, shadowCaster.transform.localToWorldMatrix, spriteSelfShadowMaterial);
                                    else
                                        cmdBuffer.DrawMesh(shadowCaster.mesh, shadowCaster.transform.localToWorldMatrix, spriteUnshadowMaterial);
                                }
                            }

                            // Update the stencil buffer
                            for (int shadowCasterIndex = 0; shadowCasterIndex < shadowCasters.Count; shadowCasterIndex++)
                            {
                                var shadowCaster = shadowCasters[shadowCasterIndex];
                                if (shadowCaster.castsShadows)
                                {
                                    // If we cast shadows draw update the stencil buffer. Use stencil only pass
                                    cmdBuffer.DrawMesh(shadowCaster.mesh, shadowCaster.transform.localToWorldMatrix, stencilOnlyShadowMaterial, 0, 1);
                                }
                            }
                        }
                    }
                }

                cmdBuffer.ReleaseTemporaryRT(pass.rendererData.shadowsRenderTarget.id);
                cmdBuffer.SetRenderTarget(renderTexture, depthTexture);
            }
        }
    }
}
