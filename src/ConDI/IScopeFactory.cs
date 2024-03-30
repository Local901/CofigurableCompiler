using System;
using System.Collections.Generic;
using System.Text;

namespace ConDI
{
    public interface IScopeFactory
    {
        /// <summary>
        /// Create a scope.
        /// </summary>
        /// <returns></returns>
        public Scope CreateScope();

        /// <summary>
        /// Create a scope with a specific parent. The parent doesn't need to know the same dependencies.
        /// </summary>
        /// <param name="parent">The parent scope.</param>
        /// <returns>The created scope.</returns>
        public Scope CreateScope(Scope parent);
    }
}
