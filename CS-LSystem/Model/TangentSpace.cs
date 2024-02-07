using OpenGL;
using System;

namespace LSystem
{
    internal class TangentSpace
    {
        public static void CalculateTangents(float[] positions, float[] textures, float[] normals,
            out float[] tangent, out float[] bitangent)
        {
            if (positions.Length % 3 != 0) throw new Exception("positions의 길이는 3의 배수이다.");
            if (textures.Length % 2 != 0) throw new Exception("textures 길이는 2의 배수이다.");
            if (normals.Length % 3 != 0) throw new Exception("normals 길이는 3의 배수이다.");

            // Allocate temporay storage for tangents and bitangents and initialize to zeros.
            int vertexCount = positions.Length / 3;
            float[] tangents = new float[positions.Length];
            float[] bitangents = new float[positions.Length];
            for (int i = 0; i < tangents.Length; i++)
            {
                tangents[i] = 0.0f;
                bitangents[i] = 0.0f;
            }

            // Calculate tangents and bitangent for each triangles and add to all three vertices.
            int triangleCount = vertexCount / 3;
            for (int k = 0; k < triangleCount; k++)
            {
                Vertex3f p0 = new Vertex3f(positions[9 * k + 0], positions[9 * k + 1], positions[9 * k + 2]);
                Vertex3f p1 = new Vertex3f(positions[9 * k + 3], positions[9 * k + 4], positions[9 * k + 5]);
                Vertex3f p2 = new Vertex3f(positions[9 * k + 6], positions[9 * k + 7], positions[9 * k + 8]);

                Vertex2f w0 = new Vertex2f(textures[6 * k + 0], textures[6 * k + 1]);
                Vertex2f w1 = new Vertex2f(textures[6 * k + 2], textures[6 * k + 3]);
                Vertex2f w2 = new Vertex2f(textures[6 * k + 4], textures[6 * k + 5]);

                Vertex3f e1 = p1 - p0;
                Vertex3f e2 = p2 - p0;

                float x1 = w1.x - w0.x, x2 = w2.x - w0.x;
                float y1 = w1.y - w0.y, y2 = w2.y - w0.y;

                float r = 1.0f / (x1 * y2 - x2 * y1);
                Vertex3f t = (e1 * y2 - e2 * y1) * r;
                Vertex3f b = (e2 * x1 - e1 * x2) * r;

                tangents[9 * k + 0] = t.x;
                tangents[9 * k + 1] = t.y;
                tangents[9 * k + 2] = t.z;
                tangents[9 * k + 3] = t.x;
                tangents[9 * k + 4] = t.y;
                tangents[9 * k + 5] = t.z;
                tangents[9 * k + 6] = t.x;
                tangents[9 * k + 7] = t.y;
                tangents[9 * k + 8] = t.z;

                bitangents[9 * k + 0] = b.x;
                bitangents[9 * k + 1] = b.y;
                bitangents[9 * k + 2] = b.z;
                bitangents[9 * k + 3] = b.x;
                bitangents[9 * k + 4] = b.y;
                bitangents[9 * k + 5] = b.z;
                bitangents[9 * k + 6] = b.x;
                bitangents[9 * k + 7] = b.y;
                bitangents[9 * k + 8] = b.z;
            }

            tangent = new float[4 * vertexCount];
            bitangent = new float[4 * vertexCount];

            // Orthonormalize each tangent and calculate the handedness.
            for (int i = 0; i < vertexCount; i++)
            {
                Vertex3f t = new Vertex3f(tangents[3 * i + 0], tangents[3 * i + 1], tangents[3 * i + 2]);
                Vertex3f b = new Vertex3f(bitangents[3 * i + 0], bitangents[3 * i + 1], bitangents[3 * i + 2]);
                Vertex3f n = new Vertex3f(normals[3 * i + 0], normals[3 * i + 1], normals[3 * i + 2]);

                // Grand-shumitz othogonal process
                Vertex3f T = (t - n * (t.Dot(n) / n.Dot(n))).Normalized;
                float w = t.Cross(b).Dot(n) > 0.0f ? 1.0f : -1.0f;
                Vertex3f B = n.Cross(T).Normalized;
                tangent[4 * i + 0] = T.x;
                tangent[4 * i + 1] = T.y;
                tangent[4 * i + 2] = T.z;
                tangent[4 * i + 3] = w;

                bitangent[4 * i + 0] = B.x;
                bitangent[4 * i + 1] = B.y;
                bitangent[4 * i + 2] = B.z;
                bitangent[4 * i + 3] = w;
            }
        }
    }
}
