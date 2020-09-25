namespace ObjParser
{
    public class ObjFace
    {
        public int Material { get; }
        public int SmoothGroup { get; }
        public Vertex[] Vertices { get; }  
        public ObjFace(int material, int smoothGroup, in Vertex[] vertices)
        {
            Material = material;
            SmoothGroup = smoothGroup;
            Vertices = vertices;
        }
    }
}