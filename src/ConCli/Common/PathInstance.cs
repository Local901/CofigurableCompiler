using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConCli.Common
{
    public struct PathInstance
    {
        public string OriginalPath { get; }
        public readonly string AbsolutePath => Path.GetFullPath(OriginalPath);

        public PathInstance(string path)
        {
            OriginalPath = path;
        }

        public string RelativePath(string relativeTo)
        {
            return Path.GetRelativePath(relativeTo, AbsolutePath);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null) return false;
            if (obj is PathInstance pathInstance) {
                return String.Equals(AbsolutePath, pathInstance.AbsolutePath);
            }
            if (obj is string stringInstance)
            {
                return String.Equals(AbsolutePath, Path.GetFullPath(stringInstance));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return AbsolutePath.GetHashCode();
        }

        public override string ToString()
        {
            return AbsolutePath;
        }
    }
}
