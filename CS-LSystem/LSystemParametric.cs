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

        public void AddRule(string alphabet, int varCount, GlobalParam g,
            Condition condition,  MultiVariableFunc func, float probability = 1.0f)
        {
            MChar predecessor = new MChar(alphabet, varCount);
            Production production = new Production(condition, g, func);
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
                MString newString = MString.Null;

                // 줄의 문자마다 순회한다.
                foreach (MChar inChar in mString)
                {
                    // 규칙마다 순회한다.
                    bool isBreak = false;
                    foreach (KeyValuePair<MChar, List<Production>> items in _productions)
                    {
                        MChar mchar = items.Key;
                        if (mchar.IsSameClass(inChar))
                        {
                            List<Production> prods = items.Value;
                            foreach (Production prod in prods)
                            {
                                if (prod.Condition(inChar))
                                {
                                    newString += prod.Func(inChar, prod.GlobalParam);
                                    isBreak = true;
                                    break;
                                }
                            }
                        }
                        if (isBreak) break;
                    }
                    if (!isBreak) newString += inChar;
                }
                mString = newString;
                //Console.WriteLine(i + "=" + newString);
            }
            return mString;
        }
    }
}
