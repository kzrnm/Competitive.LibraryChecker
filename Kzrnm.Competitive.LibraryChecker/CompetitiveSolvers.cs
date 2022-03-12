using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Kzrnm.Competitive.LibraryChecker
{
    public static class CompetitiveSolvers
    {
        /// <summary>
        /// get <see cref="ICompetitiveSolver"/> classes in <paramref name="assembly"/>.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetSolverTypes(Assembly assembly)
            => assembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(ICompetitiveSolver)) && !t.IsAbstract);

        /// <summary>
        /// get <see cref="ICompetitiveSolver"/> instances in <paramref name="assembly"/>.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ICompetitiveSolver> GetSolvers(Assembly assembly)
            => GetSolverTypes(assembly)
                .Select(type =>
                {
                    var obj = Activator.CreateInstance(type) as ICompetitiveSolver;
                    if (obj == null)
                        ThrowNotHavingDefaultConstructor(type);
                    return obj;
                });

        private static void ThrowNotHavingDefaultConstructor(Type type)
            => throw new InvalidOperationException($"{type.Name} doesn't have default constructor.");

        /// <summary>
        /// get <see cref="ICompetitiveSolver"/> instance having <paramref name="name"/> in <paramref name="assembly"/>.
        /// </summary>
        /// <returns></returns>
        public static ICompetitiveSolver GetSolver(Assembly assembly, string name)
            => GetSolvers(assembly)
                .Single(s => s.Name == name);

        /// <summary>
        /// Run <see cref="ICompetitiveSolver"/> class.
        /// </summary>
        /// <returns></returns>
        public static void RunSolver(Assembly assembly, string name, Stream inputStream, Stream outputStream)
            => GetSolver(assembly, name).Solve(inputStream, outputStream);

        /// <summary>
        /// Run <see cref="ICompetitiveSolver"/> class.
        /// </summary>
        public static void RunSolverWithTimeout(Assembly assembly, string name, Stream inputStream, Stream outputStream)
            => RunSolverWithTimeout(assembly, name, inputStream, outputStream, 1.0);

        /// <summary>
        /// Run <see cref="ICompetitiveSolver"/> class.
        /// </summary>
        public static void RunSolverWithTimeout(Assembly assembly, string name, Stream inputStream, Stream outputStream, double timeoutCoefficient = 1.0)
        {
            var solver = GetSolver(assembly, name);
            var task = Task.Run(() => solver.Solve(inputStream, outputStream));

            if (!task.Wait(TimeSpan.FromSeconds(solver.TimeoutSecond * timeoutCoefficient)))
                throw new Exception($"Solver({name}) is timed out!");
        }
    }
}
