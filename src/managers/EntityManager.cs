using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public EntityManager(int checkInterval)
        {
            this.checkInterval = (checkInterval >= 1000) ? checkInterval : 1000;
            
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
            // just for testing case
            Console.WriteLine("tick!");
        }
    }
}
