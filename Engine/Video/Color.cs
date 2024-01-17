using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Video
{
    internal struct Color
    {
        private static float inv255 = 1.0f / 255.0f;

        byte[] rgba;

        public byte R
        {
            get { return rgba[0]; }
            set { rgba[0] = value; }
        }

        public byte G
        {
            get { return rgba[1]; }
            set { rgba[1] = value; }
        }

        public byte B
        {
            get { return rgba[2]; }
            set { rgba[2] = value; }
        }

        public byte A
        {
            get { return rgba[3]; }
            set { rgba[3] = value; }
        }


        public Color()
        {
            rgba = new byte[4];
        }

        public Color(byte r, byte g, byte b, byte a) => rgba = [r, g, b, a];
        public Color(byte r, byte g, byte b) => rgba = [r, g, b, 255];

        public Color(Color4 color) : this((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255), (byte)(color.A * 255))
        {
            
        }


        public byte[] Get() => rgba;

        public Color4 ToColor4() => new Color4(R * inv255, G * inv255, B * inv255, A * inv255);


        public static Color Red => new Color(255, 0, 0);
        public static Color Green => new Color(0, 255, 0);
        public static Color Blue => new Color(0, 255, 0);
        public static Color White => new Color(255, 255, 255);
    }
}
