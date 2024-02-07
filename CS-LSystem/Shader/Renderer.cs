using OpenGL;

namespace LSystem
{
    public class Renderer
    {
        public static RawModel3d Line = Loader3d.LoadLine(0, 0, 0, 1, 0, 0);
        public static RawModel3d Cube = Loader3d.LoadCube();
        public static RawModel3d Cone = Loader3d.LoadCone(4, 1.0f, 3.0f, false);
        public static RawModel3d Sphere = Loader3d.LoadSphere(r: 1, piece: 6);
        public static RawModel3d Rect = Loader3d.LoadPlane();

        public static void Render(StaticShader shader, Entity entity, Camera camera)
        {
            Gl.Enable(EnableCap.Blend);
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            shader.Bind();

            Gl.BindVertexArray(entity.Model.VAO);
            Gl.EnableVertexAttribArray(0);
            Gl.EnableVertexAttribArray(1);
            Gl.EnableVertexAttribArray(2);

            if (entity.IsTextured)
            {
                shader.LoadIsTextured(true);
                TexturedModel modelTextured = entity.Model as TexturedModel;
                shader.SetInt("modelTexture", 0);
                Gl.ActiveTexture(TextureUnit.Texture0);
                Gl.BindTexture(TextureTarget.Texture2d, modelTextured.Texture.TextureID);
                Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, Gl.REPEAT);
                Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, Gl.REPEAT);
            }

            if (entity.Material != null) shader.LoadObjectColor(entity.Material.Ambient);
            shader.LoadProjMatrix(camera.ProjectiveMatrix);
            shader.LoadViewMatrix(camera.ViewMatrix);
            shader.LoadModelMatrix(entity.ModelMatrix);

            if (entity.PrimitiveType == PrimitiveType.Lines)
            {
                Gl.LineWidth(entity.LineWidth);
            }
            else
            {
                Gl.LineWidth(1.0f);
            }

            Gl.DrawArrays(entity.PrimitiveType, 0, entity.Model.VertexCount);

            Gl.DisableVertexAttribArray(2);
            Gl.DisableVertexAttribArray(1);
            Gl.DisableVertexAttribArray(0);
            Gl.BindVertexArray(0);

            shader.Unbind();
        }

    }
}
