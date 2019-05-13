using System;
using Optimus.Helpers;

namespace Optimus.Model
{
    public struct OptimusFileInfo
    {
        public bool Tracked { get; }
        public DateTime? OptimizedAt { get; }
        public string RelativePath { get; }
        
        internal OptimusFileInfo(string relativePath, bool tracked = false, DateTime? optimizedAt = null)
        {
            RelativePath = relativePath.NormalizeSeparators();
            Tracked = tracked;
            OptimizedAt = optimizedAt;
        }
    }
}