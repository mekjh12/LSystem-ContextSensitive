using System;
using System.Collections.Generic;

namespace LSystem
{
    public delegate float FuncFxF(float a);
    public delegate float FuncFxFF(float a, float b);
    public delegate float FuncFxFFF(float a, float b, float c);
    public delegate bool FuncBxF(float a);
    public delegate bool FuncBxFF(float a, float b);
    public delegate bool FuncBxFFF(float a, float b, float c);

    public struct Module
    {
        string _alphabet;
        float[] _parametric;

        public float[] Parametric
        {
            get => _parametric;
            set => _parametric = value;
        }

        public string Alphabet => _alphabet;

        public Module(string alphabet, int paramCount)
        {
            _alphabet = alphabet;
            _parametric = new float[paramCount];
            for (int i = 0; i < paramCount; i++)
            {
                _parametric[i] = 0.0f;
            }
        }
    }

    public struct Condition
    {
        FuncBxF _condition1;
        FuncBxFF _condition2;
        FuncBxFFF _condition3;

        public Condition(FuncBxF condition)
        {
            _condition1 = condition;
            _condition2 = null;
            _condition3 = null;
        }

        public Condition(FuncBxFF condition)
        {
            _condition1 = null;
            _condition2 = condition;
            _condition3 = null;
        }

        public Condition(FuncBxFFF condition)
        {
            _condition1 = null;
            _condition2 = null;
            _condition3 = condition;
        }

        public bool IsSatisfied(float[] p)
        {
            if (p.Length == 0) return true;
            if (p.Length == 1) return _condition1(p[0]);
            if (p.Length == 2) return _condition2(p[0], p[1]);
            if (p.Length == 3) return _condition3(p[0], p[1], p[2]);
            return false;
        }
    }

    public struct Successor
    {
        SuccessorModule[] _modules;

        public SuccessorModule[] SuccessorModules => _modules;

        public Successor(SuccessorModule[] module)
        {
            _modules = module;
        }

        public Successor(SuccessorModule module)
        {
            _modules = new SuccessorModule[1] { module };
        }

        public Successor(string predecessor, params FuncFxFF[] funcs)
        {
            _modules = new SuccessorModule[1] 
            {
                new SuccessorModule(new Module(predecessor, funcs.Length), funcs)
            };
        }

        public Successor(string predecessor, params FuncFxF[] funcs)
        {
            _modules = new SuccessorModule[1]
            {
                new SuccessorModule(new Module(predecessor, funcs.Length), funcs)
            };
        }

        public Successor(string predecessor, params FuncFxFFF[] funcs)
        {
            _modules = new SuccessorModule[1]
            {
                new SuccessorModule(new Module(predecessor, funcs.Length), funcs)
            };
        }
    }

    public struct SuccessorModule
    {
        Module _successor;
        FuncFxF[] _condition1;
        FuncFxFF[] _condition2;
        FuncFxFFF[] _condition3;

        public string Alphabet => _successor.Alphabet;

        public SuccessorModule(Module successor)
        {
            _successor = successor;
            _condition1 = null;
            _condition2 = null;
            _condition3 = null;
        }

        public SuccessorModule(Module successor, params FuncFxF[] condition)
        {
            _successor = successor;
            _condition1 = condition;
            _condition2 = null;
            _condition3 = null;
        }

        public SuccessorModule(Module successor, params FuncFxFF[] condition)
        {
            _successor = successor;
            _condition1 = null;
            _condition2 = condition;
            _condition3 = null;
        }

        public SuccessorModule(Module successor, params FuncFxFFF[] condition)
        {
            _successor = successor;
            _condition1 = null;
            _condition2 = null;
            _condition3 = condition;
        }
    }

    public struct Predecessor
    {
        Module _successor;

        public string Alphabet => _successor.Alphabet;

        public Predecessor(Module successor)
        {
            _successor = successor;
        }
    }

    public struct Production
    {
        Predecessor _predecessor;
        Condition _condition;
        Successor _successor;
        float _probability;

        public Predecessor Predecessor => _predecessor;

        public Successor Successor => _successor;

        public Condition Condition => _condition;

        public float Probability => _probability;

        public Production(Predecessor predecessor, Condition condition, Successor production, float probability)
        {
            _predecessor = predecessor;
            _successor = production;
            _condition = condition;
            _probability = probability;
        }
    }
}
