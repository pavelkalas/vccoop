using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VcCoop.src.content;
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
    /// <summary>
    /// MISSION MANAGER
    /// 
    /// Class for managging missions status, informations, reporting etc..
    /// </summary>
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
        private readonly Automation automation;

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
                // restarts the map if ANY player die but zero enemy was shot
                if (player.Dead > 0 && enemy.Dead == 0)
                {
                    automation.InsertToQueue("adminsay \"" + Strings.Get(3) + "\"");

                    Thread.Sleep(2500);

                    for (int i = 0; i < 2; i++)
                    {
                        automation.InsertToQueue("restartmap");
                        Thread.Sleep(1000);
                    }
                }

                // if ANY player die but some enemy was down
                else if (player.Dead > 0 && enemy.Dead > 0)
                {
                    // first attempt failed
                    if (currentMissionAttempt == 1)
                    {
                        automation.InsertToQueue("adminsay \"" + Strings.Get(0) + "\"");
                    }

                    // second attempt failed
                    else if (currentMissionAttempt == maxAttempts - 1)
                    {
                        automation.InsertToQueue("adminsay \"" + Strings.Get(1) + "\"");
                    }

                    // third attempt failed (last)
                    else if (currentMissionAttempt == maxAttempts)
                    {
                        automation.InsertToQueue("adminsay \"" + Strings.Get(2) + "\"");
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

                        // skip the attempt incrementing
                        return;
                    }

                    // increment attempt
                    currentMissionAttempt++;
                }
            }
        }
    }
}
