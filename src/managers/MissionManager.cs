using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
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

        /// <summary>
        /// Maximum mission attempts
        /// </summary>
        private const int maxAttempts = 3;

        /// <summary>
        /// Actual mission attempts
        /// </summary>
        private int currentMissionAttempt = 1;

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

            // do logic only when is some player on server
            if (player.Total > 0)
            {
                if (player.Dead > 0 && enemy.Dead == 0)
                {
                    automation.InsertToQueue("adminsay \"Starting the mission attempt...\"");

                    Thread.Sleep(2500);

                    for (int i = 0; i < 2; i++)
                    {
                        automation.InsertToQueue("restartmap");
                        Thread.Sleep(1000);
                    }
                }
                else if (player.Dead > 0 && enemy.Dead > 0)
                {
                    // first attempt failed
                    if (currentMissionAttempt == 1)
                    {
                        automation.InsertToQueue("adminsay \"Mission failed! Try again..\"");
                    }

                    // second attempt failed
                    else if (currentMissionAttempt == maxAttempts - 1)
                    {
                        automation.InsertToQueue("adminsay \"Mission failed! This is your last cahnce..\"");
                    }

                    // third attempt failed (last)
                    else if (currentMissionAttempt == maxAttempts)
                    {
                        automation.InsertToQueue("adminsay \"Last attempt failed, switching to nextmap\"");
                    }

                    if (currentMissionAttempt != maxAttempts)
                    {
                        Thread.Sleep(2500);

                        for (int i = 0; i < 2; i++)
                        {
                            automation.InsertToQueue("restartmap");
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(2500);

                        // switch the map
                        automation.InsertToQueue("nextmap");
                        
                        // resets the attempt count to 1
                        currentMissionAttempt = 1;

                        // do mission stats record..
                        // ...
                    }

                    currentMissionAttempt++;
                }
            }
        }
    }
}
