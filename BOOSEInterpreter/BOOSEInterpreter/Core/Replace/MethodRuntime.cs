using System;
using System.Collections.Generic;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Very small runtime helper for BOOSE methods.
    /// BOOSE method calls need a return address + a list of local variables to clean up.
    /// </summary>
    internal sealed class MethodRuntime
    {
        private readonly Stack<MethodFrame> frames = new();

        private MethodRuntime() { }

        public static MethodRuntime Instance { get; } = new MethodRuntime();

        public bool HasFrame => frames.Count > 0;

        /// <summary>
        /// Clear all active call frames. This should be called when a new program run starts
        /// to ensure no stale frames from a previous (failed) execution remain.
        /// </summary>
        public void Clear() => frames.Clear();

        public void Push(MethodFrame frame) => frames.Push(frame);

        public MethodFrame Pop()
        {
            if (frames.Count == 0)
                throw new InvalidOperationException("Method call stack underflow.");
            return frames.Pop();
        }
    }

    /// <summary>
    /// Data that represents one active method call.
    /// </summary>
    internal sealed class MethodFrame
    {
        public MethodFrame(string methodName, int returnPc, IReadOnlyList<string> locals)
        {
            MethodName = methodName;
            ReturnPc = returnPc;
            Locals = locals;
        }

        public string MethodName { get; }
        public int ReturnPc { get; }
        public IReadOnlyList<string> Locals { get; }
    }
}
