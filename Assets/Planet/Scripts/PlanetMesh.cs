using UnityEngine;

namespace Planet
{
    [RequireComponent(typeof(MeshFilter))]
    public class PlanetMesh : MonoBehaviour, IPlanetView
    {
        public void Generate (PlanetSettings settings)
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            filter.sharedMesh = MeshGenerator.Generate(settings);
        }

        public void Clear ()
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            if (filter.sharedMesh != null)
            {
                if (Application.isPlaying)
                    Destroy(filter.sharedMesh);
                else
                    DestroyImmediate(filter.sharedMesh);
            }
        }
    }
}
