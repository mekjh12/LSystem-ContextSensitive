using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LSystem
{
    class IniFile
    {
        private static string s_PATH_ROOT = "";
        private static string s_FILENAME = "";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        public static void SetFileName(string fileName)
        {
            s_PATH_ROOT = Application.StartupPath;
            s_PATH_ROOT = Directory.GetParent(s_PATH_ROOT).FullName;
            s_PATH_ROOT = Directory.GetParent(s_PATH_ROOT).FullName;

            IniFile.s_FILENAME = IniFile.s_PATH_ROOT + @"\" + fileName;
            if (!File.Exists(IniFile.s_FILENAME))
            {
                File.Create(IniFile.s_FILENAME);
                Console.WriteLine($"{IniFile.s_FILENAME}을 생성하였습니다.");
            }
        }

        public static void WritePrivateProfileString(string section, string key, int value)
        {
            WritePrivateProfileString(section, key, value.ToString(), IniFile.s_FILENAME);
        }

        public static void WritePrivateProfileString(string section, string key, float value)
        {
            WritePrivateProfileString(section, key, value.ToString(), IniFile.s_FILENAME);
        }

        public static void WritePrivateProfileString(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, IniFile.s_FILENAME);
        }

        public static string GetPrivateProfileString(string section, string key, string defalut)
        {
            StringBuilder sb = new StringBuilder();
            GetPrivateProfileString(section, key, defalut, sb, 32, IniFile.s_FILENAME);
            return sb.ToString();
        }

        public static float GetPrivateProfileFloat(string section, string key, float defalut = 0.0f)
        {
            StringBuilder sb = new StringBuilder();
            GetPrivateProfileString(section, key, "", sb, 32, IniFile.s_FILENAME);
            string res = sb.ToString();
            return (res == "") ? defalut : float.Parse(res);
        }

        public static float[] GetPrivateProfileFloatArray(string section, string key)
        {
            StringBuilder sb = new StringBuilder();
            GetPrivateProfileString(section, key, "", sb, 32, IniFile.s_FILENAME);
            string[] cols = sb.ToString().Split(new char[] { ',' });
            List<float> result = new List<float>();
            for (int i = 0; i < cols.Length; i++)
            {
                result.Add(float.Parse(cols[i]));
            }
            return result.ToArray();
        }
    }
}
