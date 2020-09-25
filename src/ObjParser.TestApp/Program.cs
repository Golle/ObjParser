using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ObjParser.TestApp
{
    class Program
    {

        
        static async Task Main(string[] args)
        {
            var files = Directory.GetFiles(Path.Combine(FindRootFolder(Environment.CurrentDirectory), "resources"), "*.obj", SearchOption.AllDirectories);

            var loader = new ObjLoader();

            foreach (var file in files)
            {
                using (new Timer(Path.GetFileName(file)))
                {
                    try
                    {
                        await loader.LoadFromFile(file);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(file + " failed");
                    }
                }
                
            }
        }
        static string FindRootFolder(string currentPath)
        {
            if (currentPath.EndsWith("src"))
            {
                return Directory.GetParent(currentPath)?.FullName;
            }
            return FindRootFolder(Directory.GetParent(currentPath)?.FullName);
        }
    }


    internal class Timer : IDisposable
    {
        private readonly string _name;
        private readonly Stopwatch _timer = Stopwatch.StartNew();

        public Timer(string name = null)
        {
            _name = name;
        }
        public void Dispose()
        {
            _timer.Stop();
            Console.WriteLine(_name != null
                ? $"[{_name}] Elapsed time: {_timer.Elapsed.Milliseconds} ms"
                : $"Elapsed time: {_timer.Elapsed.Milliseconds} ms");
        }
    }
}
