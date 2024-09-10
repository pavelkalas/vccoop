using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using VcCoop.src.models;
using VcCoop.src.utils;

namespace VcCoop.src.managers
{
    internal class MissionManager
    {
        /// <summary>
        /// Enemy info structure
        /// </summary>
        struct Enemy
        {
            public int Live;
            public int Dead;
            public int Total;
        }

        /// <summary>
        /// Player info structure
        /// </summary>
        struct Player
        {
            public int Live;
            public int Dead;
            public int Total;
        }

        /// <summary>
        /// Player structure instance
        /// </summary>
        Player player = new Player();

        /// <summary>
        /// Enemy structure instance
        /// </summary>
        Enemy enemy = new Enemy();

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
            // collect informations about players
            player.Total = entities.Where(e => e is models.Player).Count();
            player.Live = entities.Where(e => e is models.Player && e.Deaths == 0).Count();
            player.Dead = entities.Where(e => e is models.Player && e.Deaths > 0).Count();

            // collect informations about enemies
            enemy.Total = entities.Where(e => e is models.Enemy).Count();
            enemy.Live = entities.Where(e => e is models.Enemy && e.Deaths == 0).Count();
            enemy.Dead = entities.Where(e => e is models.Enemy && e.Deaths > 0).Count();

            // just print out for test
            Console.Clear();
            Console.WriteLine("Players:");
            Console.WriteLine(" Total: " + player.Total);
            Console.WriteLine(" Alive: " + player.Live);
            Console.WriteLine("  Dead: " + player.Dead);

            Console.WriteLine("\nEnemy:");
            Console.WriteLine(" Total: " + enemy.Total);
            Console.WriteLine(" Alive: " + enemy.Live);
            Console.WriteLine("  Dead: " + enemy.Dead);
        }
    }
}
