using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    public class StaticShader : ShaderProgram
    {
        const string VERTEX_FILE = @"\Shader\static.vert";
        const string FRAGMENT_FILE = @"\Shader\static.frag";

        public StaticShader() : base(EngineLoop.PROJECT_PATH + VERTEX_FILE,
            EngineLoop.PROJECT_PATH + FRAGMENT_FILE, "")
        {

        }

        protected override void BindAttributes()
        {
            base.BindAttribute(0, "position");
            base.BindAttribute(1, "textureCoords");
        }

        protected override void GetAllUniformLocations()
        {
            UniformLocations("model", "view", "proj");
            UniformLocations("modelTexture");
            UniformLocations("color", "isTextured");
        }

        public void LoadTexture(string textureUniformName, TextureUnit textureUnit, uint texture)
        {
            base.LoadInt(_location[textureUniformName], textureUnit - TextureUnit.Texture0);
            Gl.ActiveTexture(textureUnit);
            Gl.BindTexture(TextureTarget.Texture2d, texture);
        }

        public void LoadProjMatrix(Matrix4x4f matrix)
        {
            base.LoadMatrix(_location["proj"], matrix);
        }

        public void LoadViewMatrix(Matrix4x4f matrix)
        {
            base.LoadMatrix(_location["view"], matrix);
        }

        public void LoadModelMatrix(Matrix4x4f matrix)
        {
            base.LoadMatrix(_location["model"], matrix);
        }

        public void LoadObjectColor(Vertex4f color)
        {
            base.LoadVector(_location["color"], color);
        }

        public void LoadIsTextured(bool isTextured)
        {
            base.LoadBoolean(_location["isTextured"], isTextured);
        }
    }
}
