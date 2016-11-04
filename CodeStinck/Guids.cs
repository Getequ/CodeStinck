// Guids.cs
// MUST match guids.h
using System;

namespace Getequ.CodeStinck
{
    static class GuidList
    {
        public const string guidVSEnumClassesPkgString = "8f5f9071-1af6-46e1-afd7-84ee1f5edbf2";
        public const string guidVSEnumClassesCmdSetString = "eb6f8d56-f2b9-4df5-b049-e9e03e7b5565";
        public const string guidVSNamespacesCmdSetString = "eb6f8d56-f2b9-4df5-b049-e9e03e7b5561";

        public static readonly Guid guidVSEnumClassesCmdSet = new Guid(guidVSEnumClassesCmdSetString);
        public static readonly Guid guidVSNamespacesCmdSet = new Guid(guidVSNamespacesCmdSetString);
    };
}