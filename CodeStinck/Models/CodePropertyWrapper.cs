using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using EnvDTE;
using EnvDTE80;

namespace Getequ.CodeStinck.Models
{
    public class CodePropertyWrapper
    {
        CodeProperty2 _prop;
        CodeClassWrapper _parent;

        public CodePropertyWrapper(CodeProperty2 func, CodeClassWrapper parent)
        {
            _prop = func;
            _parent = parent;
        }

        public string Name { get { return _prop.Name; } }

        public string Type { get { return _prop.Type.CodeType.Name; } }

        public int StartLine { get { return _prop.StartPoint.Line; } }
        public int EndLine { get { return _prop.EndPoint.Line; } }

        public Point Range { get { return new Point { X = StartLine, Y = EndLine }; } }

        public IList<string> Code
        {
            get
            {
                return _parent.FileCode.GetRange(_prop.StartPoint.Line-1, _prop.EndPoint.Line-1);
            }
        }

        public bool ContainsWord(string word)
        { 
            return Code.Any(x => x.ContainsWord(word));
        }
    }
}
