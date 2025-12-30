using System;
using System.Collections.Generic;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Runtime helper that manages active method call frames for BOOSE programs.
    /// </summary>
    /// <remarks>
    /// Method calls are represented by a <see cref="MethodFrame"/> that stores the return
    /// program counter and the list of local variables created for the call. The runtime
    /// maintains a stack of frames; callers push a frame before jumping into the method
    /// body and pop the frame when the method returns.
    /// </remarks>
    internal sealed class MethodRuntime
    {
        private readonly Stack<MethodFrame> frames = new();

        private MethodRuntime() { }

        public static MethodRuntime Instance { get; } = new MethodRuntime();

        /// <summary>
        /// Gets a value indicating whether there is at least one active method frame.
        /// </summary>
        public bool HasFrame => frames.Count > 0;

        /// <summary>
        /// Clear all active call frames. This should be called when a new program run starts
        /// to ensure no stale frames from a previous (failed) execution remain.
        /// </summary>
        public void Clear() => frames.Clear();

        /// <summary>
        /// Push a new <see cref="MethodFrame"/> onto the call stack.
        /// </summary>
        /// <param name="frame">The frame representing the active method call.</param>
        public void Push(MethodFrame frame) => frames.Push(frame);

        /// <summary>
        /// Pop the top <see cref="MethodFrame"/> from the call stack.
        /// </summary>
        /// <returns>The popped <see cref="MethodFrame"/>.</returns>
        /// <exception cref="InvalidOperationException">If the call stack is empty.</exception>
        public MethodFrame Pop()
        {
            if (frames.Count == 0)
                throw new InvalidOperationException("Method call stack underflow.");
            return frames.Pop();
        }
    }

    /// <summary>
    /// Represents an active method call frame.
    /// </summary>
    /// <remarks>
    /// A frame stores the method name (for diagnostics), the return program counter
    /// and the list of local variable names that need to be cleaned up when the method returns.
    /// </remarks>
    internal sealed class MethodFrame
    {
        /// <summary>
        /// Create a new method frame.
        /// </summary>
        /// <param name="methodName">The method's name.</param>
        /// <param name="returnPc">The program counter to return to after the call.</param>
        /// <param name="locals">The list of local variable names introduced by the method.</param>
        public MethodFrame(string methodName, int returnPc, IReadOnlyList<string> locals)
        {
            MethodName = methodName;
            ReturnPc = returnPc;
            Locals = locals;
        }

        /// <summary>The name of the method.</summary>
        public string MethodName { get; }
        /// <summary>The program counter to return to after the method completes.</summary>
        public int ReturnPc { get; }
        /// <summary>The names of the local variables declared by the method.</summary>
        public IReadOnlyList<string> Locals { get; }
    }
}
