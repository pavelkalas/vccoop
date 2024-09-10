using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VcCoop.src.models;
using VcCoop.src.utils;

namespace VcCoop.src.managers
{
    internal class MissionManager
    {
        /// <summary>
        /// Automation instance
        /// </summary>
        private Automation automation;

        public MissionManager(Automation automation)
        {
            this.automation = automation;
        }

        public void OnDataTick(List<Entity> entities)
        {
        }
    }
}
