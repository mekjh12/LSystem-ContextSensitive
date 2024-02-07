using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    /// <summary>
    /// Material은 Light Source에 따라 재질의 특성을 고려하여 빛을 반사한다.
    /// Ma, Md, Ms, Me
    /// Sa, Sd, Ss, Se(X)
    /// 이때, Md=Sd와 같다.
    /// </summary>
    public class Material
    {
        protected Vertex4f _ambient;
        //protected Vertex4f _diffuse;
        protected Vertex4f _specular;
        protected Vertex4f _emissive;

        protected float _shininess;
        //protected Vertex4f _color;

        public static Material Red(float alpha = 0.1f)
        {
            Material material = new Material()
            {
                _ambient = new Vertex4f(1, 0, 0, alpha),
                //_diffuse = new Vertex4f(1, 0, 0, alpha),
                _specular = new Vertex4f(1, 0, 0, alpha),
                _emissive = new Vertex4f(1, 0, 0, alpha)
            };
            return material;
        }

        public static Material Yellow(float alpha = 0.1f)
        {
            Material material = new Material()
            {
                _ambient = new Vertex4f(1, 1, 0, alpha),
                //_diffuse = new Vertex4f(1, 0, 0, 1),
                _specular = new Vertex4f(1, 1, 0, alpha),
                _emissive = new Vertex4f(1, 1, 0, alpha)
            };
            return material;
        }

        public static Material Green(float alpha = 0.1f)
        {
            Material material = new Material()
            {
                _ambient = new Vertex4f(0, 1, 0, alpha),
                //_diffuse = new Vertex4f(1, 0, 0, 1),
                _specular = new Vertex4f(0, 1, 0, alpha),
                _emissive = new Vertex4f(0, 1, 0, alpha)
            };
            return material;
        }

        public static Material Blue
        {
            get
            {
                Material material = new Material()
                {
                    _ambient = new Vertex4f(0, 0, 1, 1),
                    //_diffuse = new Vertex4f(1, 0, 0, 1),
                    _specular = new Vertex4f(0, 0, 1, 1),
                    _emissive = new Vertex4f(0, 0, 1, 1)
                };
                return material;
            }
        }

        public static Material White
        {
            get
            {
                Material material = new Material()
                {
                    _ambient = new Vertex4f(1, 1, 1, 1),
                    //_diffuse = new Vertex4f(1, 1, 1, 1),
                    _specular = new Vertex4f(1, 1, 1, 1),
                    _emissive = new Vertex4f(1, 1, 1, 1)
                };
                return material;
            }
        }


        public Material()
        {
            _ambient = new Vertex4f(1.0f, 1.0f, 1.0f, 1.0f);
            _specular = new Vertex4f(1.0f, 1.0f, 1.0f, 1.0f);
            _emissive = new Vertex4f(0.0f, 0.0f, 0.0f, 1.0f);
            _shininess = 32.0f;
        }

        public Material(Vertex4f color)
        {
            _ambient = color;
            _specular = color;
            _emissive = color;
            _shininess = 32.0f;
        }

        public Material(float r, float g, float b, float a)
        {
            Vertex4f color = new Vertex4f(r, g, b, a);
            _ambient = color;
            _specular = color;
            _emissive = color;
            _shininess = 32.0f;
        }


        /// <summary>
        /// Shininess가 0부터 무한대의 값을 갖는다.
        /// 큰 값을 가지면 표면이 매끄러으므로 중심부에 빛이 모여있고 
        /// 작은 값을 가지면 표면이 거칠어 중심부로부터 빛이 퍼진다.
        /// </summary>
        public float Shininess
        {
            get
            {
                return _shininess;
            }

            set
            {
                _shininess = value;
            }
        }

        public Vertex4f Ambient
        {
            get
            {
                return _ambient;
            }

            set
            {
                _ambient = value;
            }
        }

        public Vertex4f Emissive
        {
            get
            {
                return _emissive;
            }

            set
            {
                _emissive = value;
            }
        }

        public Vertex4f Specular
        {
            get
            {
                return _specular;
            }

            set
            {
                _specular = value;
            }
        }

    }
}
