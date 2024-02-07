using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    public abstract class ShaderProgram : ShaderUniform
    {
        protected uint _programID;
        private uint _vertexShaderID;
        private uint _geometryShaderID;
        private uint _fragmentShaderID;
        private uint _tcsShaderID;
        private uint _tesShaderID;
        private string _filename;

        protected Dictionary<string, int> _location;

        protected abstract void BindAttributes();

        protected abstract void GetAllUniformLocations();

        public uint ProgramID => _programID;

        public void SetInt(string name, int value)
        {
            Gl.Uniform1(Gl.GetUniformLocation(_programID, name), value);
        }

        public void SetVec3(string uniformName, Vertex3f vec3)
        {
            int loc = Gl.GetUniformLocation(_programID, uniformName);
            base.LoadVector(loc, vec3);
        }

        public void SetVec2(string uniformName, Vertex2f vec2)
        {
            int loc = Gl.GetUniformLocation(_programID, uniformName);
            base.LoadVector(loc, vec2);
        }

        public void SetVec4(string uniformName, Vertex4f vec4)
        {
            int loc = Gl.GetUniformLocation(_programID, uniformName);
            base.LoadVector(loc, vec4);
        }

        public void SetFloat(string uniformName, float value)
        {
            int loc = Gl.GetUniformLocation(_programID, uniformName);
            base.LoadFloat(loc, value);
        }

        public void SetBoolean(string uniformName, bool value)
        {
            int loc = Gl.GetUniformLocation(_programID, uniformName);
            base.LoadBoolean(loc, value);
        }

        public void SetMatrix4x4(string uniformName, Matrix4x4f value)
        {
            int loc = Gl.GetUniformLocation(_programID, uniformName);
            base.LoadMatrix(loc, value);
        }

        public void SetMatrix3x3(string uniformName, Matrix3x3f value)
        {
            int loc = Gl.GetUniformLocation(_programID, uniformName);
            base.LoadMatrix(loc, value);
        }

        public void UniformLocation(string uniformName)
        {
            _location.Add(uniformName, GetUniformLocation(uniformName));
        }

        public void UniformLocations(params string[] uniformNames)
        {
            foreach (string uniformName in uniformNames)
            {
                _location.Add(uniformName, GetUniformLocation(uniformName));
            }
        }

        public ShaderProgram(string vertexFile, string fragmentFile, string geometryFile,
            string tcsFile = "", string tesFile = "")
        {
            Init(vertexFile, fragmentFile, geometryFile, tcsFile, tesFile);
        }

        public void Init(string vertexFile, string fragmentFile, string geometryFile,
            string tcsFile = "", string tesFile = "")
        {

            _filename = vertexFile;

            _location = new Dictionary<string, int>();

            _programID = Gl.CreateProgram();

            string shaderName = Path.GetFileNameWithoutExtension(vertexFile);
            int success = 0;

            if (File.Exists(vertexFile))
            {
                _vertexShaderID = LoadShader(vertexFile, ShaderType.VertexShader);
                if (_vertexShaderID >= 0) success++;
                Gl.AttachShader(_programID, _vertexShaderID);
            }

            if (File.Exists(fragmentFile))
            {
                _fragmentShaderID = LoadShader(fragmentFile, ShaderType.FragmentShader);
                if (_fragmentShaderID >= 0) success++;
                Gl.AttachShader(_programID, _fragmentShaderID);
            }

            if (File.Exists(geometryFile))
            {
                _geometryShaderID = LoadShader(geometryFile, ShaderType.GeometryShader);
                if (_geometryShaderID >= 0) success++;
                Gl.AttachShader(_programID, _geometryShaderID);
            }

            if (File.Exists(tcsFile))
            {
                _tcsShaderID = LoadShader(tcsFile, ShaderType.TessControlShader);
                if (_tcsShaderID >= 0) success++;
                Gl.AttachShader(_programID, _tcsShaderID);
            }

            if (File.Exists(tesFile))
            {
                _tesShaderID = LoadShader(tesFile, ShaderType.TessEvaluationShader);
                if (_tesShaderID >= 0) success++;
                Gl.AttachShader(_programID, _tesShaderID);
            }

            BindAttributes();

            Gl.LinkProgram(_programID);
            Gl.ValidateProgram(_programID);

            Debug.WriteLine($"{shaderName} Shader success num {success}");

            Gl.DeleteShader(_vertexShaderID);
            Gl.DeleteShader(_fragmentShaderID);
            Gl.DeleteShader(_geometryShaderID);
            Gl.DeleteShader(_tcsShaderID);
            Gl.DeleteShader(_tesShaderID);

            GetAllUniformLocations();
        }

        private string[] LoadTextFile(string fileName)
        {
            if (!File.Exists(fileName)) return null;

            StringBuilder shaderSource = new StringBuilder();
            try
            {
                StreamReader sr = new StreamReader(fileName);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    line = IncludeFile(fileName, line);
                    shaderSource.Append(line).Append("\n");
                }
                sr.Close();

            }
            catch (IOException e)
            {
                Debug.WriteLine("Could not read file! " + e.Message);
            }

            string[] shaderSources = new string[shaderSource.Length];
            for (int i = 0; i < shaderSource.Length; i++)
            {
                shaderSources[i] = shaderSource[i].ToString();
            }

            return shaderSources;
        }

        private string IncludeFile(string fileName, string shaderSource)
        {
            string result = "";
            string txt = shaderSource;
            if (txt.StartsWith("//include"))
            {
                string dir = Path.GetDirectoryName(fileName);
                txt = txt.Replace(@"//include", "").Replace("'", "").Replace(";", "").Trim();
                string includeFileName = dir + "\\" + txt;
                string fn = Path.GetFileName(fileName);
                Console.WriteLine($"//{fn} include = " + File.Exists(includeFileName));
                if (File.Exists(includeFileName))
                {
                    string inc = File.ReadAllText(includeFileName);

                    // region 영역을 제거한다. 파일에서 단 한번만 허용한다.
                    int start = inc.IndexOf("//#region");
                    int end = inc.IndexOf("//#endregion");

                    if (start >= 0)
                    {
                        string cutSource = inc.Substring(0, start)
                            + inc.Substring(end + 12);

                        result += cutSource.Replace("\r\n", "\n");
                    }
                    else
                    {
                        result += inc;
                    }
                }
            }
            else
            {
                result += txt;
            }

            //Console.WriteLine(result);
            return result;
        }

        private uint LoadShader(string fileName, ShaderType type)
        {
            if (!File.Exists(fileName)) return 0;

            string[] shaderSources = LoadTextFile(fileName);

            uint shaderID = Gl.CreateShader(type);
            Gl.ShaderSource(shaderID, shaderSources);
            Gl.CompileShader(shaderID);

            int param;
            Gl.GetShader(shaderID, ShaderParameterName.CompileStatus, out param);
            string shortFileName = fileName.Replace(EngineLoop.PROJECT_PATH, "");
            if (param == Gl.FALSE)
            {
                string msg = "[GLSL 컴파일실패]" + type + "=" + shaderID + "\n" + "\nCould not compile shader.\n" + "\n[파일명]" + fileName;
                Debug.WriteLine(msg + $" Shader Program 에러");
            }
            else
            {
                string shaderName = Path.GetFileName(shortFileName);
                Console.WriteLine($"[성공] {shaderName} {type} [{shaderID}]");
            }

            return shaderID;
        }

        protected int GetUniformLocation(string uniformName)
        {
            return Gl.GetUniformLocation(_programID, uniformName);
        }

        protected void BindAttribute(uint attribute, string variableName)
        {
            Gl.BindAttribLocation(_programID, attribute, variableName);
        }

        public void Bind()
        {
            Gl.UseProgram(_programID);
        }

        public void Unbind()
        {
            Gl.UseProgram(0);
        }

        public void CleanUp()
        {
            Unbind();
            if (_vertexShaderID > 0) Gl.DetachShader(_programID, _vertexShaderID);
            if (_fragmentShaderID > 0) Gl.DetachShader(_programID, _fragmentShaderID);
            if (_geometryShaderID > 0)
            {
                if (_geometryShaderID > 0) Gl.DetachShader(_programID, _geometryShaderID);
            }

            if (_vertexShaderID > 0) Gl.DeleteShader(_vertexShaderID);
            if (_fragmentShaderID > 0) Gl.DeleteShader(_fragmentShaderID);
            if (_geometryShaderID > 0) Gl.DeleteShader(_geometryShaderID);

            if (_programID > 0) Gl.DeleteProgram(_programID);
        }


    }
}
