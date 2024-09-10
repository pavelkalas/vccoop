using System;
using System.Collections.Generic;
using System.Linq;
using VcCoop.src.managers;
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

        /// <summary>
        /// Mission manager
        /// </summary>
        private MissionManager missionManager;

        public EventsHandler(Automation automation)
        {
            this.automation = automation;
            this.missionManager = new MissionManager(automation);
        }

        /// <summary>
        /// Runs every time specified in EntityManager class
        /// </summary>
        /// <param name="entities">Entity instance</param>
        public void OnEntityListUpdate(List<Entity> entities)
        {
            missionManager.OnDataTick(entities);
        }
    }
}
