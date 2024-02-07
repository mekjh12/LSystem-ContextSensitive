using OpenGL;
using System;

namespace LSystem
{
    public class RawModel3d
    {
        uint _vao;
        uint _vbo;
        uint _ibo;
        int _indexCount;
        int _vertexCount;
        bool _isDrawElement;

        Vertex3f[] _vertices;

        public bool IsDrawElement => _isDrawElement;

        public Vertex3f[] Vertices => _vertices;

        public uint VAO => _vao;

        public uint VBO => _vbo;

        public uint IBO => _ibo;

        public int IndicesCount => _indexCount;

        public int VertexCount
        {
            get => _vertexCount;
            set => _vertexCount = value;
        }

        public RawModel3d(uint vao, Vertex3f[] vertices)
        {
            _isDrawElement = false;
            _vao = vao;
            _vertexCount = vertices.Length;
            _vertices = vertices;
        }

        public RawModel3d(float[] positions)
        {
            // VAO, VBO 생성
            _vao = Gl.GenVertexArray();
            Gl.BindVertexArray(_vao);
            _vbo = StoreDataInAttributeList(0, 3, positions);
            Gl.BindVertexArray(0);

            // 기본 정보을 개체에 가져옴.
            _vertices = GetVertexArray(positions);
            _isDrawElement = false;
        }

        public RawModel3d(float[] positions, float[] normals)
        {
            // VAO, VBO 생성
            _vao = Gl.GenVertexArray();
            Gl.BindVertexArray(_vao);
            _vbo = StoreDataInAttributeList(0, 3, positions);
            Gl.BindVertexArray(0);

            // 기본 정보을 개체에 가져옴.
            _vertices = GetVertexArray(positions);
            _isDrawElement = false;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="vao"></param>
        /// <param name="positions"></param>
        public RawModel3d(uint vao, float[] positions)
        {
            _isDrawElement = false;
            _vao = vao;
            _vertexCount = positions.Length / 3;
            _vertices = new Vertex3f[_vertexCount];

            for (int i = 0; i < _vertexCount; i++)
            {
                _vertices[i] = new Vertex3f(positions[3 * i + 0], positions[3 * i + 1], positions[3 * i + 2]);
            }
        }

        /// <summary>
        /// VAO, VBO, IBO의 메모리를 해제한다.
        /// </summary>
        public void Clean()
        {
            if (_vao >= 0)
            {
                if (_vbo >= 0) Gl.DeleteBuffers(_vbo);
                if (_ibo >= 0) Gl.DeleteBuffers(_ibo);
                Gl.DeleteVertexArrays(_vao);
            }
        }

        /// <summary>
        /// 실수형 배열을 Vertex3f 배열로 반환한다.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private Vertex3f[] GetVertexArray(float[] array)
        {
            if (array.Length % 3 != 0)
                new Exception("배열의 갯수는 3의 배수이어야 합니다.");

            int vertexCount = array.Length / 3;
            Vertex3f[] vertices = new Vertex3f[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = new Vertex3f(array[3 * i + 0], array[3 * i + 1], array[3 * i + 2]);
            }
            return vertices;
        }

        /// <summary>
        /// * data를 gpu에 올리고 vbo를 반환한다.<br/>
        /// * vao는 함수 호출 전에 바인딩하여야 한다.<br/>
        /// </summary>
        /// <param name="attributeNumber">attributeNumber 슬롯 번호</param>
        /// <param name="coordinateSize">자료의 벡터 성분의 개수 (예) vertex3f는 3이다.</param>
        /// <param name="data"></param>
        /// <param name="usage"></param>
        /// <returns>vbo를 반환</returns>
        public static unsafe uint StoreDataInAttributeList(uint attributeNumber, int coordinateSize, float[] data, BufferUsage usage = BufferUsage.StaticDraw)
        {
            // VBO 생성
            uint vboID = Gl.GenBuffer();

            // VBO의 데이터를 CPU로부터 GPU에 복사할 때 사용하는 BindBuffer를 다음과 같이 사용
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(data.Length * sizeof(float)), data, usage);

            // 이전에 BindVertexArray한 VAO에 현재 Bind된 VBO를 attributeNumber 슬롯에 설정
            Gl.VertexAttribPointer(attributeNumber, coordinateSize, VertexAttribType.Float, false, 0, IntPtr.Zero);
            //Gl.VertexArrayVertexBuffer(glVertexArrayVertexBuffer, vboID, )

            // GPU 메모리 조작이 필요 없다면 다음과 같이 바인딩 해제
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vboID;
        }

    }
}
