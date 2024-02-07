using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LSystem
{
    public class Texture
    {
        private string _fileName; // 다른 텍스처와 비교하여 중복저장을 막기 위한 변수

        private uint _textureID;

        private int _width;
        private int _height;

        public string FileName
        {
            get => _fileName;
            set => _fileName = value;
        }

        public uint TextureID
        {
            get => _textureID;
            set => _textureID = value;
        }

        public Texture(string filename) : this((Bitmap)Bitmap.FromFile(filename))
        {
            _fileName = filename;
        }

        public Texture(Bitmap bitmap)
        {
            _width = bitmap.Width;
            _height = bitmap.Height;

            this._textureID = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, _textureID);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, data.Width, data.Height, 0,
                 OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        public void Bind(TextureUnit textureUnit)
        {
            Gl.ActiveTexture(textureUnit);
            Gl.BindTexture(TextureTarget.Texture2d, _textureID);
        }

        void Bind(ShaderProgram shader, TextureUnit textureUnit, string uniformTextureName)
        {
            int unit = (int)TextureUnit.Texture0;
            shader.SetInt(uniformTextureName, (int)textureUnit - unit);
            Gl.ActiveTexture(textureUnit);
            Gl.BindTexture(TextureTarget.Texture2d, _textureID);
        }

        public void Clear()
        {
            List<uint> ids = new List<uint>();
            if (_textureID > 0) ids.Add(_textureID);
            Gl.DeleteTextures(ids.ToArray());
        }

    }
}
