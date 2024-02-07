namespace LSystem
{
    public class TexturedModel : RawModel3d
    {
        private Texture _texture;

        public Texture Texture
        {
            get => _texture;
        }

        public TexturedModel(RawModel3d model, Texture texture) :
            base(model.VAO, model.Vertices)
        {
            if (texture != null)
            {
                _texture = texture;
            }
        }
    }
}
