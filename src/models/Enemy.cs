namespace VcCoop.src.models
{
    internal class Enemy : Entity
    {
        public Enemy(string name, int kills, int deaths) : base(name, kills, deaths)
        {
        }

        /// <summary>
        /// Override object method .ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Name: {this.Name}, Kills: {this.Kills}, Deaths: {this.Deaths}";
    }
}
