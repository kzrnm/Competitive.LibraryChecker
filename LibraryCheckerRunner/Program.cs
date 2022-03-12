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
    public class TwoSatSolver : ICompetitiveSolver
    {
        public string Name => "two_sat";
        public double TimeoutSecond => 5;
        public void Solve(Stream inputStream, Stream outputStream)
        {
            var utf8 = new System.Text.UTF8Encoding(false);
            var cr = new ConsoleReader(inputStream, utf8);
            using var cw = new ConsoleWriter(outputStream, utf8);
            _ = cr.Ascii();
            _ = cr.Ascii();
            int n = cr;
            int m = cr;
            var twoSat = new TwoSat(n);
            for (int i = 0; i < m; i++)
            {
                int a = cr;
                int b = cr;
                _ = cr.Int();

                int a1 = Math.Abs(a) - 1;
                bool a2 = a >= 0;
                int b1 = Math.Abs(b) - 1;
                bool b2 = b >= 0;
                twoSat.AddClause(a1, a2, b1, b2);
            }
            if (twoSat.Satisfiable())
            {
                cw.WriteLine("s SATISFIABLE");
                cw.StreamWriter.Write("v ");
                var res = new int[n + 1];
                var answer = twoSat.Answer();
                for (int i = 0; i < n; i++)
                {
                    if (answer[i])
                        res[i] = i + 1;
                    else
                        res[i] = -(i + 1);
                }
                cw.WriteLineJoin(res);
            }
            else
                cw.WriteLine("s UNSATISFIABLE");
        }
    }
}
