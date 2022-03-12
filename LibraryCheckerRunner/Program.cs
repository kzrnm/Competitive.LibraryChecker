using AtCoder;
using Kzrnm.Competitive.IO;
using Kzrnm.Competitive.LibraryChecker;
using System;
using System.IO;

namespace LibraryCheckerRunner
{
    internal class Program
    {
        static void Main()
        {
            new UnionFindSolver().Solve(Console.OpenStandardInput(), Console.OpenStandardOutput());
        }
    }

    public class UnionFindSolver : ICompetitiveSolver
    {
        public string Name => "unionfind";
        public double TimeoutSecond => 5;
        public void Solve(Stream inputStream, Stream outputStream)
        {
            var utf8 = new System.Text.UTF8Encoding(false);
            var cr = new ConsoleReader(inputStream, utf8);
            using var cw = new ConsoleWriter(outputStream, utf8);
            int n = cr;
            int q = cr;

            var dsu = new DSU(n);

            for (int i = 0; i < q; i++)
            {
                int t = cr;
                int u = cr;
                int v = cr;
                if (t == 0)
                    dsu.Merge(u, v);
                else
                    cw.WriteLine(dsu.Same(u, v) ? 1 : 0);
            }
        }
    }
}
