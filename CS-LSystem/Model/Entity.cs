using OpenGL;

namespace LSystem
{
    public class Entity
    {
        protected string _name;
        protected uint _guid = 0;
        protected Vertex3f _color;
        protected bool _textured = false;
        protected RawModel3d _model;
        protected OpenGL.Quaternion _quaternion = OpenGL.Quaternion.Identity;
        protected Material _material;
        protected PrimitiveType _primitiveType;
        protected float _lineWidth = 1.0f;

        Vertex3f _position;
        Vertex3f _scale;

        public PrimitiveType PrimitiveType => _primitiveType;

        public uint OBJECT_GUID => _guid;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public float LineWidth
        {
            get =>_lineWidth; 
            set => _lineWidth = value;
        }

        public RawModel3d Model
        {
            get => _model;
            set
            {
                _textured = (value is TexturedModel);
                _model = value;
            }
        }

        public Vertex3f Position
        {
            get => _position;
            set => _position = value;
        }

        public Matrix4x4f ModelMatrix
        {
            get // 연산순서는 S->R->T순이다.
            {
                Matrix4x4f S = Extension.Scaled(_scale);
                Matrix4x4f R = ((Matrix4x4f)(_quaternion));
                Matrix4x4f T = Matrix4x4f.Translated(_position.x, _position.y, _position.z);
                return S * R * T;
            }
        }

        public Material Material
        {
            get => _material;
            set => _material = value;
        }

        public bool IsTextured
        {
            get => _textured;
            set => _textured = value;
        }

        public Entity(RawModel3d rawModel3D, PrimitiveType primitiveType = PrimitiveType.Triangles, string name = "")
        {
            _primitiveType = primitiveType;
            _guid = GUID.GenID;
            _scale = Vertex3f.One;
            _position = Vertex3f.Zero;
            _model = rawModel3D;
            _name = (name == "") ? $"Entity" + _guid : name;
            _color = Rand.NextColor3f;
            _textured = (_model is TexturedModel);
        }

        public void Scaled(float scaleX, float scaleY, float scaleZ)
        {
            _scale.x = scaleX;
            _scale.y = scaleY;
            _scale.z = scaleZ;
        }

        public void IncreasePosition(float dx, float dy, float dz)
        {
            _position.x += dx;
            _position.y += dy;
            _position.z += dz;
        }
        public virtual void Yaw(float deltaDegree)
        {
            // q0 * q이므로 q회전 -> q0회전이다.
            OpenGL.Quaternion q = new OpenGL.Quaternion(Vertex3f.UnitY, deltaDegree);
            _quaternion = _quaternion.Concatenate(q);
        }

        public virtual void Roll(float deltaDegree)
        {
            OpenGL.Quaternion q = new OpenGL.Quaternion(Vertex3f.UnitX, deltaDegree);
            _quaternion = _quaternion.Concatenate(q);
        }

        public virtual void Pitch(float deltaDegree)
        {
            OpenGL.Quaternion q = new OpenGL.Quaternion(Vertex3f.UnitZ, deltaDegree);
            _quaternion = _quaternion.Concatenate(q);
        }

    }
}
