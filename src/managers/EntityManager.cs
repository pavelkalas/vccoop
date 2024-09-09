using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VcCoop.src.models;
using VcCoop.src.utils;

namespace VcCoop.src.managers
{
    internal class EntityManager
    {
        /// <summary>
        /// Interval to delay next check step.
        /// </summary>
        private readonly int checkInterval;

        /// <summary>
        /// Main task timer
        /// </summary>
        private Timer taskTimer;

        /// <summary>
        /// Automation instance
        /// </summary>
        private Automation automation;

        public EntityManager(int checkInterval, Automation automation)
        {
            this.checkInterval = (checkInterval >= 1000) ? checkInterval : 1000;
            this.automation = automation;
        }

        /// <summary>
        /// Start the task.
        /// </summary>
        public void StartTask()
        {
            taskTimer = new Timer(Task, null, 0, checkInterval);
        }

        /// <summary>
        /// Stop the running task.
        /// </summary>
        public void StopTask()
        {
            taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            taskTimer.Dispose();
        }

        /// <summary>
        /// Main task method.
        /// </summary>
        private void Task(Object obj)
        {
            // clears the listbox (to get only "list" command data)
            automation.SetEmptyElement("ListBox");

            // sends list to vcguard window, to retrieve player data
            automation.InsertToQueue("list");

            // wait some time to make sure all data from LIST are retrieved
            Thread.Sleep(200);

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

            // just for testing purposes!
            // ...
            Console.Clear();
            // ...
            foreach (Entity entity in entityList)
            {
                if (entity is Player || entity is Enemy)
                {
                    Console.WriteLine(entity);
                }
            }
        }
    }
}
