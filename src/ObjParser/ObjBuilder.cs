using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ObjParser
{
    internal class ObjBuilder
    {
        private IList<Vector3> _vertices = new List<Vector3>();
        private IList<Vector3> _normals = new List<Vector3>();
        private IList<Vector2> _textures = new List<Vector2>();

        private ObjMaterial[] _materials;

        private ObjGroup _currentGroup;

        private readonly IList<ObjGroup> _groups = new List<ObjGroup>();

        private int _smoothGroup;
        private int _currentMaterialIndex;

        public ObjBuilder()
        {
            _currentGroup = new ObjGroup();
        }

        internal void AddPosition(in ReadOnlySpan<string> position)
        {
            if (position.Length != 3)
            {
                throw new FormatException("Vertices must be of format v x y z");
            }
            _vertices.Add(ParseVector3(position));
        }

        internal void AddTexture(in ReadOnlySpan<string> texture)
        {
            if (texture.Length < 2 || texture.Length > 3)
            {
                throw new FormatException("Textures must be of format vt x y [w]");
            }
            _textures.Add(ParseVector2(texture));
        }

        internal void AddNormal(in ReadOnlySpan<string> normal)
        {
            if (normal.Length != 3)
            {
                throw new FormatException("Normals must be of format v x y z");
            }
            _normals.Add(ParseVector3(normal));
        }

        internal void AddGroup(in ReadOnlySpan<string> group)
        {
            _groups.Add(_currentGroup);

            _currentGroup = new ObjGroup(group.Length > 0 ? group[0] : null);
        }

        internal void AddSmoothGroup(in ReadOnlySpan<string> smooth)
        {
            if (smooth.Length != 1)
            {
                throw new FormatException("Smooth must be of format s {off|int}");
            }
            _smoothGroup = smooth[0].Equals("off", StringComparison.OrdinalIgnoreCase) ? 0 : int.Parse(smooth[0]);
        }

        public void AddFace(in ReadOnlySpan<string> face)
        {
            if(face.Length < 3)
            {
                throw new FormatException("Face must be of format f {f1} {f2} {f3} [{f4...}]");
            }
            
            var vertices = new Vertex[face.Length];

            for (var i = 0; i < face.Length; ++i)
            {
                var values = face[0].Split('/', StringSplitOptions.TrimEntries)
                    .Select(int.Parse)
                    .ToArray();
                vertices[i] = new Vertex(values[0], values[1], values[2]);
            }
            
            _currentGroup.AddFace(new ObjFace(_currentMaterialIndex, _smoothGroup, vertices));
        }

        public void UseMaterial(in ReadOnlySpan<string> values)
        {
            if(_materials == null)
            {
                throw new InvalidOperationException("Materials is missing.");
            }
            
            var name = values[0];
            _currentMaterialIndex = Array.FindIndex(_materials, m => m.Name == name);
            if (_currentMaterialIndex == -1)
            {
                throw new InvalidOperationException($"Material {name} could not be found.");
            }
        }

        public void SetMaterials(ObjMaterial[] materials)
        {
            if (_materials != null)
            {
                throw new InvalidOperationException("Materials has already been set.");
            }
            _materials = materials;
        }

        internal WavefrontObject Build()
        {
            return new WavefrontObject(_groups.ToArray(), _materials, _vertices.ToArray(), _normals.ToArray(), _textures.ToArray());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 ParseVector3(in ReadOnlySpan<string> values) => new Vector3(ParseFloat(values[0]), ParseFloat(values[1]), ParseFloat(values[2]));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 ParseVector2(in ReadOnlySpan<string> values) => new Vector2(ParseFloat(values[0]), ParseFloat(values[1]));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ParseFloat(in string value) => float.Parse(value, CultureInfo.InvariantCulture);
    }
}