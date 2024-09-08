using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcCoop.src.utils;

namespace VcCoop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int processId = Process.GetProcesses().Where(p => p.MainWindowTitle.EndsWith("5425")).Select(p => p.Id).FirstOrDefault();

            Automation auto = new Automation(processId);

            auto.InsertToQueue("Hello");

        }
    }
}
