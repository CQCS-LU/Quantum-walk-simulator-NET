
namespace CQCS.QuantumWalks.Common
{
    public interface IQuantumWalkSimulator
    {
        /// <summary>
        /// Gets the total number of vertices.
        /// </summary>
        int N { get; }

        /// <summary>
        /// Gets current step.
        /// </summary>
        int T { get; }

        /// <summary>
        /// Performs the specified number of quantum walk steps.
        /// </summary>
        void Run(int stepCount = 1);

        /// <summary>
		/// Returns a scalar product of the current and the initial state.
		/// </summary>
		double GetScalarProduct();

        /// <summary>
        /// Returns a total probability of all marked vertices.
        /// </summary>
        double GetMarkedVertexProbability();

#if DEBUG
        /// <summary>
        /// Returns a total probability of all vertices.
        /// </summary>
        double GetTotalProbability();
#endif
    }
}
