using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace Getequ
{
    public static class ProjectItemHelper
    {
        public static IList<string> GetFileLines(ProjectItem projectItem)
        {
            if (projectItem == null)
                return new List<string>();

            string fileName = projectItem.get_FileNames(0);

            return File.ReadAllLines(fileName);
        }

        public static string GetFileString(ProjectItem projectItem)
        {
            if (projectItem == null)
                return null;

            string fileName = projectItem.get_FileNames(0);

            return File.ReadAllText(fileName);
        }
    }
}
