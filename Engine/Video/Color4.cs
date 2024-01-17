using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Video
{
    public struct Color4
    {
        float[] rgba;

        public float R 
        {
            get { return rgba[0]; }
            set { rgba[0] = value; }
        }

        public float G
        {
            get { return rgba[1]; }
            set { rgba[1] = value; }
        }

        public float B
        {
            get { return rgba[2]; }
            set { rgba[2] = value; }
        }

        public float A
        {
            get { return rgba[3]; }
            set { rgba[3] = value; }
        }

        public Color4()
        {
            rgba = new float[4];
        }

        public Color4(float r, float g, float b, float a) => rgba = [r, g, b, a];


        public unsafe float* GetPtr()
        {
            fixed (float* ptr = rgba)
                return ptr;

        }

        public unsafe float[] Get()
        {
            return rgba; 

        }

        public Span<float> GetSpan() => new Span<float>(rgba);
    }
}
