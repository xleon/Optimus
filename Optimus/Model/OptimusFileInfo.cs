using System;
using Optimus.Helpers;

namespace Optimus.Model
{
    public class OptimusFileInfo
    {
        public bool Tracked { get; }
        public DateTimeOffset? OptimizedAt { get; }
        public string RelativePath { get; }
        
        internal OptimusFileInfo(string relativePath, bool tracked = false, DateTimeOffset? optimizedAt = null)
        {
            RelativePath = relativePath.NormalizeSeparators();
            Tracked = tracked;
            OptimizedAt = optimizedAt;
        }
    }
}