using ConDI;
using ConLine.Steps;
using System.Collections.Generic;

namespace ConLine
{
    public interface IPipeLine : IStep
    {
        public IInjectorSetup InjectorSetup { get; }
        /// <summary>
        /// Can the pipeline still be upated.
        /// </summary>
        public bool CanBeEdited { get; }
        /// <summary>
        /// Add a step to the pipeline.
        /// </summary>
        /// <param name="step">The step to be added.</param>
        void AddStep(IStep step);
        /// <summary>
        /// Get a step of this pipeline.
        /// </summary>
        /// <param name="name">The name of the step.</param>
        /// <returns>The step with that name if found else null.</returns>
        IStep? GetStep(string name);

        /// <summary>
        /// Get all the connection that start from the provided connection point.
        /// </summary>
        /// <param name="from">The connection the connections start from.</param>
        /// <returns>List of connections.</returns>
        IReadOnlyList<Connection> GetConnectionsFrom(Connection from);

        /// <summary>
        /// Get the connection leading to the provided connection point.
        /// </summary>
        /// <param name="to">The connection the connection connects to.</param>
        /// <returns>The connection origin if there is one.</returns>
        Connection? GetConnectionTo(Connection to);

        /// <summary>
        /// Get all steps of type <seealso cref="Input"/>.
        /// </summary>
        /// <returns>Array of <seealso cref="Input">Inputs</seealso></returns>
        public Input[] GetInputSteps();

        /// <summary>
        /// Get all steps of type <seealso cref="Output"/>.
        /// </summary>
        /// <returns>Array of <seealso cref="Output">Outputs</seealso></returns>
        public Output[] GetOutputSteps();

        /// <summary>
        /// Add a connection from one step to another.
        /// </summary>
        /// <param name="fromStep"></param>
        /// <param name="toStep"></param>
        void AddConnection(Connection from, Connection to);
    }
}
