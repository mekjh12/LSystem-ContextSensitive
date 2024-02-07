using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LSystem
{
    static class KeyBoard
    {
        private static Dictionary<System.Windows.Input.Key, bool> prevPressed
            = new Dictionary<System.Windows.Input.Key, bool>();

        public static bool IsKeyPress(System.Windows.Input.Key key)
        {
            if (prevPressed.ContainsKey(key))
            {
                return (System.Windows.Input.Keyboard.IsKeyDown(key) == false && prevPressed[key] == true);
            }

            return false;
        }

        public static bool GetKey(System.Windows.Input.Key key)
        {
            return System.Windows.Input.Keyboard.IsKeyDown(key);
        }

        public static void Init()
        {
            foreach (System.Windows.Input.Key key in Enum.GetValues(typeof(System.Windows.Input.Key)))
            {
                if (!prevPressed.ContainsKey(key))
                {
                    prevPressed.Add(key, false);
                }
            }
        }

        public static void Update()
        {
            foreach (System.Windows.Input.Key key in Enum.GetValues(typeof(System.Windows.Input.Key)))
            {
                if (key == System.Windows.Input.Key.None) continue;
                if (prevPressed.ContainsKey(key))
                {
                    prevPressed[key] = System.Windows.Input.Keyboard.IsKeyDown(key);
                }
            }
        }

    }
}
