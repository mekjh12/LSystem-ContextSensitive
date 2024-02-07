using OpenGL;
using System;

namespace LSystem
{
    public class Rand
    {
        private static Random _rnd = new Random();

        /// <summary>
        /// 0.0보다 크거나 같고 1.0보다 작은 부동 소수점 난수입니다.
        /// </summary>
        /// <returns></returns>
        public static float NextFloat
        {
            get
            {
                return (float)_rnd.NextDouble();
            }
        }

        /// <summary>
        ///  -1.0보다 크거나 같고 1.0보다 작은 부동 소수점 난수입니다.
        /// </summary>
        public static float NextFloat2
        {
            get
            {
                return (float)(2.0f * _rnd.NextDouble() - 1.0f);
            }
        }


        /// <summary>
        /// start보다 크거나 같고 end보다 작은 부동 소수점 난수입니다.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static float Next(float start = 0.0f, float end = 1.0f)
        {
            return start + (end - start) * (float)_rnd.NextDouble();
        }

        /// <summary>
        /// 지정된 범위 내의 임의의 정수를 반환합니다.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int Next(int start = 0, int end = 100)
        {
            return _rnd.Next(start, end);
        }

        /// <summary>
        /// 색깔의 랜덤을 반환한다. 0부터 1이다.
        /// </summary>
        /// <returns></returns>
        public static float NextColor => (float)_rnd.NextDouble();

        /// <summary>
        /// 0.0보다 크거나 같고 1.0보다 작은 부동 소수점 난수입니다.
        /// </summary>
        /// <returns></returns>
        public static double NextDouble()
        {
            return _rnd.NextDouble();
        }

        public static int NextInt(int min = -1, int max = 1)
        {
            return _rnd.Next(min, max);
        }

        public static Vertex3f NextColor3f => new Vertex3f(NextColor, NextColor, NextColor);

    }
}
