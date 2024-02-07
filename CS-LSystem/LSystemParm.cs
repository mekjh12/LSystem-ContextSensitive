using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LSystem
{
    public delegate bool Condition(MChar mchar);

    public delegate MString MultiVariableFunc(MChar mchar, GlobalParam g);

    public class GlobalParam
    {
        Dictionary<string, float> _value;

        public GlobalParam()
        {
            _value = new Dictionary<string, float>();
        }

        public void Add(string key, float value)
        {
            _value.Add(key, value);
        }

        public float this[string key] => _value[key];
        
    }

    public struct Production
    {
        MultiVariableFunc _func;
        Condition _condition;
        GlobalParam _g;

        public GlobalParam GlobalParam => _g;

        public MultiVariableFunc Func => _func;

        public Condition Condition => _condition;

        public Production(Condition condition, GlobalParam g, MultiVariableFunc func)
        {
            _func = func;
            _g = g;
            _condition = condition;
        }
    }

    public struct MChar
    {
        string _alphabet;
        float[] _parametric;
        
        public int Length => _parametric.Length;

        public float this[int i] => _parametric[i];

        public float[] Parametric
        {
            get => _parametric;
            set => _parametric = value;
        }

        public string Alphabet => _alphabet;

        public static MChar Char(string alphabet, params float[] values) => new MChar(alphabet, values);

        public static MChar A => new MChar("A");

        public static MChar Plus => new MChar("+");

        public static MChar Minus => new MChar("-");

        public static MChar Open => new MChar("[");

        public static MChar Close => new MChar("]");

        public static MChar PitUp => new MChar("^");

        public static MChar PitDown => new MChar("&");

        public static MChar RollLeft => new MChar("\\");

        public static MChar RollRight => new MChar("/");

        public MChar(string alphabet, int varCount)
        {
            _alphabet = alphabet;
            _parametric = new float[varCount];
            for (int i = 0; i < varCount; i++)
            {
                _parametric[i] = 0.0f;
            }
        }

        public MChar(string alphabet, params float[] param)
        {
            _alphabet = alphabet;
            _parametric = param;
        }

        public bool IsSameClass(MChar mchar)
        {
            return _alphabet == mchar.Alphabet && Length == mchar.Length;
        }

        public override string ToString()
        {
            string txt = "";
            txt += $"{_alphabet}(";
            for (int i = 0; i < _parametric.Length; i++)
            {
                txt += _parametric[i] + ((i < _parametric.Length - 1) ? "," : "");
            }
            txt += ")";
            return txt;
        }

        public static MString operator +(MChar a, MChar b)
        {
            return new MString(new MChar[] { a, b });
        }

        public MString ToMString()=> new MString(new MChar[] { this });
    }

    public class MString: IEnumerator, IEnumerable
    {
        MChar[] _chars;
        int position = -1;

        public static MString Null => new MString();

        public int Length => _chars.Length;

        public MChar this[int i] => _chars[i];

        public MString(params MChar[] mchar)
        {
            _chars = mchar;
        }

        public object Current => _chars[position];

        public override string ToString()
        {
            string txt = "";
            foreach (MChar item in _chars)
            {
                int count = item.Parametric.Length;
                if (count == 0)
                {
                    txt += item.Alphabet;
                }
                else
                {
                    txt += $"{item.Alphabet}(";
                    for (int i = 0; i < count; i++)
                    {
                        txt += item.Parametric[i]
                            + ((i < item.Parametric.Length - 1) ? "," : "");
                    }
                    txt += ")";
                }
            }
            return txt;
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _chars.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        public static MString operator +(MString a, MString b)
        {
            MString mString = new MString();
            List<MChar> list = new List<MChar>();
            list.AddRange(a._chars);
            list.AddRange(b._chars);
            mString._chars = list.ToArray();
            return mString;
        }

        public static MString operator +(MString a, MChar b)
        {
            MString mString = new MString();
            List<MChar> list = new List<MChar>();
            list.AddRange(a._chars);
            list.Add(b);
            mString._chars = list.ToArray();
            return mString;
        }
    }
}
