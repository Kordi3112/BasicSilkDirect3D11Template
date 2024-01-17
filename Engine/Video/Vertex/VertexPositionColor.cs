using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Engine.Video
{
    public struct VertexPositionColor
    {
        public Vector2 Position;
        public Vector4 Color;

        public VertexPositionColor(Vector2 pos, Vector4 col)
        {
            Position = pos;
            Color = col;
        }


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
