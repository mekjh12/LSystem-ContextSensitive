using OpenGL;
using System;
using System.Collections.Generic;

namespace LSystem
{
    class LoaderLSystem
    {
        public static RawModel3d Load3dByAonoKunii(MString word, GlobalParam g)
        {
            List<float> list = new List<float>();
            List<float> branchList3D = new List<float>();

            // 문자열을 순회하면서 경로를 만든다.
            Stack<Pose> stack = new Stack<Pose>();
            Pose pose = new Pose(Quaternion.Identity, Vertex3f.Zero);
            Stack<float> thickStack = new Stack<float>();

            // 줄기의 밑둥의 큰 원이다.
            Vertex3f[] baseVertor = null;
            Stack<Vertex3f[]> branchBaseStack = new Stack<Vertex3f[]>();

            // context-sensitive를 위한 설정
            Stack<string> contextStack = new Stack<string>();
            string branchContext = "";

            int idx = 0;
            float thick = 100.0f;

            // 한 글자마다 탐색함.
            while (idx < word.Length)
            {
                if (word.Length == 0) break;
                MChar c = word[idx];
                Matrix4x4f matQuaternion = (Matrix4x4f)pose.Quaternion;
                Vertex3f forward = matQuaternion.ForwardVector();
                Vertex3f up = matQuaternion.UpVector();
                Vertex3f left = matQuaternion.LeftVector();

                if (c.Alphabet == "F")
                {
                    float r = c[0];
                    Vertex3f start = pose.Postiton;
                    Vertex3f end = start + forward * r;
                    branchContext += c.Alphabet;

                    list.Add(start.x);
                    list.Add(start.y);
                    list.Add(start.z);
                    list.Add(end.x);
                    list.Add(end.y);
                    list.Add(end.z);

                    (float[] res, Vertex3f[] rot) = LoadBranch(baseVertor, start, end, 0.01f * thick, false);
                    branchList3D.AddRange(res);
                    baseVertor = rot;

                    pose.Postiton = end;

                    // tropism vector modify!
                    Vertex3f H = forward;
                    Vertex3f T = new Vertex3f(g["Tx"], g["Ty"], g["Tz"]);
                    Vertex3f B = H.Cross(T);
                    float alpha = g["e"] * B.Norm();
                    B.Normalize();
                    Vertex3f H1 = B.Rotate(H, alpha).Normalized;
                    Vertex3f L1 = H1.Cross(T).Normalized;
                    Vertex3f U1 = H1.Cross(L1).Normalized;
                    pose.Quaternion = Matrix3x3f.Identity.ToQuaternionFromTNB(L1, U1, H1);
                }
                else if (c.Alphabet == "f")
                {
                    float r = c[0];
                    Vertex3f start = pose.Postiton;
                    Vertex3f end = start + forward * r;
                    branchContext += c.Alphabet;

                    list.Add(start.x);
                    list.Add(start.y);
                    list.Add(start.z);
                    list.Add(end.x);
                    list.Add(end.y);
                    list.Add(end.z);

                    pose.Postiton = end;
                }
                else if (c.Alphabet == "+")
                {
                    float angle = c[0];
                    pose.Quaternion = up.Rotate(angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "-")
                {
                    float angle = c[0];
                    pose.Quaternion = up.Rotate(-angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "&")
                {
                    float angle = c[0];
                    pose.Quaternion = left.Rotate(angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "^")
                {
                    float angle = c[0];
                    pose.Quaternion = left.Rotate(-angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "\\")
                {
                    float angle = c[0];
                    pose.Quaternion = forward.Rotate(angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "/")
                {
                    float angle = c[0];
                    pose.Quaternion = forward.Rotate(-angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "$")
                {
                    Vertex3f V = Vertex3f.UnitZ;
                    Matrix4x4f mat = ((Matrix4x4f)pose.Quaternion);
                    Vertex3f H = mat.ForwardVector().Normalized;
                    Vertex3f L = V.Cross(H).Normalized;
                    Vertex3f U = H.Cross(L).Normalized;
                    pose.Quaternion = Matrix3x3f.Identity.ToQuaternionFromTNB(H, L, U);
                }
                else if (c.Alphabet == "!")
                {
                    thick = c[0];

                    if (baseVertor == null)
                    {
                        baseVertor = new Vertex3f[16];
                        float radius = 0.01f * thick;
                        float unitTheta = 360 / baseVertor.Length;
                        for (int i = 0; i < baseVertor.Length; i++)
                        {
                            baseVertor[i] = new Vertex3f((float)(radius * Math.Cos((unitTheta * i).ToRadian())),
                                (float)(radius * Math.Sin((unitTheta * i).ToRadian())), 0);
                        }
                    }
                }
                else if (c.Alphabet == "[")
                {
                    stack.Push(pose);
                    branchBaseStack.Push(baseVertor);
                    thickStack.Push(thick);
                    contextStack.Push(branchContext);
                    branchContext = "";
                }
                else if (c.Alphabet == "]")
                {
                    pose = stack.Pop();
                    baseVertor = branchBaseStack.Pop();
                    thick = thickStack.Pop();
                    branchContext = contextStack.Pop();
                }

                idx++;
            }

            // raw3d 모델을 만든다.
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo = StoreDataInAttributeList(0, 3, branchList3D.ToArray());
            Gl.BindVertexArray(0);
            RawModel3d rawModel = new RawModel3d(vao, branchList3D.ToArray());
            return rawModel;
        }

        public static RawModel3d Load3dByHonda(MString word, GlobalParam g)
        {
            List<float> list = new List<float>();
            List<float> branchList3D = new List<float>();

            // 문자열을 순회하면서 경로를 만든다.
            Stack<Pose> stack = new Stack<Pose>();
            Pose pose = new Pose(Quaternion.Identity, Vertex3f.Zero);

            // 줄기의 밑둥의 큰 원이다.
            Vertex3f[] baseVertor = new Vertex3f[12];
            float radius = 0.2f;
            float unitTheta = 360 / baseVertor.Length;
            for (int i = 0; i < baseVertor.Length; i++)
            {
                baseVertor[i] = new Vertex3f((float)(radius * Math.Cos((unitTheta * i).ToRadian())),
                    (float)(radius * Math.Sin((unitTheta * i).ToRadian())), 0);
            }
            Stack<Vertex3f[]> branchBaseStack = new Stack<Vertex3f[]>();

            int idx = 0;
            float thick = 1.0f;

            while (idx < word.Length)
            {
                if (word.Length == 0) break;
                MChar c = word[idx];
                Matrix4x4f matQuaternion = (Matrix4x4f)pose.Quaternion;
                Vertex3f forward = matQuaternion.ForwardVector();
                Vertex3f up = matQuaternion.UpVector();
                Vertex3f left = matQuaternion.LeftVector();

                if (c.Alphabet == "F")
                {
                    float r = c[0];
                    Vertex3f start = pose.Postiton;
                    Vertex3f end = start + forward * r;

                    list.Add(start.x);
                    list.Add(start.y);
                    list.Add(start.z);
                    list.Add(end.x);
                    list.Add(end.y);
                    list.Add(end.z);

                    (float[] res, Vertex3f[] rot) = LoadBranch(baseVertor, start, end, 0.01f * thick, false);
                    branchList3D.AddRange(res);
                    baseVertor = rot;

                    pose.Postiton = end;
                }
                else if (c.Alphabet == "f")
                {
                    float r = c[0];
                    Vertex3f start = pose.Postiton;
                    Vertex3f end = start + forward * r;

                    list.Add(start.x);
                    list.Add(start.y);
                    list.Add(start.z);
                    list.Add(end.x);
                    list.Add(end.y);
                    list.Add(end.z);

                    pose.Postiton = end;
                }
                else if (c.Alphabet == "+")
                {
                    float angle = c[0];
                    pose.Quaternion = up.Rotate(angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "-")
                {
                    float angle = c[0];
                    pose.Quaternion = up.Rotate(-angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "&")
                {
                    float angle = c[0];
                    pose.Quaternion = left.Rotate(angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "^")
                {
                    float angle = c[0];
                    pose.Quaternion = left.Rotate(-angle).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "\\")
                {
                    pose.Quaternion = forward.Rotate(g["d"]).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "/")
                {
                    pose.Quaternion = forward.Rotate(-g["d"]).Concatenate(pose.Quaternion);
                }
                else if (c.Alphabet == "$")
                {
                    Vertex3f V = Vertex3f.UnitZ;
                    Matrix4x4f mat = ((Matrix4x4f)pose.Quaternion);
                    Vertex3f H = mat.ForwardVector().Normalized;
                    Vertex3f L = V.Cross(H).Normalized;
                    Vertex3f U = H.Cross(L).Normalized;
                    Matrix3x3f m = Matrix3x3f.Identity;
                    m.Column(0, L);
                    m.Column(1, U);
                    m.Column(2, H);
                    pose.Quaternion = m.ToQuaternion();
                }
                else if (c.Alphabet == "!")
                {
                    thick = c[0];
                }
                else if (c.Alphabet == "[")
                {
                    stack.Push(pose);
                    branchBaseStack.Push(baseVertor);
                }
                else if (c.Alphabet == "]")
                {
                    pose = stack.Pop();
                    baseVertor = branchBaseStack.Pop();
                }

                idx++;
            }

            // raw3d 모델을 만든다.
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vbo;
            vbo = StoreDataInAttributeList(0, 3, branchList3D.ToArray());
            Gl.BindVertexArray(0);

            RawModel3d rawModel = new RawModel3d(vao, branchList3D.ToArray());

            return rawModel;
        }

        /// <summary>
        /// 줄기의 모델을 읽어온다.
        /// </summary>
        /// <param name="startNPolygon">이전 단계의 줄기 윗면의 점들</param>
        /// <param name="start">줄기의 시작 지점</param>
        /// <param name="end">줄기가 끝나는 지점</param>
        /// <param name="ratioThick">줄기의 단면의 반지름의 줄어드는 비율</param>
        /// <returns></returns>
        public static (float[], Vertex3f[] endNPolygon) LoadBranch(Vertex3f[] startNPolygon,
            Vertex3f start, Vertex3f end, float ratioThick, bool isRelativesize)
        {
            List<float> positionList = new List<float>();
            int num = startNPolygon.Length;
            Vertex3f[] evec = new Vertex3f[num];

            // 이전 단계의 굵기를 계산하여 다음 단계의 굵기를 계산한다.
            float maxThick = float.MinValue;
            for (int i = 0; i < startNPolygon.Length; i++)
            {
                float d = (startNPolygon[0] - startNPolygon[i]).Norm();
                maxThick = Math.Max(maxThick, d);
            }
            float baseThick = maxThick * 0.5f;
            float terminalThick = baseThick * ratioThick;

            // 이전 단계의 줄기로 새로운 줄기의 윗면의 점들을 계산한다.
            Vertex3f s0 = startNPolygon[0];
            Vertex3f s = s0 - start;
            Vertex3f f = end - start;
            Vertex3f u = f.Cross(s).Normalized;
            Vertex3f l = u.Cross(f).Normalized;
            evec[0] = isRelativesize ? start + f + l * (terminalThick) : start + f + l * (ratioThick);
            float unitDeg = 360.0f / num;

            Vertex3f r0 = evec[0] - start;
            for (int i = 1; i < num; i++)
            {
                Vertex3f src = f.Rotate(r0, unitDeg * i);
                evec[i] = src + start;
            }

            // 아랫면과 윗면의 점들로 옆면을 만든다.
            for (int i = 0; i < num; i++)
            {
                positionList.Add(startNPolygon[(i + 0) % num].x);
                positionList.Add(startNPolygon[(i + 0) % num].y);
                positionList.Add(startNPolygon[(i + 0) % num].z);
                positionList.Add(startNPolygon[(i + 1) % num].x);
                positionList.Add(startNPolygon[(i + 1) % num].y);
                positionList.Add(startNPolygon[(i + 1) % num].z);
                positionList.Add(evec[(i + 0) % num].x);
                positionList.Add(evec[(i + 0) % num].y);
                positionList.Add(evec[(i + 0) % num].z);

                positionList.Add(evec[(i + 0) % num].x);
                positionList.Add(evec[(i + 0) % num].y);
                positionList.Add(evec[(i + 0) % num].z);
                positionList.Add(startNPolygon[(i + 1) % num].x);
                positionList.Add(startNPolygon[(i + 1) % num].y);
                positionList.Add(startNPolygon[(i + 1) % num].z);
                positionList.Add(evec[(i + 1) % num].x);
                positionList.Add(evec[(i + 1) % num].y);
                positionList.Add(evec[(i + 1) % num].z);
            }

            float[] positions = positionList.ToArray();
           
            return (positions, evec);
        }

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
