using System.Diagnostics;

namespace Esercizio_async_stampa_quarta
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var docs = new List<(string nome, int mb)> {
                    ("libro1", 5),
                    ("libro2", 2),
                    ("libro3", 8),
                    ("libro4", 3)
            };

            Console.WriteLine("=== DOWNLOAD SEQUENZIALE ===");
            long tempoSequenziale = await SequantialPrintDocument(docs);

            Console.WriteLine("\n=== DOWNLOAD PARALLELO ===");
            long tempoParallelo = await ParallelPrintDocument(docs);

            Console.WriteLine("\n=== REPORT FINALE ===");
            Console.WriteLine($"Tempo sequenziale: {tempoSequenziale} ms");
            Console.WriteLine($"Tempo parallelo:   {tempoParallelo} ms");
        }

        static async Task<string> PrintDocument(string nome, int numPagine)
        {
            Console.WriteLine($"Inziato stampa di {nome} (che ha {numPagine} pag)");

            await Task.Delay(numPagine * 300);

            return ($"Terminato stampa di {nome} (che ha {numPagine} pag)");
        }

        static async Task<long> SequantialPrintDocument(List<(string nome, int numPagine)> documenti)
        {
            /*
            Stopwatch sw = Stopwatch.StartNew();

            sw.Stop();
            return sw.ElapsedMilliseconds;
            */

            DateTime start = DateTime.Now;

            foreach (var document in documenti)
            {
                Console.WriteLine(await PrintDocument(document.nome, document.numPagine));
            }

            double seconds = (DateTime.Now - start).TotalMilliseconds;

            return (long)seconds;
        }

        static async Task<long> ParallelPrintDocument(List<(string nome, int numPagine)> documenti)
        {
            Stopwatch sw = Stopwatch.StartNew();

            List<Task<string>> tasks = new();
            foreach (var document in documenti)
            {
                tasks.Add(PrintDocument(document.nome, document.numPagine));
            }

            List<Task<string>> tasksRimanenti = new(tasks);

            while (tasksRimanenti.Count > 0)
            {
                Task<string> finito = await Task.WhenAny(tasksRimanenti);
                Console.WriteLine(">> " + finito.Result);
                tasksRimanenti.Remove(finito);
            }


            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}
