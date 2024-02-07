using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LSystem
{
    class LSystem
    {        
        struct Production
        {
            string successor;
            float probability;

            public string Value => successor;

            public Production(string successor, float probability)
            {
                this.successor = successor;
                this.probability = probability;
            }
        }

        int _n;
        float _delta;
        Dictionary<string, Production> _productions;
        Color _branchColor;
        Color _leafColor;

        public Color BranchColor
        {
            get => _branchColor; 
            set => _branchColor = value;
        }

        public Color LeafColor
        {
            get => _leafColor; 
            set => _leafColor = value;
        }

        public float Delta
        {
            get => _delta;
            set => _delta = value;
        }

        public LSystem()
        {
            _branchColor = Color.Brown;
            _leafColor = Color.Green;
        }


        public void AddRule(string predecessor, string successor, float probability)
        {
            if (_productions == null) _productions = new Dictionary<string, Production>();

            Production suc = new Production(successor, probability);

            if (_productions.ContainsKey(predecessor))
            {
                _productions[predecessor] = suc;
            }
            else
            {
                _productions.Add(predecessor, suc);
            }
        }

        /*
        public void AddRule(string predecessor,string successor)
        {
            if (_productions == null) _productions = new Dictionary<string, string>();

            if (_productions.ContainsKey(predecessor))
            {
                _productions[predecessor] = successor;
            }
            else
            {
                _productions.Add(predecessor, successor);
            }
        }
        */

        /// <summary>
        /// * 생성하여 문장을 파일에 저장한다. <br/>
        /// * 생성하는 시간이 걸리기 때문에 빠른 로딩을 위하여 활용한다.<br/>
        /// </summary>
        /// <param name="axiom"></param>
        /// <param name="n"></param>
        /// <param name="delta"></param>
        /// <param name="filename"></param>
        public void GenerateToFile(string axiom, int n, float delta, string filename)
        {
            string words = Generate(axiom, n, delta);
            File.WriteAllText(filename, words);
        }

        /// <summary>
        /// * 문장을 파일로부터 읽어온다.<br/>
        /// * 생성하는 시간이 걸리기 때문에 빠른 로딩을 위하여 활용한다.<br/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GenerateFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }

            return null;
        }

        public string Generate(string axiom, int n, float delta)
        {
            _n = n;
            _delta = delta;

            string word = axiom;

            // 확률의 합을 1로 만들기

            List<string> list = new List<string>();

            // key 배열 만들기
            foreach (KeyValuePair<string, Production> item in _productions)
                list.Add(item.Key);
            string[] keys = list.ToArray();

            for (int a = 0; a < n; a++)
            {
                // 단어들로 자르기
                int last = word.Length;
                list.Clear(); // 재활용
                for (int i = 0; i < last; i++)
                {
                    bool included = false;

                    for (int j = 0; j < keys.Length; j++)
                    {
                        string key = keys[j];
                        if (word[i] == key[0])
                        {
                            int length = key.Length;
                            if (i + length <= last)
                            {
                                string cutWord = word.Substring(i, length);
                                if (cutWord == key)
                                {
                                    list.Add(cutWord);
                                    included = true;
                                    i += length - 1; // break후에 +1을 해주므로 -1을 한다.
                                    break;
                                }
                            }
                        }
                    }

                    if (!included)
                    {
                        list.Add(word[i].ToString());
                    }
                }

                string[] words = list.ToArray();

                // 자른 단어를 치환한다.
                for (int i = 0; i < words.Length; i++)
                {
                    for (int j = 0; j < keys.Length; j++)
                    {
                        if (words[i] == keys[j])
                        {
                            Production p = _productions[keys[j]];
                            words[i] = p.Value;
                            break;
                        }
                    }
                }

                // 단어를 이어붙인다.
                string sentence = "";
                for (int i = 0; i < words.Length; i++)
                {
                    sentence += words[i];
                }

                word = sentence;
            }

            return word;
        }

        /*
        public string GenerateNew(string axiom, int n, float delta)
        {
            _n = n;
            _delta = delta;

            string word = axiom;

            // p1, ... ,pn의 rule이 있을 때, p1을 대체하고
            // p2의 predecessor가 p1의 successor에 있는 경우
            // 순차적으로 문자열을 대체하는 것을 막기 위하여
            // 아래와 같은 mask 문자열을 이용한다.

            for (int i = 0; i < _n; i++)
            {
                string[] mask = new string[word.Length];

                // predecessor를 mask에 저장한다.
                foreach (KeyValuePair<string, string> item in _productions)
                {
                    string predecessor = item.Key;
                    for (int j = 0; j < word.Length - predecessor.Length + 1; j++)
                    {
                        if (word.Substring(j, predecessor.Length) == predecessor)
                        {
                            mask[j] = predecessor;
                        }
                    }
                }

                // mask를 순회하면서 새로운 문자열을 이어붙인다.
                // 만약 null이면 oldWord의 문자열을 그대로 가져온다.
                string newWord = "";
                for (int j = 0; j < mask.Length; j++)
                {
                    if (mask[j] != null)
                    {
                        string predecessor = mask[j];
                        string succesor = _productions[predecessor];
                        newWord += succesor;
                    }
                    else
                    {
                        newWord += word.Substring(j, 1);
                    }
                }

                word = newWord;
            }

            return word;
        }
        */

        public void LoadProductionFromFile(string fileName)
        {
            LoadProductions(File.ReadAllText(fileName));
        }

        public void LoadProductions(string grammer)
        {
            string[] lines = grammer.Split(new char[] { '\n' });
            for (int i = 0; i < lines.Length; i++)
            {
                string[] cols = lines[i].Split(new char[] { ',' });
                if (cols.Length == 2)
                {
                    AddRule(cols[0].Trim(), cols[1].Trim(), 0.0f);
                }
                else if (cols.Length == 3)
                {
                    AddRule(cols[0].Trim(), cols[1].Trim(), float.Parse(cols[2].Trim()));
                }
            }
        }

        public void Render(Graphics g, string axiom,
            int px, int py, float prad, float height,
            float branchLength, float drawWidth)
        {
            Vertex3f pose = new Vertex3f(px, py, prad);
            string word = Generate(axiom, 5, 4);
            float r = (float)branchLength;

            // draw mode
            g.Clear(Color.Gray);
            Stack<Vertex3f> stack = new Stack<Vertex3f>();

            Vertex2f end = new Vertex2f();
            Vertex2f start = new Vertex2f();

            Pen pen = new Pen(_branchColor, drawWidth);

            while (true)
            {
                if (word.Length == 0) break;
                char c = word[0];
                word = word.Substring(1);

                if (c == 'F' || c == 'X')
                {
                    start.x = pose.x;
                    start.y = pose.y;
                    float deg = pose.z;
                    float rad = deg * 3.141502f / 180.0f;
                    end.x = (float)(r * Math.Cos(rad)) + start.x;
                    end.y = (float)(r * Math.Sin(rad)) + start.y;
                    g.DrawLine(pen, start.x, height - start.y, end.x, height - end.y);
                    pose.x = end.x;
                    pose.y = end.y;
                }
                else if (c == '+')
                {
                    pose.z += _delta;
                }
                else if (c == '-')
                {
                    pose.z -= _delta;
                }
                else if (c == '[')
                {
                    stack.Push(new Vertex3f(pose.x, pose.y, pose.z));
                }
                else if (c == ']')
                {
                    pose = stack.Pop();
                }
            }
        }

        public void RenderRndColorRewriting(Graphics g, string axiom, int px, int py, float prad, float height, float BranchLength)
        {
            Vertex3f pose = new Vertex3f(px, py, prad);
            string word = Generate(axiom, 5, 4);
            float r = (float)BranchLength;

            // draw mode
            g.Clear(Color.Gray);
            Stack<Vertex3f> stack = new Stack<Vertex3f>();
            Stack<Pen> color = new Stack<Pen>();

            Random rnd = new Random();

            Vertex2f end = new Vertex2f();
            Vertex2f start = new Vertex2f();

            Pen pen = new Pen(Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255)), 10);
            color.Push(pen);

            while (true)
            {
                if (word.Length == 0) break;
                char c = word[0];
                word = word.Substring(1);

                if (c == 'F' || c == 'X')
                {
                    start.x = pose.x;
                    start.y = pose.y;
                    float deg = pose.z;
                    float rad = deg * 3.141502f / 180.0f;
                    end.x = (float)(r * Math.Cos(rad)) + start.x;
                    end.y = (float)(r * Math.Sin(rad)) + start.y;
                    g.DrawLine(pen, start.x, height - start.y, end.x, height - end.y);
                    pose.x = end.x;
                    pose.y = end.y;
                }
                else if (c == '+')
                {
                    pose.z += _delta;
                }
                else if (c == '-')
                {
                    pose.z -= _delta;
                }
                else if (c == '[')
                {
                    color.Push(pen);
                    pen = new Pen(Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255)), 0.5f * pen.Width);
                    stack.Push(new Vertex3f(pose.x, pose.y, pose.z));
                }
                else if (c == ']')
                {
                    pose = stack.Pop();
                    pen = color.Pop();
                }
            }
        }
    }
}
