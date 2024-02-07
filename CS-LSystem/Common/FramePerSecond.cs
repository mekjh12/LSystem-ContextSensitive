using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    class FramePerSecond
    { 
        // 마지막 프레임과 현재 프레임 사이의 시간
        static int deltaTime = 0;
        static int globalTick = 0;

        // 마지막 프레임의 시간
        static int lastFrame = 0;

        static float fps = 0.0f;
        static int prevDeltaTime;
        static float weightedValueTime = 0.005f;

        public static int DeltaTime => deltaTime;

        public static int GlobalTick => globalTick;

        public static uint FPS => (uint)fps;

        public static void Update()
        {
            int currentTick = System.Environment.TickCount;
            deltaTime = currentTick - lastFrame;
            if (deltaTime <= 0) deltaTime = 1;
            prevDeltaTime = deltaTime;

            float deltaAverage = (float)1000.0f / (float)(deltaTime);
            float wd = weightedValueTime * deltaTime;
            wd = wd.Clamp(0, 1);
            fps = (1 - wd) * fps + wd * deltaAverage;

            lastFrame = currentTick;
            globalTick++;
        }
    }
}
