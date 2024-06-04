using ConDI;

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
        /// Add a connection from one step to another.
        /// </summary>
        /// <param name="fromStep"></param>
        /// <param name="toStep"></param>
        void AddConnection(Connection from, Connection to);
    }
}
