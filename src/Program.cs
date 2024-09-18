using System;
using System.Diagnostics;
using System.Linq;
using VcCoop.src.content;
using VcCoop.src.managers;
using VcCoop.src.utils;

// Copyright (c) 2024 Pavel Kalaš. All rights reserved.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to use
// the Software for personal, educational, and research purposes, including the
// rights to use, copy, modify, merge, publish, distribute copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// The Software is provided "as is", without warranty of any kind, express or implied,
// including but not limited to the warranties of merchantability, fitness for a particular
// purpose and noninfringement. In no event shall the authors or copyright holders be liable
// for any claim, damages or other liability, whether in an action of contract, tort or
// otherwise, arising from, out of or in connection with the Software or the use or other
// dealings in the Software.
// 
// Distribution and/or publication of the Software, modified or unmodified, to the public
// is strictly prohibited.
// 
// Developed by Pavel Kalaš 2024

namespace VcCoop
{
    internal class Program
    {
        /// <summary>
        /// Automation instance
        /// </summary>
        private static Automation automation;

        /// <summary>
        /// Entity manager instance
        /// </summary>
        private static EntityManager entityManager;

        static void Main(string[] args)
        {
            /*
             * TO START THIS PROGRAM, YOU NEED TO CREATE A BATCH FILE WITH FOLLOWING CONTENT
             * 
             * cd %~dp0%
             * VcCoop.EXE [port]  (replace [port] with a port number of your vcguard server)
             * 
             * !!! MAKE SURE YOU RUN THE BATCH FILE AS ADMINISTRATOR !!!
             * 
             */

            // check for commandline arguments
            if (args.Length > 0)
            {
                // get port argument, and if is argument an integer, try to bind as port
                if (int.TryParse(args[0].Trim(), out int port))
                {
                    // get process of vcguard by PORT in TITLE of gui window
                    Process process = Process.GetProcesses().Where(proc => proc.MainWindowTitle.EndsWith(port.ToString()) && proc.ProcessName == "vcded").FirstOrDefault();

                    // check if is process not null
                    if (process != null)
                    {
                        // initialize instance of automation and entity manager
                        automation = new Automation(process.Id, 1000);
                        entityManager = new EntityManager(1000, automation);

                        // load strings into memory
                        Strings.Load();

                        // start main task (message queue)
                        automation.StartTask();

                        // start entity listener task
                        entityManager.StartTask();

                        Console.WriteLine("INFO : RUNNING OK!");
                    }
                    else
                    {
                        // server not found at specified port, abort!
                        Console.WriteLine("\nERROR : Cannot find server process! Try again!\n\nPress any key to continue...");
                    }
                }
                else
                {
                    // if the port is not a valid integer
                    Console.WriteLine("\nERROR : Port number must be an integer!\n\nPress any key to continue...");
                }
            }
            else
            {
                // no console arguments given, abort!
                Console.WriteLine("\nERROR : Missing startup arguments!\n\nUse:\n\n  VcCoop.EXE [server port]\n\n  example: VcCoop.EXE 5425\n\nPress any key to continue...");
            }

            // wait for key to avoid closing console.
            Console.ReadLine();
        }
    }
}
