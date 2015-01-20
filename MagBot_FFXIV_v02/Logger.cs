using System;
using System.IO;

namespace MagBot_FFXIV_v02
{
    public class Logger : IDisposable
    {
        //Log4Net seems to use Singleton pattern

        private readonly StreamWriter _w;

        public Logger(string filename)
        {
            _path = MainForm.FFXIVFolderPath + @"\" + filename + ".txt";
            _w = File.CreateText(_path);
            //_w = File.AppendText(_path);

            _w.WriteLine("");
            _w.WriteLine("STARTING NEW: " + filename);
            _w.WriteLine("==================================");
        }

        private readonly string _path;

        public void Log(string logMessage)
        {
            _w.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " | " + logMessage);
            Console.WriteLine(logMessage);
        }

        public void DumpLog()
        {
            using (StreamReader r = File.OpenText(_path))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _w.Dispose();
        }
    }
}
