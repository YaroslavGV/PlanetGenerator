using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planet
{
    [CreateAssetMenu(fileName = "Planet Settings", menuName = "Planet/Settings")]
    public class PlanetSettings : ScriptableObject
    {
        [SerializeField] private GeneralSettings _general;
        [SerializeField] private LandscapeSettings _landscape;
        [SerializeField] private SuperficiesSettings _superficies;

        public GeneralSettings General => _general;
        public LandscapeSettings Landscape => _landscape;
        public SuperficiesSettings Superficies => _superficies;
    }

    [Serializable]
    public class GeneralSettings
    {
        [Range(0, 6)]
        [SerializeField] private int _icoLevel = 3;
        [SerializeField] private float _radius = 1;
        [Tooltip("Offset from Radius")]
        [SerializeField] private float _liquid = 0.2f;

        public int IcoLevel => _icoLevel;
        public float Radius => _radius;
        public float Liquid => _liquid;
    }

    [Serializable]
    public class LandscapeSettings
    {
        [SerializeField] private int _seed = 0;
        [SerializeField] private float _strenght = 1;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private NoiseLayerSettings[] _layers = new[]
        {
            new NoiseLayerSettings(1, .75f, 1, true),
            new NoiseLayerSettings(0.5f, 1.5f, 1),
            new NoiseLayerSettings(0.25f, 3, 5),
            new NoiseLayerSettings(0.125f, 5, 1)
        };

        public int Seed => _seed;
        public float Strenght => _strenght;
        public Vector3 Offset => _offset;
        public IEnumerable<NoiseLayerSettings> Layers => _layers;

        [ContextMenu("Roll Seed")]
        public void RollSeed () => _seed = UnityEngine.Random.Range(0, 100000);

        public float GetTotalStrenght () 
        {
            float layersStrenght = 0;
            foreach (NoiseLayerSettings layer in _layers)
                if (layer.enable)
                    layersStrenght += layer.weightFactor;
            return layersStrenght*Strenght;
        }
    }

    [Serializable]
    public struct NoiseLayerSettings
    {
        public bool enable;
        public float weightFactor;
        public Vector3 offset;
        public float roughness;
        [Range(1, 5)]    
        public float sharpness;
        public bool flip;

        public NoiseLayerSettings (float weightFactor, float roughness, float sharpness, bool flip = false) : this()
        {
            enable = true;
            this.weightFactor = weightFactor;
            this.roughness = roughness;
            this.sharpness = sharpness;
            this.flip = flip;
        }
    }

    [Serializable]
    public class SuperficiesSettings
    {
        [SerializeField] private bool _enable = true;
        [SerializeField] private Vector2Int _textureSize = new Vector2Int(512, 512);
        [SerializeField] private Gradient _height = 
            new Gradient() 
            { 
                colorKeys = new[] 
                {
                    new GradientColorKey(Color.HSVToRGB(40/360f, 1, 0.9f), 0),
                    new GradientColorKey(Color.HSVToRGB(120/360f, 0.85f, 0.6f), 0.05f),
                    new GradientColorKey(Color.HSVToRGB(120/360f, 0.8f, 0.4f), 0.2f),
                    new GradientColorKey(Color.HSVToRGB(30/360f, 0.8f, 0.4f), 0.5f),
                    new GradientColorKey(Color.HSVToRGB(20/360f, 0.2f, 0.6f), 1)
                }
            };
        [SerializeField] private Gradient _depth =
            new Gradient()
            {
                colorKeys = new[]
                {
                    new GradientColorKey(Color.HSVToRGB(220/360f, 0.8f, 0.4f), 0),
                    new GradientColorKey(Color.HSVToRGB(220/360f, 0.8f, 1), 1)
                }
            };
        [SerializeField] private Gradient _climateHeight =
            new Gradient()
            {
                colorKeys = new[]
                {
                    new GradientColorKey(Color.HSVToRGB(0/360f, 0, 0), 0),
                    new GradientColorKey(Color.HSVToRGB(60/360f, 1, 1), 0.3f),
                    new GradientColorKey(Color.HSVToRGB(60/360f, 1, 1), 0.7f),
                    new GradientColorKey(Color.HSVToRGB(240/360f, 0.2f, 1), 0.75f),
                    new GradientColorKey(Color.HSVToRGB(240/360f, 0.2f, 1), 1)
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(0, 0),
                    new GradientAlphaKey(0, 0.3f),
                    new GradientAlphaKey(0.5f, 0.5f),
                    new GradientAlphaKey(0, 0.7f),
                    new GradientAlphaKey(0, 0.75f),
                    new GradientAlphaKey(0.75f, 0.85f),
                    new GradientAlphaKey(1, 1)
                }

            };
        [SerializeField] private Gradient _climateDepth =
            new Gradient()
            {
            	colorKeys = new[]
            	{
            		new GradientColorKey(Color.HSVToRGB(0/360f, 0, 0), 0),
            		new GradientColorKey(Color.HSVToRGB(240/360f, 0.2f, 1), 0.75f),
            		new GradientColorKey(Color.HSVToRGB(240/360f, 0.2f, 1), 1)
            	},
            	alphaKeys = new[]
            	{
            		new GradientAlphaKey(0, 0),
            		new GradientAlphaKey(0, 0.75f),
            		new GradientAlphaKey(1, 0.85f),
            		new GradientAlphaKey(1, 1)
            	}
            };

        public bool Enable => _enable;
        public Vector2Int TextureSize => _textureSize;
        public Gradient Height => _height;
        public Gradient Depth => _depth;
        public Gradient ClimateHeight => _climateHeight;
        public Gradient ClimateDepth => _climateDepth;

        [ContextMenu("LogGradients")]
        public void LogGradients ()
        {
            Debug.Log(_height.GetTextConstructor("Height"));
            Debug.Log(_depth.GetTextConstructor("Depth"));
            Debug.Log(_climateHeight.GetTextConstructor("ClimatHeight"));
            Debug.Log(_climateDepth.GetTextConstructor("ClimatDepth"));
        }
    }
}
