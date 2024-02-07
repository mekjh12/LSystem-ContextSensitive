using OpenGL;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace LSystem
{
    public partial class EngineLoop
    {
        public static string EXECUTE_PATH;
        public static string PROJECT_PATH;

        private FPSCamera _camera;

        private int _width;
        private int _height;

        private Action<int> _update;
        private Action<int> _render;

        public FPSCamera Camera => _camera;

        public int Width => _width;
        
        public int Height => _height;

        public Action<int> UpdateFrame
        {
            get => _update;
            set => _update = value;
        }
        public Action<int> RenderFrame
        {
            get => _render;
            set => _render = value;
        }

        public EngineLoop()
        {
            EXECUTE_PATH = Application.StartupPath;
            EngineLoop.PROJECT_PATH = Application.StartupPath;
            EngineLoop.PROJECT_PATH = Directory.GetParent(EngineLoop.PROJECT_PATH).FullName;
            EngineLoop.PROJECT_PATH = Directory.GetParent(EngineLoop.PROJECT_PATH).FullName;
        }

        public void Init(int width, int height)
        {
            _width = width;
            _height = height;

            ShowCursor(false);
            Gl.Viewport(0, 0, _width, _height);
        }

        public void Update(int deltaTime)
        {
            if (_camera == null)
            {
                _camera = new FPSCamera("fpsCam", -13, -1.5f, 3, 0, 0);
                _camera.Init(_width, _height);
            }

            KeyCheck(deltaTime);
            _camera.Update(deltaTime);

            if (_update != null) _update(deltaTime);

        }

        public void Render(int deltaTime)
        {
            if (_render != null) _render(deltaTime);
        }

        public void KeyCheck(int deltaTime)
        {
            float milliSecond = deltaTime * 0.001f;
            float cameraSpeed = 1.0f;

            if (Keyboard.IsKeyDown(Key.W)) _camera.GoForward(milliSecond * cameraSpeed);
            if (Keyboard.IsKeyDown(Key.S)) _camera.GoForward(-milliSecond * cameraSpeed);
            if (Keyboard.IsKeyDown(Key.D)) _camera.GoRight(milliSecond * cameraSpeed);
            if (Keyboard.IsKeyDown(Key.A)) _camera.GoRight(-milliSecond * cameraSpeed);
            if (Keyboard.IsKeyDown(Key.E)) _camera.GoUp(milliSecond * cameraSpeed);
            if (Keyboard.IsKeyDown(Key.Q)) _camera.GoUp(-milliSecond * cameraSpeed);
        }

    }
}
