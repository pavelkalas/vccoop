using System.Collections.Generic;

using VcCoop.src.models;
using VcCoop.src.utils;

namespace VcCoop.src.handlers
{
    internal class EventsHandler
    {
        /// <summary>
        /// Automation instance
        /// </summary>
        private Automation automation;

        public EventsHandler(Automation automation)
        {
            this.automation = automation;
        }

        /// <summary>
        /// Runs every time specified in EntityManager class
        /// </summary>
        /// <param name="entities">Entity instance</param>
        public void OnEntityListUpdate(List<Entity> entities)
        {
        }
    }
}
