﻿namespace rpg.Models
{
    public class Weapon
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public int Damage { get; set; } 

        public Character Character { get; set; } 

        public int CharacterId { get; set; }    //EF knows this is the foreign key
    }
}
