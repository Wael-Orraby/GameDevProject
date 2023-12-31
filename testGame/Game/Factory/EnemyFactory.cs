﻿using GameDevProject.Game;
using GameDevProject.Game.Character;

namespace testGame.Game.Factory
{
    public class EnemyFactory
    {
        private Level level;

        public EnemyFactory(Level level)
        {
            this.level = level;
        }

        public Enemy CreateEnemy(Microsoft.Xna.Framework.Vector2 position, string spriteSet)
        {
            Enemy enemy = new Enemy(level, position, spriteSet);
            enemy.LoadContent(spriteSet);
            return enemy;
        }
    }
}
