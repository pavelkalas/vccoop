using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using VcCoop.src.handlers;
using VcCoop.src.models;
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


namespace VcCoop.src.managers
{
    internal class EntityManager
    {
        /// <summary>
        /// Interval to delay next check step.
        /// </summary>
        private readonly int checkInterval;

        /// <summary>
        /// Automation instance
        /// </summary>
        private readonly Automation automation;

        /// <summary>
        /// Event handler instance
        /// </summary>
        private readonly EventsHandler eventsHandler;

        /// <summary>
        /// Controls when thread should start and when stop
        /// </summary>
        private bool taskRunning;

        public EntityManager(int checkInterval, Automation automation)
        {
            this.checkInterval = (checkInterval >= 1000) ? checkInterval : 1000;
            this.automation = automation;
            this.eventsHandler = new EventsHandler(automation);
        }

        /// <summary>
        /// Start the task.
        /// </summary>
        public void StartTask()
        {
            taskRunning = true;
            new Thread(Task).Start();
        }

        /// <summary>
        /// Stop the running task.
        /// </summary>
        public void StopTask()
        {
            taskRunning = false;
        }

        /// <summary>
        /// Main task method.
        /// </summary>
        private void Task()
        {
            while (taskRunning)
            {
                // clears the listbox (to get only "list" command data)
                automation.SetEmptyElement("ListBox");

                // sends list to vcguard window, to retrieve player data
                automation.InsertToQueue("list");

                // wait some time to make sure all data from LIST are retrieved
                Thread.Sleep(500);

                // get all data from console
                List<string> rawData = new List<string>(automation.GetElementData("ListBox"));

                // returns it when nobody is on server.
                // (this basically can happen when server crash, so server can be empty)
                if (rawData == null)
                {
                    return;
                }

                // filtered data from console (selected only players line, removed the time in front of each string)
                rawData = rawData
                    .Where(ent => ent.Contains("kills:") && ent.Contains("deaths:") && ent.Contains("ping:") && ent.Length > 10) // select only content which contains kills, deaths and ping string
                    .Select(ent => ent.Substring(10).Trim()) // remove the time in front of each entry
                    .ToList(); // cast to list

                // use regular expression to extract kills, deaths, and ping values
                var regex = new Regex(@"kills:\s*(\d+)\s*deaths:\s*(\d+)\s*ping:\s*(\d+)");

                // Parse the data like name, kills, deaths and ping
                List<Entity> entityList = rawData
                    .Select(entry =>
                    {
                        string playerData = entry.Split(']')[1].Trim();

                        string name = playerData.Split(new[] { ' ' }, 2)[0];

                        string statsData = playerData.Substring(name.Length).Trim();

                        var match = regex.Match(statsData);

                        if (match.Success)
                        {
                            int kills = int.Parse(match.Groups[1].Value);
                            int deaths = int.Parse(match.Groups[2].Value);
                            int ping = int.Parse(match.Groups[3].Value);

                            // if IS ping equal to ZERO, it's NPC, not a player
                            //
                            // (but sometimes it can cause confusion, because
                            // when a player join a server, it shows ping 0
                            // for some little time.. until it establish)
                            if (ping == 0)
                            {
                                return new Enemy(name, kills, deaths);
                            }
                            else
                            {
                                return new Player(name, kills, deaths, ping);
                            }
                        }

                        return new Entity(null, 0, 0);
                    })
                    .ToList();

                eventsHandler.OnEntityListUpdate(entityList);

                Thread.Sleep(checkInterval);
            }
        }
    }
}
