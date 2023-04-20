using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MeshTest
{
    [CustomEditor(typeof(MeshDebug))]
    public class MeshDebugEditor : Editor
    {
        private MeshDebug _target;
        private MeshFilter _filter;

        private void OnEnable ()
        {
            _target = target as MeshDebug;
            _filter = _target.GetComponent<MeshFilter>();
        }

        private void OnSceneGUI ()
        {
            if (_filter.sharedMesh == null)
                return;

            List<Drawer> drawers = new List<Drawer>();
            if (_target.VerticesDraw.enable)
                drawers.Add(new VerticesDrawer(_target.VerticesDraw.color, _target.VerticesDraw.radius));
            if (_target.VerticleDraw.enable)
                drawers.Add(new VertexDrawer(_target.VerticleDraw.color, _target.VerticleDraw.radius, _target.VerticleDraw.index));
            if (_target.NormalsDraw.enable)
                drawers.Add(new NormalDrawer(_target.NormalsDraw.color, _target.NormalsDraw.lenght));

            if (drawers.Count > 0)
                Draw(drawers);
        }

        private void Draw (List<Drawer> drawers)
        {
            Transform editorCamera = SceneView.currentDrawingSceneView.camera.transform;
            Mesh mesh = _filter.sharedMesh;

            bool[] isFront = new bool[mesh.vertexCount];
            if (_target.DrawBack)
            {
                for (int i = 0; i < mesh.vertexCount; i++)
                    isFront[i] = true;
            } 
            else
            {
                for (int i = 0; i < mesh.vertexCount; i++)
                    isFront[i] = IsFront(mesh.vertices[i], mesh.normals[i], editorCamera.position);
            }

            foreach (Drawer drawer in drawers)
                drawer.Draw(mesh.vertices, mesh.normals, isFront);
        }

        private bool IsFront (Vector3 vertex, Vector3 normal, Vector3 camera)
        {
            return Vector3.Dot((vertex-camera).normalized, normal) < 0;
        }

        private abstract class Drawer
        {
            public abstract void Draw (Vector3[] vertex, Vector3[] normal, bool[] isFront);
        }

        private class VerticesDrawer : Drawer
        {
            private readonly Color _color;
            private readonly float _radius;

            public VerticesDrawer (Color color, float radius)
            {
                _color = color;
                _radius = radius;
            }

            public override void Draw (Vector3[] vertex, Vector3[] normal, bool[] isFront)
            {
                for (int i = 0; i < vertex.Length; i++)
                {
                    if (isFront[i])
                    {
                        Handles.color = _color;
                        Handles.DrawSolidDisc(vertex[i], normal[i], _radius);
                    }
                }
            }
        }

        private class VertexDrawer : Drawer
        {
            private readonly Color _color;
            private readonly float _radius;
            private readonly int _index;

            public VertexDrawer (Color color, float radius, int index)
            {
                _color = color;
                _radius = radius;
                _index = index;
            }

            public override void Draw (Vector3[] vertex, Vector3[] normal, bool[] isFront)
            {
                if (_index > -1 && _index < vertex.Length)
                {
                    Handles.color = _color;
                    Handles.DrawSolidDisc(vertex[_index], normal[_index], _radius);
                }
            }
        }

        private class NormalDrawer : Drawer
        {
            private readonly Color _color;
            private readonly float _lenght;

            public NormalDrawer (Color color, float lenght)
            {
                _color = color;
                _lenght = lenght;
            }

            public override void Draw (Vector3[] vertex, Vector3[] normal, bool[] isFront)
            {
                for (int i = 0; i < vertex.Length; i++)
                {
                    if (isFront[i])
                    {
                        Handles.color = _color;
                        Handles.DrawLine(vertex[i], vertex[i]+normal[i]*_lenght);
                    }
                }
            }
        }
    }
}
