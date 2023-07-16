using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LuckyBlock
{
    internal class MaterialUtils
    {
        public static Shader MarmosetUber { get; private set; } = Shader.Find("MarmosetUBER");

        public static void ApplySNShaders(GameObject prefab, float shininess = 8f, float specularInt = 1f, float glowStrength = 1f)
        {
            var renderers = prefab.GetComponentsInChildren<Renderer>(true);
            var newShader = MarmosetUber;
            for (var i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] is ParticleSystemRenderer)
                {
                    continue;
                }
                for (var j = 0; j < renderers[i].materials.Length; j++)
                {
                    var material = renderers[i].materials[j];
                    var specularTexture = material.GetTexture("_SpecGlossMap");
                    var emissionTexture = material.GetTexture("_EmissionMap");
                    var emissionColor = material.GetColor(ShaderPropertyID._EmissionColor);
                    material.shader = newShader;

                    material.DisableKeyword("_SPECGLOSSMAP");
                    material.DisableKeyword("_NORMALMAP");
                    if (specularTexture != null)
                    {
                        material.SetTexture("_SpecTex", specularTexture);
                        material.SetFloat("_SpecInt", specularInt);
                        material.SetFloat("_Shininess", shininess);
                        material.EnableKeyword("MARMO_SPECMAP");
                        material.SetColor("_SpecColor", new Color(1f, 1f, 1f, 1f));
                        material.SetFloat("_Fresnel", 0.24f);
                        material.SetVector("_SpecTex_ST", new Vector4(1.0f, 1.0f, 0.0f, 0.0f));
                    }
                    if (material.IsKeywordEnabled("_EMISSION"))
                    {
                        material.EnableKeyword("MARMO_EMISSION");
                        material.SetFloat(ShaderPropertyID._EnableGlow, 1f);
                        material.SetTexture(ShaderPropertyID._Illum, emissionTexture);
                        material.SetColor(ShaderPropertyID._GlowColor, emissionColor);
                        material.SetFloat(ShaderPropertyID._GlowStrength, glowStrength);
                        material.SetFloat(ShaderPropertyID._GlowStrengthNight, glowStrength);
                    }

                    if (material.GetTexture("_BumpMap"))
                    {
                        material.EnableKeyword("MARMO_NORMALMAP");
                    }

                    if (material.name.Contains("Cutout"))
                    {
                        material.EnableKeyword("MARMO_ALPHA_CLIP");
                    }
                    if (material.name.Contains("Transparent"))
                    {
                        material.EnableKeyword("_ZWRITE_ON");
                        material.EnableKeyword("WBOIT");
                        material.SetInt("_ZWrite", 0);
                        material.SetInt("_Cutoff", 0);
                        material.SetFloat("_SrcBlend", 1f);
                        material.SetFloat("_DstBlend", 1f);
                        material.SetFloat("_SrcBlend2", 0f);
                        material.SetFloat("_DstBlend2", 10f);
                        material.SetFloat("_AddSrcBlend", 1f);
                        material.SetFloat("_AddDstBlend", 1f);
                        material.SetFloat("_AddSrcBlend2", 0f);
                        material.SetFloat("_AddDstBlend2", 10f);
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack | MaterialGlobalIlluminationFlags.RealtimeEmissive;
                        material.renderQueue = 3101;
                        material.enableInstancing = true;
                    }
                }
            }
        }
    }
}
