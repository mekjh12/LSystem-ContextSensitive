using System;
using System.Collections.Generic;

namespace LSystem
{
    public class LSystemParametric
    {
        Dictionary<MChar, List<Production>> _productions;
        Random _rnd;

        public LSystemParametric(Random random)
        {
            _rnd = random;
        }

        public void AddRule(string alphabet, int varCount, string leftContext, int leftVarCount,
            GlobalParam g,
            Condition condition, MultiVariableFunc func, float probability = 1.0f)
        {
            MChar predecessor = new MChar(alphabet, varCount);
            MChar left = new MChar(leftContext, leftVarCount);

            Production production = new Production(condition, g, func, left, MChar.Null);
            AddRule(predecessor, production, probability);
        }

        public void AddRule(string alphabet, int varCount, GlobalParam g,
            Condition condition,  MultiVariableFunc func, float probability = 1.0f)
        {
            MChar predecessor = new MChar(alphabet, varCount);
            Production production = new Production(condition, g, func, MChar.Null, MChar.Null);
            AddRule(predecessor, production, probability);
        }

        public void AddRule(MChar predecessor, Production production, float probability = 1.0f)
        {
            // p1: A(x,y) : y<3 => A(2x, x+y)
            if (_productions == null) _productions = new Dictionary<MChar, List<Production>>();

            if (_productions.ContainsKey(predecessor))
            {
                _productions[predecessor].Add(production);
            }
            else
            {
                List<Production> list = new List<Production>();
                list.Add(production);
                _productions.Add(predecessor, list);
            }
        }

        public MString Generate(MString axiom, int num)
        {
            MString mString = axiom;

            for (int i = 0; i < num; i++)
            {
                MString oldString = mString;

                Stack<MChar> stack = new Stack<MChar>();
                Stack<int> indexStack = new Stack<int>();
                int chNum = 0;

                MString[] nstr = new MString[oldString.Length]; // nstr 초기화
                for (int j = 0; j < nstr.Length; j++)
                {
                    nstr[j] = oldString[j].ToMString();
                }

                // (1) 줄의 문자마다 순회한다.
                for (int k = 0; k < oldString.Length; k++)
                {
                    MChar inChar = oldString[k];

                    // 스택을 이용하여 트리구조의 leftChar를 쌓아 경로를 만든다.
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

                    string leftContext = stack.String();
                    MChar temp = stack.Pop();
                    MChar leftChar = (stack.Count==0)? MChar.Null: stack.Peek();
                    stack.Push(temp);

                    // (2) 키에 맞는 Productions 규칙마다 순회한다.
                    foreach (KeyValuePair<MChar, List<Production>> items in _productions)
                    {
                        MChar mchar = items.Key;
                        if (!mchar.IsSameNumOfInParameter(inChar)) continue;
                        List<Production> prods = items.Value;

                        foreach (Production prod in prods)
                        {
                            //===============================================================================
                            // 하나의 Production에 대하여 조사한다.
                            //===============================================================================
                            // left context가 없으면,                               
                            if (prod.Left == MChar.Null)
                            {
                                // Production Condition을 만족하면
                                if (prod.Condition(inChar, leftChar, MChar.Null))
                                {
                                    nstr[k] = prod.Func(inChar, leftChar, MChar.Null, prod.GlobalParam); // 치환
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
                                        nstr[k] = prod.Func(inChar, leftChar, MChar.Null, prod.GlobalParam); // 치환
                                    }
                                }
                            }
                            //===============================================================================
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
                Console.WriteLine(i + "=" + newString);
                oldString = newString;
            }


            return mString;
        }
    }
}
