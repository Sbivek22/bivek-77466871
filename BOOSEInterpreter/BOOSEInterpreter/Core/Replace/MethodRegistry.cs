using System;
using System.Collections.Generic;
using BOOSEInterpreter.Core.Replace;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Runtime registry for method declarations.
    /// 
    /// The official BOOSE <c>Method</c>/<c>Call</c> implementation is restricted. To support
    /// unrestricted programs (multiple methods/calls), the application keeps a dedicated registry
    /// of method declarations.
    /// </summary>
    internal sealed class MethodRegistry
    {
        private readonly Dictionary<string, AppMethod> methods = new(StringComparer.OrdinalIgnoreCase);

        private MethodRegistry() { }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static MethodRegistry Instance { get; } = new MethodRegistry();

        /// <summary>
        /// Remove all registered methods.
        /// </summary>
        public void Clear() => methods.Clear();

        /// <summary>
        /// Register (or replace) a method declaration.
        /// </summary>
        public void Register(AppMethod method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (string.IsNullOrWhiteSpace(method.MethodName))
                throw new ArgumentException("MethodName is required.", nameof(method));

            methods[method.MethodName.Trim()] = method;
        }

        /// <summary>
        /// Find a method by name.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the method does not exist.</exception>
        public AppMethod Get(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new InvalidOperationException("Method name is required.");

            if (!methods.TryGetValue(methodName.Trim(), out var m))
                throw new InvalidOperationException($"Unknown method '{methodName}'.");

            return m;
        }
    }
}
