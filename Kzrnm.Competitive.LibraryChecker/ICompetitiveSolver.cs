using System.IO;

namespace Kzrnm.Competitive.LibraryChecker
{
    /// <summary>
    /// Solver
    /// </summary>
    public interface ICompetitiveSolver
    {
        /// <summary>
        /// Solver name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Timeout of solver
        /// </summary>
        double TimeoutSecond { get; }
        /// <summary>
        /// Run program
        /// </summary>
        void Solve(Stream inputStream, Stream outputStream);
    }
}
