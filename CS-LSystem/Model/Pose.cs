using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    struct Pose
    {
        Quaternion _q;
        Vertex3f _pos;

        public Quaternion Quaternion
        {
            get => _q; 
            set => _q = value;   
        }

        public Matrix4x4f Matrix4x4f => (Matrix4x4f)_q;

        public Vertex3f Postiton
        {
            get=> _pos;
            set => _pos = value;
        }

        public Pose(Quaternion q,  Vertex3f pos)
        {
            _q = q;
            _pos = pos;
        }

    }
}
