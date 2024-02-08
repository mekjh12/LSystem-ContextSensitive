using System;
using System.Collections.Generic;
using static Khronos.Platform;

namespace LSystem
{
    public class LSystemParametric
    {
        Dictionary<string, List<Production>> _productions;
        Random _rnd;

        public LSystemParametric(Random random)
        {
            _rnd = random;
        }

        public void AddRule(string key, string alphabet, int varCount, string leftContext, int leftVarCount,
            GlobalParam g,
            Condition condition, MultiVariableFunc func, float probability = 1.0f)
        {
            MChar predecessor = new MChar(alphabet, varCount);
            MChar left = new MChar(leftContext, leftVarCount);

            Production production = new Production(condition, g, func, predecessor, left, MChar.Null, probability);
            AddRule(key, production);
        }

        public void AddRule(string key, string alphabet, int varCount, GlobalParam g,
            Condition condition,  MultiVariableFunc func, float probability = 1.0f)
        {
            MChar predecessor = new MChar(alphabet, varCount);
            Production production = new Production(condition, g, func, predecessor, MChar.Null, MChar.Null, probability);
            AddRule(key, production);
        }

        public void AddRule(string key, Production production)
        {
            // p1: A(x,y) : y<3 => A(2x, x+y)
            if (_productions == null) _productions = new Dictionary<string, List<Production>>();

            if (_productions.ContainsKey(key))
            {
                _productions[key].Add(production);
            }
            else
            {
                List<Production> list = new List<Production>();
                list.Add(production);
                _productions.Add(key, list);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axiom"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public MString Generate(MString axiom, int num)
        {
            MString mString = axiom;

            for (int i = 0; i < num; i++)
            {
                MString oldString = mString;

                Stack<MChar> stack = new Stack<MChar>();
                Stack<int> indexStack = new Stack<int>();
                int chNum = 0;

                // nstr 초기화하여 oldstring->nstr로 복사한다.
                MString[] nstr = new MString[oldString.Length]; 
                for (int j = 0; j < nstr.Length; j++)
                    nstr[j] = oldString[j].ToMString();

                // (1) 줄의 문자마다 순회한다.
                for (int k = 0; k < oldString.Length; k++)
                {
                    MChar inChar = oldString[k];

                    // 스택을 이용하여 트리구조의 leftChar를 쌓아 경로를 만든다.
                    #region
                    if (inChar.Alphabet == "[") 
                    {
                        indexStack.Push(chNum);
                        chNum = 0;
                    }
                    else if (inChar.Alphabet == "]")
                    {
                        for (int j = 0; j < chNum; j++) stack.Pop();
                        chNum = indexStack.Pop();
                    }
                    else
                    {
                        stack.Push(inChar);
                        chNum++;
                    }
                    #endregion

                    // left Context를 위한 context-load
                    #region
                    string leftContext = stack.String();
                    MChar temp = stack.Pop();
                    MChar leftChar = (stack.Count==0)? MChar.Null: stack.Peek();
                    stack.Push(temp);
                    #endregion

                    // (2) 키에 맞는 Productions 규칙마다 순회한다.
                    foreach (KeyValuePair<string, List<Production>> items in _productions)
                    {
                        // 조건에 맞는 Production 만 선별한다.
                        List<Production> satisfiedProd = new List<Production>();
                        foreach (Production prod in items.Value) // 만족하는 Production을 모은다.
                        {
                            MChar mchar = prod.Predecessor;
                            if (!mchar.IsSameNumOfInParameter(inChar)) continue; // 매개변수 수가 일치하는가?

                            // left context가 없으면,                               
                            if (prod.Left == MChar.Null)
                            {
                                // Production Condition을 만족하면
                                if (prod.Condition(inChar, leftChar, MChar.Null))
                                {
                                    satisfiedProd.Add(prod);
                                }
                            }
                            // left context가 있으면,
                            else
                            {
                                if (prod.Left.Alphabet == leftChar.Alphabet)
                                {
                                    // Production Condition을 만족하면
                                    if (prod.Condition(inChar, leftChar, MChar.Null))
                                    {
                                        satisfiedProd.Add(prod);
                                    }
                                }
                            }
                        }

                        // 랜덤사건을 통하여 해당되는 Production을 고른다.
                        #region
                        float sum = 0.0f; // 해당 키의 확률의 합을 구한다.
                        foreach (Production prod in satisfiedProd)
                            sum += prod.Probability;

                        float incidentProbability = (float)_rnd.NextDouble(); // 사건의 확률을 만든다.
                        incidentProbability *= sum;
                        int indexIncident = 0;
                        sum = 0.0f;
                        for (int a = 0; a < satisfiedProd.Count; a++)
                        {
                            sum += satisfiedProd[a].Probability;
                            if (incidentProbability < sum)
                            {
                                indexIncident = a;
                                break;
                            }
                        }
                        #endregion
                        
                        //Console.WriteLine($"{inChar.Alphabet} satisfiedProd Count=" + satisfiedProd.Count + $" sel={indexIncident}");

                        // 치환해야 할 Production을 실행한다. 
                        foreach (Production prod in satisfiedProd)
                        {
                            nstr[k] = prod.Func(inChar, leftChar, MChar.Null, prod.GlobalParam); // 치환
                        }

                    }
                }

                //
                MString newString = MString.Null;
                for (int j = 0; j < nstr.Length; j++)
                {
                    newString += nstr[j];
                }

                mString = newString;
                Console.WriteLine($"{i+1} = {newString}");
                oldString = newString;
            }

            return mString;
        }
    }
}
