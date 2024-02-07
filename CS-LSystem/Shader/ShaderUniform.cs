using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    public abstract class ShaderUniform
    {
        protected void LoadFloat(int location, float value)
        {
            Gl.Uniform1f<float>(location, 1, value);
        }

        protected void LoadInt(int location, int value)
        {
            Gl.Uniform1i<int>(location, 1, value);
        }

        protected void LoadVector(int location, Vertex3f value)
        {
            Gl.Uniform3f(location, 1, value);
        }

        protected void LoadVector(int location, Vertex2f value)
        {
            Gl.Uniform2f(location, 1, value);
        }

        protected void LoadVector(int location, Vertex4f value)
        {
            Gl.Uniform4f(location, 1, value);
        }

        protected void LoadBoolean(int location, bool value)
        {
            float toLoad = (value == true) ? 1.0f : 0.0f;
            Gl.Uniform1f(location, 1, toLoad);
        }

        protected void LoadMatrix(int location, Matrix4x4f matrix)
        {
            Gl.UniformMatrix4(location, false, ((float[])matrix));
        }

        protected void LoadMatrix(int location, Matrix3x3f matrix)
        {
            Gl.UniformMatrix3(location, false, ((float[])matrix));
        }

    }
}
