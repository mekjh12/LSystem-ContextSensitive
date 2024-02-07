using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    class Loader3d
    {
        public static RawModel3d LoadLine(float sx, float sy, float sz, float ex, float ey, float ez)
        {
            float[] positions = new float[] { sx, sy, sz, ex, ey, ez };

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo;
            vbo = StoreDataInAttributeList(0, 3, positions);
            Gl.BindVertexArray(0);

            RawModel3d rawModel = new RawModel3d(vao, positions);
            return rawModel;
        }

        /// <summary>
        /// 반시계 방향으로 회전하여 적용한다.
        /// </summary>
        /// <param name="rotDeg">degree</param>
        /// <returns></returns>
        public static RawModel3d LoadPlane(float rotDeg = 0)
        {
            float Cos(float radian) => (float)Math.Cos(radian);
            float Sin(float radian) => (float)Math.Sin(radian);

            float[] positions =
            {
                -1.0f, -1.0f, 0.0f,
                1.0f, -1.0f, 0.0f,
                1.0f, 1.0f,  0.0f,
                1.0f, 1.0f,  0.0f,
                -1.0f, 1.0f,  0.0f,
                -1.0f, -1.0f,  0.0f
            };

            float[] normals =
            {
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f
            };

            float[] textures =
            {
                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 0.0f
            };

            TangentSpace.CalculateTangents(positions, textures, normals, out float[] tangents, out float[] bitangents);

            // (tu,tv)를 회전하여 텍스처링한다.
            for (int i = 0; i < textures.Length; i += 2)
            {
                float tu = textures[i + 0];
                float tv = textures[i + 1];
                float deg = rotDeg.ToRadian();
                textures[i + 0] = Cos(deg) * tu - Sin(deg) * tv;
                textures[i + 1] = Sin(deg) * tu + Cos(deg) * tv;
            }

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo;
            vbo = StoreDataInAttributeList(0, 3, positions);
            vbo = StoreDataInAttributeList(1, 2, textures);
            vbo = StoreDataInAttributeList(2, 3, normals);
            vbo = StoreDataInAttributeList(3, 4, tangents);
            vbo = StoreDataInAttributeList(4, 4, bitangents);
            Gl.BindVertexArray(0);

            RawModel3d rawModel = new RawModel3d(vao, positions);
            return rawModel;
        }

        /// <summary>
        /// 구면을 만든다.
        /// </summary>
        /// <param name="r">구의 반지름</param>
        /// <param name="piece">위도0도부터 90도까지 n등분을 한다. 이것은 360도를 4n등분한다.</param>
        /// <param name="outer">외부 또는 내부</param>
        /// <returns></returns>
        public static RawModel3d LoadSphere(float r = 1.0f, int piece = 3, float Tu = 1.0f, float Tv = 1.0f, bool outer = true)
        {
            float unitAngle = (float)(Math.PI / 2.0f) / piece;
            float deltaTheta = 360.ToRadian() / (4.0f * piece);

            List<float> vertices = new List<float>();
            List<float> textures = new List<float>();

            // <image url="$(ProjectDir)_PictureComment\SphereCoordinate.png" scale="0.8" />
            // 반시계 방향으로 면을 생성한다.
            for (int i = -piece; i < piece; i++) // phi
            {
                for (int j = 0; j < piece * 4; j++) // theta
                {
                    float theta1 = j * unitAngle;
                    float theta2 = (j + 1) * unitAngle;
                    float phi1 = i * unitAngle;
                    float phi2 = (i + 1) * unitAngle;
                    float tu1 = Tu * (deltaTheta * (j + 0)) / 360.ToRadian();
                    float tu2 = Tu * deltaTheta * (j + 1) / 360.ToRadian();
                    float tv1 = Tv * i * unitAngle / 90.ToRadian();
                    float tv2 = Tv * (i + 1) * unitAngle / 90.ToRadian();

                    if (outer)
                    {
                        if (i == (piece - 1))
                        {
                            vertices.AddRange(SphereCoordination(r, theta2, phi1));
                            vertices.AddRange(SphereCoordination(r, theta2, phi2));
                            vertices.AddRange(SphereCoordination(r, theta1, phi1));
                            textures.AddRange(TextureCoordination(tu2, tv1));
                            textures.AddRange(TextureCoordination(tu2, tv2));
                            textures.AddRange(TextureCoordination(tu1, tv1));
                        }
                        else if (i == -piece)
                        {
                            vertices.AddRange(SphereCoordination(r, theta2, phi2));
                            vertices.AddRange(SphereCoordination(r, theta1, phi2));
                            vertices.AddRange(SphereCoordination(r, theta1, phi1));
                            textures.AddRange(TextureCoordination(tu2, tv2));
                            textures.AddRange(TextureCoordination(tu1, tv2));
                            textures.AddRange(TextureCoordination(tu1, tv1));
                        }
                        else
                        {
                            vertices.AddRange(SphereCoordination(r, theta2, phi2));
                            vertices.AddRange(SphereCoordination(r, theta1, phi2));
                            vertices.AddRange(SphereCoordination(r, theta1, phi1));
                            vertices.AddRange(SphereCoordination(r, theta2, phi1));
                            vertices.AddRange(SphereCoordination(r, theta2, phi2));
                            vertices.AddRange(SphereCoordination(r, theta1, phi1));
                            textures.AddRange(TextureCoordination(tu2, tv2));
                            textures.AddRange(TextureCoordination(tu1, tv2));
                            textures.AddRange(TextureCoordination(tu1, tv1));
                            textures.AddRange(TextureCoordination(tu2, tv1));
                            textures.AddRange(TextureCoordination(tu2, tv2));
                            textures.AddRange(TextureCoordination(tu1, tv1));
                        }
                    }
                    else
                    {
                        if (i == (piece - 1))
                        {
                            vertices.AddRange(SphereCoordination(r, theta2, phi1));
                            vertices.AddRange(SphereCoordination(r, theta1, phi1));
                            vertices.AddRange(SphereCoordination(r, theta2, phi2));
                            textures.AddRange(TextureCoordination(tu2, tv1));
                            textures.AddRange(TextureCoordination(tu1, tv1));
                            textures.AddRange(TextureCoordination(tu2, tv2));
                        }
                        else if (i == -piece)
                        {
                            vertices.AddRange(SphereCoordination(r, theta2, phi2));
                            vertices.AddRange(SphereCoordination(r, theta1, phi1));
                            vertices.AddRange(SphereCoordination(r, theta1, phi2));
                            textures.AddRange(TextureCoordination(tu2, tv2));
                            textures.AddRange(TextureCoordination(tu1, tv1));
                            textures.AddRange(TextureCoordination(tu1, tv2));
                        }
                        else
                        {
                            vertices.AddRange(SphereCoordination(r, theta2, phi2));
                            vertices.AddRange(SphereCoordination(r, theta1, phi1));
                            vertices.AddRange(SphereCoordination(r, theta1, phi2));
                            vertices.AddRange(SphereCoordination(r, theta2, phi1));
                            vertices.AddRange(SphereCoordination(r, theta1, phi1));
                            vertices.AddRange(SphereCoordination(r, theta2, phi2));
                            textures.AddRange(TextureCoordination(tu2, tv2));
                            textures.AddRange(TextureCoordination(tu1, tv1));
                            textures.AddRange(TextureCoordination(tu1, tv2));
                            textures.AddRange(TextureCoordination(tu2, tv1));
                            textures.AddRange(TextureCoordination(tu1, tv1));
                            textures.AddRange(TextureCoordination(tu2, tv2));
                        }
                    }
                }
            }

            float[] positions = vertices.ToArray();
            float[] texCoords = textures.ToArray();

            TangentSpace.CalculateTangents(positions, texCoords, positions, out float[] tangents, out float[] bitangents);

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo;
            vbo = StoreDataInAttributeList(0, 3, positions);
            vbo = StoreDataInAttributeList(1, 2, texCoords);
            vbo = StoreDataInAttributeList(2, 3, positions);
            vbo = StoreDataInAttributeList(3, 4, tangents);
            vbo = StoreDataInAttributeList(4, 4, bitangents);

            Gl.BindVertexArray(0);

            RawModel3d rawModel = new RawModel3d(vao, positions);
            return rawModel;

            float[] TextureCoordination(float tu, float tv)
            {
                float[] verts = new float[2];
                verts[0] = tu;
                verts[1] = tv;
                return verts;
            }

            float[] SphereCoordination(float radius, float theta, float phi)
            {
                float[] verts = new float[3];
                float z = radius * (float)Math.Sin(phi);
                float R = radius * (float)Math.Cos(phi);
                float x = R * (float)Math.Cos(theta);
                float y = R * (float)Math.Sin(theta);
                verts[0] = x;
                verts[1] = y;
                verts[2] = z;
                return verts;
            }
        }


        public static RawModel3d LoadCone(int n, float radius, float height, bool isOuter = true)
        {
            float Cos(float radian) => (float)Math.Cos(radian);
            float Sin(float radian) => (float)Math.Sin(radian);

            List<float> positionList = new List<float>();
            List<float> normalList = new List<float>();
            List<float> textureList = new List<float>();

            Vertex3f[] vertices = new Vertex3f[n + 2];
            Vertex2f[] texCoords = new Vertex2f[n + 1];

            float unitRad = (360.0f / n).ToRadian();
            for (int i = 0; i < n; i++)
            {
                float rad = i * unitRad;
                float px = radius * Cos(rad);
                float py = radius * Sin(rad);
                vertices[i] = new Vertex3f(px, py, 0);
                float tu = 0.5f * Cos(rad) + 0.5f;
                float tv = 0.5f * Sin(rad) + 0.5f;
                texCoords[i] = new Vertex2f(tu, tv);
            }
            vertices[n] = new Vertex3f(0, 0, 0);
            vertices[n + 1] = new Vertex3f(0, 0, height);
            texCoords[n] = new Vertex2f(0.5f, 0.5f);

            int a, b, c;

            // 옆면
            for (int i = 0; i < n; i++)
            {
                if (isOuter)
                {
                    a = (i + 1) % n;
                    b = (i + 0) % n;
                    c = n + 1;
                }
                else
                {
                    a = (i + 0) % n;
                    b = (i + 1) % n;
                    c = n + 1;
                }
                positionList.Add(vertices[a].x);
                positionList.Add(vertices[a].y);
                positionList.Add(vertices[a].z);
                positionList.Add(vertices[b].x);
                positionList.Add(vertices[b].y);
                positionList.Add(vertices[b].z);
                positionList.Add(vertices[c].x);
                positionList.Add(vertices[c].y);
                positionList.Add(vertices[c].z);

                textureList.Add(texCoords[a].x);
                textureList.Add(texCoords[a].y);
                textureList.Add(texCoords[b].x);
                textureList.Add(texCoords[b].y);
                textureList.Add(texCoords[n].x);
                textureList.Add(texCoords[n].y);

                Vertex3f normalA = vertices[a] - vertices[c];
                normalA.z = -(normalA.x * normalA.x + normalA.y * normalA.y) / normalA.z;
                Vertex3f normalB = vertices[b] - vertices[c];
                normalB.z = -(normalB.x * normalB.x + normalB.y * normalB.y) / normalB.z;

                normalList.Add(normalA.x); normalList.Add(normalA.y); normalList.Add(normalA.z);
                normalList.Add(normalB.x); normalList.Add(normalB.y); normalList.Add(normalB.z);
                normalList.Add(0); normalList.Add(0); normalList.Add(1);
            }

            // 아랫면
            for (int i = 0; i < n; i++)
            {
                if (isOuter)
                {
                    a = (i + 0) % n;
                    b = (i + 1) % n;
                    c = n;
                }
                else
                {
                    a = (i + 1) % n;
                    b = (i + 0) % n;
                    c = n;
                }
                positionList.Add(vertices[a].x);
                positionList.Add(vertices[a].y);
                positionList.Add(vertices[a].z);
                positionList.Add(vertices[b].x);
                positionList.Add(vertices[b].y);
                positionList.Add(vertices[b].z);
                positionList.Add(vertices[c].x);
                positionList.Add(vertices[c].y);
                positionList.Add(vertices[c].z);
                textureList.Add(texCoords[a].x); textureList.Add(texCoords[a].y);
                textureList.Add(texCoords[b].x); textureList.Add(texCoords[b].y);
                textureList.Add(texCoords[c].x); textureList.Add(texCoords[c].y);
                normalList.Add(0); normalList.Add(0); normalList.Add(-1);
                normalList.Add(0); normalList.Add(0); normalList.Add(-1);
                normalList.Add(0); normalList.Add(0); normalList.Add(-1);
            }

            float[] positions = positionList.ToArray();
            float[] textures = textureList.ToArray();
            float[] normals = normalList.ToArray();

            TangentSpace.CalculateTangents(positions, textures, normals, out float[] tangents, out float[] bitangents);

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo;
            vbo = StoreDataInAttributeList(0, 3, positions);
            vbo = StoreDataInAttributeList(1, 2, textures);
            vbo = StoreDataInAttributeList(2, 3, normals);
            vbo = StoreDataInAttributeList(3, 4, tangents);
            vbo = StoreDataInAttributeList(4, 4, bitangents);

            Gl.BindVertexArray(0);

            RawModel3d rawModel = new RawModel3d(vao, positions);
            return rawModel;
        }


        public static RawModel3d LoadCube(float tu = 1.0f, float tv = 1.0f, bool outer = true)
        {
            // vertices 8 points.
            Vertex3f[] Points = new Vertex3f[8]
            {
                new Vertex3f(-1, -1, -1),
                new Vertex3f(1, -1, -1),
                new Vertex3f(1, 1, -1),
                new Vertex3f(-1, 1, -1),
                new Vertex3f(-1, -1, 1),
                new Vertex3f(1, -1, 1),
                new Vertex3f(1, 1, 1),
                new Vertex3f(-1, 1, 1)
            };

            Vertex3f[] Normals = new Vertex3f[6]
            {
                Vertex3f.UnitX,
                -Vertex3f.UnitX,
                Vertex3f.UnitY,
                -Vertex3f.UnitY,
                Vertex3f.UnitZ,
                -Vertex3f.UnitZ
            };

            Vertex2f[] texCoords = new Vertex2f[4]
            {
                new Vertex2f(0, 0),
                new Vertex2f(tu, 0),
                new Vertex2f(tu, tv),
                new Vertex2f(0, tv)
            };

            //         7------6                        
            //         |      |                
            //         |  +z  |   counter-clockwise
            //         |      |               
            //  7------4------5------6------7          
            //  |      |      |      |      |  
            //  |  -x  |  -y  |  +x  |  +y  |
            //  |      |      |      |      | 
            //  3------0------1------2------3           
            //         |      |                 
            //         |  -z  |                
            //         |      |                
            //         3------2                         
            List<float> positionList = new List<float>();
            List<float> normalList = new List<float>();
            List<float> textureList = new List<float>();

            attachQuad3(positionList, Points, 1, 2, 6, 5, outer); // +x
            attachQuad3(positionList, Points, 3, 0, 4, 7, outer); // -x
            attachQuad3(positionList, Points, 2, 3, 7, 6, outer); // +y
            attachQuad3(positionList, Points, 0, 1, 5, 4, outer); // -y
            attachQuad3(positionList, Points, 4, 5, 6, 7, outer); // +z
            attachQuad3(positionList, Points, 3, 2, 1, 0, outer); // -z

            attachQuad3N(normalList, Normals, 0, 1, outer); // +x
            attachQuad3N(normalList, Normals, 1, 0, outer); // -x
            attachQuad3N(normalList, Normals, 2, 3, outer); // +y
            attachQuad3N(normalList, Normals, 3, 2, outer); // -y
            attachQuad3N(normalList, Normals, 4, 5, outer); // +z
            attachQuad3N(normalList, Normals, 5, 4, outer); // -z

            attachQuad2(textureList, texCoords, 0, 1, 2, 3, outer); // +x
            attachQuad2(textureList, texCoords, 0, 1, 2, 3, outer); // -x
            attachQuad2(textureList, texCoords, 0, 1, 2, 3, outer); // +y
            attachQuad2(textureList, texCoords, 0, 1, 2, 3, outer); // -y
            attachQuad2(textureList, texCoords, 0, 1, 2, 3, outer); // +z
            attachQuad2(textureList, texCoords, 0, 1, 2, 3, outer); // -z

            // gen vertext array.
            float[] positions = positionList.ToArray();
            float[] textures = textureList.ToArray();
            float[] normals = normalList.ToArray();

            TangentSpace.CalculateTangents(positions, textures, normals, out float[] tangents, out float[] bitangents);

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo;
            vbo = StoreDataInAttributeList(0, 3, positions);
            vbo = StoreDataInAttributeList(1, 2, textures);
            vbo = StoreDataInAttributeList(2, 3, normals);
            vbo = StoreDataInAttributeList(3, 4, tangents);
            vbo = StoreDataInAttributeList(4, 4, bitangents);

            Gl.BindVertexArray(0);

            void attachQuad3(List<float> list, Vertex3f[] points, int a, int b, int c, int d, bool isOuter)
            {
                if (isOuter)
                {
                    list.AddRange(attachVertices3f(points, a, b, c));
                    list.AddRange(attachVertices3f(points, a, c, d));
                }
                else
                {
                    list.AddRange(attachVertices3f(points, a, c, b));
                    list.AddRange(attachVertices3f(points, a, d, c));
                }
            }

            void attachQuad3N(List<float> list, Vertex3f[] points, int a, int b, bool isOuter)
            {
                if (isOuter)
                {
                    list.AddRange(attachVertices3f(points, a, a, a));
                    list.AddRange(attachVertices3f(points, a, a, a));
                }
                else
                {
                    list.AddRange(attachVertices3f(points, b, b, b));
                    list.AddRange(attachVertices3f(points, b, b, b));
                }
            }

            void attachQuad2(List<float> list, Vertex2f[] points, int a, int b, int c, int d, bool isOuter)
            {
                if (isOuter)
                {
                    list.AddRange(attachVertices2f(points, a, b, c));
                    list.AddRange(attachVertices2f(points, a, c, d));
                }
                else
                {
                    list.AddRange(attachVertices2f(points, a, c, b));
                    list.AddRange(attachVertices2f(points, a, d, c));
                }
            }

            float[] attachVertices3f(Vertex3f[] points, int a, int b, int c)
            {
                float[] vertices = new float[9];
                vertices[0] = points[a].x; vertices[1] = points[a].y; vertices[2] = points[a].z;
                vertices[3] = points[b].x; vertices[4] = points[b].y; vertices[5] = points[b].z;
                vertices[6] = points[c].x; vertices[7] = points[c].y; vertices[8] = points[c].z;
                return vertices;
            }

            float[] attachVertices2f(Vertex2f[] points, int a, int b, int c)
            {
                float[] vertices = new float[6];
                vertices[0] = points[a].x; vertices[1] = points[a].y;
                vertices[2] = points[b].x; vertices[3] = points[b].y;
                vertices[4] = points[c].x; vertices[5] = points[c].y;
                return vertices;
            }

            return new RawModel3d(vao, positions);
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
