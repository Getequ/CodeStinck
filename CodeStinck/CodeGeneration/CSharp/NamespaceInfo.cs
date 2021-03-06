﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Getequ.CodeGeneration.CSharp
{
    public class NamespaceInfo : BaseCodeFragment
    {
        public List<string> InnerUsing = new List<string>();
        public List<string> OuterUsing = new List<string>();

        public NamespaceInfo(string nsName)
        {
            Name = nsName;
        }

        public override List<string> Generate(int indent = 0)
        {
            List<string> result = new List<string>();

            OuterUsing.ForEach(x => result.Add(("using " + x + ";").Ind(indent)));
            if (OuterUsing.Any())
                result.Add("");

            result.Add(("namespace " + Name).Ind(indent) + Environment.NewLine + "{".Ind(indent));

            InnerUsing.ForEach(x => result.Add(("using " + x + ";").Ind(indent+1)));
            
            if (InnerUsing.Any())
                result.Add("".Ind(indent));

            NestedValues.ForEach(x => { 
                result.AddRange(x.Generate(indent + 1)); 
                result.Add(""); 
            });

            if (string.IsNullOrWhiteSpace(result.Last()))
                result.RemoveAt(result.Count - 1);

            result.Add("}".Ind(indent));

            return result;
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Generate(0));
        }
    }
}
