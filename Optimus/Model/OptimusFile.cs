using System;

namespace Optimus.Model
{
    public class OptimusFile
    {
        public bool Tracked { get; set; }
        public DateTime OptimizedAt { get; set; }
        public string Path { get; set; }
    }
}