namespace ObjParser
{
    public readonly struct Vertex
    {
        public readonly int VertexIndex;
        public readonly int TextureIndex;
        public readonly int NormalIndex;
        public Vertex(int vertexIndex, int textureIndex, int normalIndex)
        {
            VertexIndex = vertexIndex;
            TextureIndex = textureIndex;
            NormalIndex = normalIndex;
        }
    }
}