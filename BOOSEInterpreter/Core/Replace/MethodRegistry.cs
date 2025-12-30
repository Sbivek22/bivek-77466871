using System;
using System.Collections.Generic;
using BOOSEInterpreter.Core.Replace;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Registry that stores method declarations for unrestricted BOOSE programs.
    /// </summary>
    /// <remarks>
    /// The original BOOSE <c>Method</c>/<c>Call</c> support is limited. This registry allows the
    /// interpreter to register, retrieve and replace multiple <see cref="AppMethod"/> declarations
    /// by name. It uses case-insensitive keys and exposes a singleton <see cref="Instance"/> for
    /// global access during program compilation and execution.
    /// </remarks>
    internal sealed class MethodRegistry
    {
        private readonly Dictionary<string, AppMethod> methods = new(StringComparer.OrdinalIgnoreCase);

        private MethodRegistry() { }

        /// <summary>
        /// Singleton instance used by the runtime and compiler to access registered methods.
        /// </summary>
        public static MethodRegistry Instance { get; } = new MethodRegistry();

        /// <summary>
        /// Remove all registered methods from the registry.
        /// </summary>
        public void Clear() => methods.Clear();

        /// <summary>
        /// Register a method declaration. If a method with the same name already exists it is replaced.
        /// </summary>
        /// <param name="method">The <see cref="AppMethod"/> to register.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="method"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the method has an empty or whitespace name.</exception>
        public void Register(AppMethod method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (string.IsNullOrWhiteSpace(method.MethodName))
                throw new ArgumentException("MethodName is required.", nameof(method));

            methods[method.MethodName.Trim()] = method;
        }

        /// <summary>
        /// Retrieve a registered method by name.
        /// </summary>
        /// <param name="methodName">The name of the method to retrieve.</param>
        /// <returns>The registered <see cref="AppMethod"/> instance.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="methodName"/> is null/whitespace or the method is not found.</exception>
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
