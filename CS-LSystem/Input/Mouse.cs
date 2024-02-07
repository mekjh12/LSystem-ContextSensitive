using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    static class Mouse
    {
        public static Vertex2i PrevPosition = Vertex2i.Zero;
        public static Vertex2i CurrentPosition = Vertex2i.Zero;
        public static Vertex2i DeltaPosition => CurrentPosition - PrevPosition;
    }
}
