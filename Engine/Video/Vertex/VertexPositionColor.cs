using Silk.NET.DXGI;


namespace Engine.Video
{
    public struct VertexPositionColor(Vector2 pos, Vector4 col)
    {
        public Vector2 Position = pos;
        public Vector4 Color = col;

        public static InputElement[] GetInputElements()
        {
            return
            [
                new InputElement("POSITION", 0, Format.FormatR32G32Float, 0, 0),
                new InputElement("COLOR", 0, Format.FormatR32G32B32A32Float, 8, 0),
            ];
        }
    }
}
