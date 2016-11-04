using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using EnvDTE;
using EnvDTE80;

namespace Getequ.CodeStinck.Models
{
    public class CodeFunctionWrapper
    {
        CodeFunction2 _func;
        CodeClassWrapper _parent;

        public CodeFunctionWrapper(CodeFunction2 func, CodeClassWrapper parent)
        {
            _func = func;
            _parent = parent;
        }

        //public static implicit operator CodeFunction2(CodeFunctionWrapper f)
        //{
        //    return f._func;
        //}

        public string Name { get { return _func.Name; } }

        public bool IsOverride { get { return _func.OverrideKind == vsCMOverrideKind.vsCMOverrideKindOverride; } }

        public bool IsPublic { get { return _func.Access == vsCMAccess.vsCMAccessPublic; } }

        public string DocComment { get { return _func.DocComment; } }

        public IEnumerable<CodeParameter2> Parameters { get { return _func.Parameters.OfType<CodeParameter2>(); } }

        public int StartLine { get { return _func.StartPoint.Line; } }
        public int EndLine { get { return _func.EndPoint.Line; } }
        public Point Range { get { return new Point { X = StartLine, Y = EndLine }; } }

        public IList<string> Code
        {
            get
            {
                return _parent.FileCode.GetRange(_func.StartPoint.Line-1, _func.EndPoint.Line-1);
            }
        }

        public bool ContainsWord(string word) 
        { 
            return Code.Any(x => x.ContainsWord(word));
        }
    }
}
