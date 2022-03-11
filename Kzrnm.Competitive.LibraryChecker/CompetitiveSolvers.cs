using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kzrnm.Competitive.LibraryChecker
{
    public static class CompetitiveSolvers
    {
        /// <summary>
        /// get <see cref="ICompetitiveSolver"/> classes in <paramref name="assembly"/>.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ICompetitiveSolver> GetSolvers(Assembly assembly)
            => assembly.GetTypes()
                .OfType<ICompetitiveSolver>();

        /// <summary>
        /// get <see cref="ICompetitiveSolver"/> class having <paramref name="name"/> in <paramref name="assembly"/>.
        /// </summary>
        /// <returns></returns>
        public static ICompetitiveSolver GetSolver(Assembly assembly, string name)
            => assembly.GetTypes()
                .OfType<ICompetitiveSolver>()
                .Single(s => s.Name == name);
    }
}
