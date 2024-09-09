using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using VcCoop.src.models;
using VcCoop.src.utils;

namespace VcCoop.src.handlers
{
    internal class EventHandler
    {
        /// <summary>
        /// Automation instance
        /// </summary>
        private Automation automation;

        public EventHandler(Automation automation)
        {
            this.automation = automation;
        }

        /// <summary>
        /// Runs every time specified in EntityManager class
        /// </summary>
        /// <param name="entity">Entity instance</param>
        public void OnEntityListUpdate(Entity entity)
        {
        }
    }
}
