using UnityEngine;

namespace Planet
{
    public static class MeshGenerator
    {
        public static Mesh Generate (PlanetSettings settings)
        {
            Mesh mesh = IcoSphereGenerator.Generate(settings.General.IcoLevel, true);

            Vector3[] vertices = mesh.vertices;
            float[] normalNoise = GetNormaNoise(vertices, settings.Landscape);
            mesh.colors = GetColorsFromNoise(vertices, normalNoise);
            for (int i = 0; i < vertices.Length; i++)
            {
                float noise = normalNoise[i]*settings.Landscape.Strenght;
                float range = Mathf.Max(settings.General.Liquid, noise);
                vertices[i] *= settings.General.Radius+range;
            }
            mesh.vertices = vertices;
            return mesh;
        }

        private static float[] GetNormaNoise (Vector3[] vertices, LandscapeSettings settings)
        {
            Noise noiseGenerator = new Noise(settings.Seed);
            float[] noise = new float[vertices.Length];
            float StrenghtSum = 0;
            foreach (NoiseLayerSettings layer in settings.Layers)
                if (layer.enable)
                    StrenghtSum += layer.weightFactor;
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;
            for (int i = 0; i < vertices.Length; i++)
            {
                float noiseValue = 0;
                foreach (NoiseLayerSettings layer in settings.Layers)
                    if (layer.enable)
                    {
                        Vector3 point = (vertices[i]+settings.Offset+layer.offset)*layer.roughness;
                        float value = noiseGenerator.Evaluate(point);
                        value = Mathf.Abs(value)*0.5f+0.5f; // -1 > 1   =>   0 > 1
                        if (layer.sharpness > 1)
                            value = Mathf.Pow(value, layer.sharpness);
                        if (layer.flip)
                            value = 1-value;
                        noiseValue += value*layer.weightFactor;
                    }
                noise[i] = noiseValue;
                if (min > noiseValue)
                    min = noiseValue;
                if (max < noiseValue)
                    max = noiseValue;
            }

            for (int i = 0; i < vertices.Length; i++)
                noise[i] = Mathf.InverseLerp(min, max, noise[i]);

            return noise;
        }

        private static Color[] GetColorsFromNoise (Vector3[] vertices, float[] noise)
        {
            Color[] colors = new Color[noise.Length];
            for (int i = 0; i < noise.Length; i++)
            {
                float landscape = noise[i];
                float north = 1-Mathf.Acos(vertices[i].y)/Mathf.PI; // south to north
                colors[i] = new Color(landscape, north, 0);
            }
            return colors;
        }
    }
}
