using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;

using EnvDTE;
using EnvDTE80;

namespace Getequ.CodeStinck.Models
{
    public class CodeClassWrapper
    {
        CodeClass2 _class;
        CodeClassWrapper _parent;
        IList<string> _code;

        IEnumerable<CodePropertyWrapper> _props;
        IEnumerable<CodeFunctionWrapper> _funcs;
        IEnumerable<CodeClassWrapper> _classes;

        public CodeClassWrapper(CodeClass2 cls, CodeClassWrapper parent = null)
        {
            _class = cls;
            _parent = parent;
            if (_class.ProjectItem == null)
                _code = new List<string>();

            string fileName = _class.ProjectItem.get_FileNames(0);

            _code = File.ReadAllLines(fileName).ToList();

            _funcs = _class.Members.OfType<CodeFunction2>().Select(x => new CodeFunctionWrapper(x, this)).ToList();
            _props = _class.Members.OfType<CodeProperty2>().Select(x => new CodePropertyWrapper(x, this)).ToList();
            _classes = _class.Members.OfType<CodeClass2>().Select(x => new CodeClassWrapper(x, this)).ToList();
        }

        public IEnumerable<CodeFunctionWrapper> Methods { get { return _funcs; } }

        public IEnumerable<CodePropertyWrapper> Properties { get { return _props; } }

        public IEnumerable<CodeClassWrapper> NestedClasses { get { return _classes; } }

        public string Base { get { return _class.Bases.OfType<CodeClass2>().First().Name; } }

        public string Name { get { return _class.Name; } }
        public ProjectItem ProjectItem { get { return _class.ProjectItem; } }

        public int StartLine { get { return _class.StartPoint.Line; } }
        public int EndLine { get { return _class.EndPoint.Line; } }
        public Point Range { get { return new Point { X = StartLine, Y = EndLine }; } }

        public IList<string> FileCode
        {
            get { return _code; }
        }

        public IList<string> Code
        {
            get { return _code.GetRange(_class.StartPoint.Line - 1, _class.EndPoint.Line - 1); }
        }
    }
}
