using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    partial class EngineLoop
    {
		#region ###기초 선언###
		private Vertex2i _mousePosition = Vertex2i.Zero;
		private Vertex2f _mouseDeltaPos = Vertex2f.Zero;

		[DllImport("user32.dll")] private static extern int ShowCursor(bool bShow);

		[DllImport("user32.dll")] static extern bool GetCursorPos(out POINT lpPoint);

		//public const float MOUSE_SENSIBLITY = 1.0f;
		#endregion

		#region ###기초 작업###

		private struct POINT { public int X; public int Y; }
		private static Vertex2i _windowOffSet = Vertex2i.Zero;
		private static Vertex2f _currentMousePointFloat = Vertex2f.Zero;
		private Vertex2f _prevMousePosition;

		public void DetectInput(int ox, int oy, int width, int height)
		{
			_windowOffSet = new Vertex2i(ox, oy);

			POINT point;
			GetCursorPos(out point);
			float fx = (float)(point.X - ox) / (float)width;
			float fy = (float)(point.Y - oy) / (float)height;
			_currentMousePointFloat = new Vertex2f(fx, fy);
			Vertex2f currentPoint = new Vertex2f(fx, fy);
			Vertex2f delta = currentPoint - _prevMousePosition;
			_mouseDeltaPos.x = (float)delta.x;
			_mouseDeltaPos.y = (float)delta.y;
			_mousePosition.x = point.X;
			_mousePosition.y = point.Y;
			//Console.WriteLine($"{mx},{my} {dx},{dy}");
		}

		#endregion
	}
}
