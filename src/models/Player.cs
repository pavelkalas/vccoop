namespace VcCoop.src.models
{
    internal class Player : Entity
    {
        /// <summary>
        /// Player ping
        /// </summary>
        public int Ping { get; private set; }

        public Player(string name, int kills, int deaths, int ping) : base(name, kills, deaths)
        {
            this.Ping = ping;
        }

        /// <summary>
        /// Override object method .ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Name: {this.Name}, Kills: {this.Kills}, Deaths: {this.Deaths}, Ping: {this.Ping}";
    }
}
