using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LSystem
{
    class LSystemStochastic
    {
        struct Production
        {
            string successor;
            float probability;

            public string Value => successor;

            public float Probability => probability;

            public Production(string successor, float probability)
            {
                this.successor = successor;
                this.probability = probability;
            }
        }

        int _n;
        float _delta;
        Dictionary<string, List<Production>> _productions;
        Color _branchColor;
        Color _leafColor;
        Random _rnd;

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

        public float Delta => _delta;

        public LSystemStochastic(Random random)
        {
            _branchColor = Color.Brown;
            _leafColor = Color.Green;
            _rnd = random;
        }

        public void AddRule(string predecessor, string successor, float probability)
        {
            if (_productions == null) _productions = new Dictionary<string, List<Production>>();

            Production suc = new Production(successor, probability);

            if (_productions.ContainsKey(predecessor))
            {
                _productions[predecessor].Add(suc);
            }
            else
            {
                List<Production> list = new List<Production>();
                list.Add(suc);
                _productions.Add(predecessor, list);
            }
        }

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
                    AddRule(cols[0].Trim(), cols[1].Trim(), 1.0f);
                }
                else if (cols.Length == 3)
                {
                    AddRule(cols[0].Trim(), cols[1].Trim(), float.Parse(cols[2].Trim()));
                }
            }
        }

        public string GenerateStochastic(string axiom, int n, float delta)
        {
            _n = n;
            _delta = delta;

            string word = axiom;

            // 확률의 합을 1로 만들기

            List<string> list = new List<string>();

            // key 배열 만들기
            foreach (KeyValuePair<string, List<Production>> item in _productions)
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
                            // predecessor들을 모아서 확률의 합을 이용하여 확률로 선택을 한다.
                            List<Production> ps = _productions[keys[j]];
                            float prob = (float)_rnd.NextDouble();

                            float sum = 0.0f;
                            for (int k = 0; k<ps.Count; k++)
                                sum += ps[k].Probability;
                            prob *= sum;

                            int idx = 0;
                            sum = 0.0f;
                            for (int k = 0; k < ps.Count; k++)
                            {
                                sum += ps[k].Probability;
                                if (prob < sum)
                                {
                                    idx = k;
                                    break;
                                }
                            }

                            // idx는 확률적으로 선택된 list index
                            words[i] = ps[idx].Value; //치환

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

        public string Generate(string axiom, int n, float delta)
        {
            _n = n;
            _delta = delta;

            string word = axiom;

            // 확률의 합을 1로 만들기

            List<string> list = new List<string>();

            // key 배열 만들기
            foreach (KeyValuePair<string, List<Production>> item in _productions) 
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
                            Production p = _productions[keys[j]][0];
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


        /// <summary>
        /// 화면의 캔버스에 문장을 분석하여 그린다.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="sentence"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="prad"></param>
        /// <param name="height">캔버스의 높이(반전을 위하여)</param>
        /// <param name="branchLength"></param>
        /// <param name="drawWidth"></param>
        public void Render(Graphics g, string sentence,
            int px, int py, float prad, float height,
            float branchLength, float drawWidth)
        {
            Vertex3f pose = new Vertex3f(px, py, prad);
            string word = sentence;
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



    }
}
