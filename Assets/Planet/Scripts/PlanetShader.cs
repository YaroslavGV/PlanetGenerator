using System;
using UnityEngine;

namespace Planet
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    public class PlanetShader : MonoBehaviour, IPlanetView
    {
        [SerializeField] private Material _template;
        [Space]
        [SerializeField] private string _liquidID = "_Liquid";
        [SerializeField] private string _gradientID = "_Superficies";
        [SerializeField, HideInInspector] private Texture2D _gradient;
        
        public void Generate (PlanetSettings settings)
        {
            if (_template == null)
            {
                Debug.LogError("Template is missing");
                return;
            }
            
            float liquid = settings.General.Liquid/settings.Landscape.Strenght; // normalized value relative to geology height
            _gradient = BlendGradients(settings.Superficies, liquid);
            
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            Material material = new Material(_template);
            material.SetFloat(_liquidID, liquid);
            material.SetTexture(_gradientID, _gradient);
            renderer.material = material;

            Texture2D txr = new Texture2D(512, 512);
            
            txr.Apply();
            //GenerateTexture(material);
        }

        public void Clear ()
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer.sharedMaterial != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(renderer.sharedMaterial);
                    Destroy(_gradient);
                }
                else
                {
                    DestroyImmediate(renderer.sharedMaterial);
                    DestroyImmediate(_gradient);
                }
            }
        }

        public Texture2D BlendGradients (SuperficiesSettings superficies, float normaLliquid)
        {
            Vector2Int size = superficies.TextureSize;
            if (size.x < 1 || size.y < 1)
                throw new Exception("width and height must be positive values");

            Gradient height = superficies.Height;
            Gradient climateHeight = superficies.ClimateHeight;
            Gradient depth = superficies.Depth;
            Gradient climateDepth = superficies.ClimateDepth;
            Texture2D texture = new Texture2D(size.x, size.y);
            texture.name = "Gradient Texture";
            for (int x = 0; x < size.x; x++)
            {
                float pX = x/(float)size.x;
                if (pX > normaLliquid)
                {
                    float pG = Mathf.InverseLerp(normaLliquid, 1, pX);
                    Color colorH = height.Evaluate(pG);
                    for (int y = 0; y < size.y; y++)
                    {
                        float pY = y/(float)size.y;
                        Color color = Overlay(colorH, climateHeight.Evaluate(pY));
                        texture.SetPixel(x, y, color);
                    }
                }
                else
                {
                    float pG = Mathf.InverseLerp(0, normaLliquid, pX);
                    Color colorH = depth.Evaluate(pG);
                    for (int y = 0; y < size.y; y++)
                    {
                        float pY = y/(float)size.y;
                        Color color = Overlay(colorH, climateDepth.Evaluate(pY));
                        texture.SetPixel(x, y, color);
                    }
                }
            }
            texture.Apply();
            return texture;
        }

        private Color Overlay (Color colorA, Color colorB)
        {
            float a = colorB.a;
            if (a == 1)
                return colorB;
            float r = Mathf.Lerp(colorA.r, colorB.r, a);
            float g = Mathf.Lerp(colorA.g, colorB.g, a);
            float b = Mathf.Lerp(colorA.b, colorB.b, a);
            return new Color(r, g, b);
        }

        [ContextMenu("Generate Texture")]
        public void GenerateTexture ()
        {
            int Resolution = 1024;
            RenderTexture renderTexture = new RenderTexture(Resolution, Resolution, 0);
            renderTexture.Create();
            Material material = GetComponent<MeshRenderer>().sharedMaterial;
            RenderTexture currentTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear(false, true, Color.black, 1.0f);
            material.SetPass(0);
            Graphics.DrawMeshNow(GetComponent<MeshFilter>().sharedMesh, Vector3.zero, Quaternion.identity);
            Texture2D texture = new Texture2D(Resolution, Resolution, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, Resolution, Resolution), 0, 0);
            RenderTexture.active = currentTexture;
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(System.IO.Path.Combine(Application.dataPath, "PlanetTexture.png"), bytes);
            Destroy(material);
            Destroy(texture);
            renderTexture.Release();
        }
    }
}
