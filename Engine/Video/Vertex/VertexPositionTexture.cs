
using Silk.NET.DXGI;

namespace Engine.Video
{
    public struct VertexPositionTexture(Vector2 pos, Vector2 tCoords)
    {
            
        public Vector2 Position = pos;
        public Vector2 TCoords = tCoords;

        public unsafe static int SizeInBytes() => sizeof(VertexPositionColorTexture);

        public static InputElement[] GetInputElements()
        {
            return
            [
                new InputElement("POSITION", 0, Format.FormatR32G32Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.FormatR32G32Float, 8, 0),
            ];
        }
    }
}
