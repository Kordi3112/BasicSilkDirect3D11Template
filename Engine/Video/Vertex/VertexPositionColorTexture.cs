using Silk.NET.DXGI;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Video
{
    
    public struct VertexPositionColorTexture(Vector2 pos, Vector4 color, Vector2 tCoords)
    {
        public Vector2 Position = pos;
        public Vector4 Color = color;
        public Vector2 TCoords = tCoords;

        public unsafe static int SizeInBytes() => sizeof(VertexPositionColorTexture);

        public static InputElement[] GetInputElements()
        {
            return
            [
                new InputElement("POSITION", 0, Format.FormatR32G32Float, 0, 0),
                new InputElement("COLOR", 0, Format.FormatR32G32B32A32Float, 8, 0),
                new InputElement("TEXCOORD", 0, Format.FormatR32G32Float, 24, 0),
            ];
        }
    }
}
