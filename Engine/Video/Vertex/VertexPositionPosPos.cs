using Silk.NET.DXGI;

namespace Engine.Video
{
    public struct VertexPositionPosPos(Vector2 position1, Vector2 position2, Vector2 position3)
    {
        public Vector2 Position1 = position1;
        public Vector2 Position2 = position2;
        public Vector2 Position3 = position3;

        public static InputElement[] GetInputElements()
        {
            return
            [
                new InputElement("POSITION", 0, Format.FormatR32G32Float, 0, 0),
                new InputElement("POSITION", 1, Format.FormatR32G32Float, 8, 0),
                new InputElement("POSITION", 2, Format.FormatR32G32Float, 16, 0),
            ];
        }
    }
}
