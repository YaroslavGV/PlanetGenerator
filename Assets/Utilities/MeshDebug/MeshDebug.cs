using System;
using UnityEngine;

namespace MeshTest
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class MeshDebug : MonoBehaviour
    {
        [SerializeField] private bool _drawBack = false;
        [SerializeField] private VerticesDrawSettings _verticesDraw = VerticesDrawSettings.defaultValues;
        [SerializeField] private VertexDrawSettings _vertexDraw = VertexDrawSettings.defaultValues;
        [SerializeField] private NormalsDrawSettings _normalsDraw = NormalsDrawSettings.defaultValues;
        
        public bool DrawBack => _drawBack;
        public VerticesDrawSettings VerticesDraw => _verticesDraw;
        public VertexDrawSettings VerticleDraw => _vertexDraw;
        public NormalsDrawSettings NormalsDraw => _normalsDraw;
    }

    [Serializable]
    public struct VerticesDrawSettings
    {
        public bool enable;
        public Color color;
        public float radius;

        public static VerticesDrawSettings defaultValues => new VerticesDrawSettings
        {
            enable = true,
            color = new Color(1f, 0.4f, 0, 1f),
            radius = 1/64f
        };
    }

    [Serializable]
    public struct VertexDrawSettings
    {
        public bool enable;
        public int index;
        public Color color;
        public float radius;

        public static VertexDrawSettings defaultValues => new VertexDrawSettings
        {
            enable = true,
            index = 0,
            color = new Color(1.4f, 0, 1, 1f),
            radius = 1/64f
        };
    }

    [Serializable]
    public struct NormalsDrawSettings
    {
        public bool enable;
        public Color color;
        public float lenght;

        public static NormalsDrawSettings defaultValues => new NormalsDrawSettings
        {
            enable = true,
            color = new Color(0, 1f, 0.4f, 1f),
            lenght = 1/4f
        };
    }
}
