using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace BEngine
{
    [Serializable]
    public struct Color
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }

        public Color()
        {
            r = 0;
            g = 0;
            b = 0;
            a = 1f;
        }

        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 1f;
        }

        public Color(float r, float g, float b,float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static implicit operator System.Numerics.Vector4(Color color)
        {
            return new System.Numerics.Vector4(color.r, color.g, color.b, color.a);
        } 

        public static implicit operator Color(System.Numerics.Vector4 vector4)
        {
            return new Color(vector4.X, vector4.Y, vector4.Z, vector4.W);
        }
    }
}