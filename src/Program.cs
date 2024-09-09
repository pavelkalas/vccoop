using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcCoop.src.managers;
using VcCoop.src.utils;

namespace VcCoop
{
    internal class Program
    {
        private static readonly string vcguardPort = "5425";
        
        private static Automation automation;

        private static EntityManager entityManager;

        static void Main(string[] args)
        {
            automation = new Automation(Process.GetProcesses().Where(proc => proc.MainWindowTitle.EndsWith(vcguardPort)).FirstOrDefault().Id, 1000);
            entityManager = new EntityManager(1000, automation);

            automation.StartTask();
            entityManager.StartTask();

            Console.ReadLine();
        }
    }
}
