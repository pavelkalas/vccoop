namespace VcCoop.src.models
{
    internal class Entity
    {
        /// <summary>
        /// Name of entity
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Entity kills
        /// </summary>
        public int Kills { get; protected set; }

        /// <summary>
        /// Entity deaths
        /// </summary>
        public int Deaths { get; protected set; }

        public Entity(string name, int kills, int deaths)
        {
            this.Name = name;
            this.Kills = kills;
            this.Deaths = deaths;
        }
    }
}
