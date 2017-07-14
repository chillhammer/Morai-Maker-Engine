package dk.itu.mario.level;

import java.util.Random;

import dk.itu.mario.MarioInterface.Constraints;
import dk.itu.mario.MarioInterface.GamePlay;
import dk.itu.mario.MarioInterface.LevelInterface;
import dk.itu.mario.engine.sprites.SpriteTemplate;
import dk.itu.mario.engine.sprites.Enemy;

public class MyLevel extends Level
{
    private static long lastSeed;
    private static Random levelSeedRandom;

    private int difficulty;
    private int type;

    public MyLevel(int width, int height)
    {
        super(width, height);
    }

    public MyLevel(int width, int height, long seed, int difficulty, int type, GamePlay playerMetrics)
    {
        this(width, height);
        this.difficulty = difficulty;
        this.type = type;
        lastSeed = seed;
        levelSeedRandom = new Random(seed);

        setBlock(0, 5, BLOCK_EMPTY);
        setBlock(2, 5, BLOCK_EMPTY);
        setBlock(4, 6, BLOCK_EMPTY);
        setBlock(6, 6, BLOCK_EMPTY);
        setBlock(8, 6, BLOCK_EMPTY);
        setBlock(10, 7, BLOCK_EMPTY);
        setBlock(12, 7, BLOCK_EMPTY);
        setBlock(14, 8, BLOCK_EMPTY);
        setBlock(16, 9, BLOCK_EMPTY);
        setBlock(18, 9, BLOCK_EMPTY);
        setBlock(20, 10, BLOCK_EMPTY);
        setBlock(22, 10, BLOCK_EMPTY);
        setBlock(24, 11, BLOCK_EMPTY);
        setBlock(26, 11, BLOCK_EMPTY);
        setBlock(28, 11, BLOCK_EMPTY);
        setBlock(30, 12, BLOCK_EMPTY);
        setBlock(32, 12, BLOCK_EMPTY);
        setBlock(34, 12, BLOCK_EMPTY);
        setBlock(36, 12, BLOCK_EMPTY);
        setBlock(38, 12, BLOCK_EMPTY);
        setBlock(40, 12, BLOCK_EMPTY);
        setBlock(42, 12, BLOCK_EMPTY);
        setBlock(44, 12, BLOCK_EMPTY);
        setBlock(46, 12, BLOCK_EMPTY);
        setBlock(48, 12, BLOCK_EMPTY);
        setBlock(62, 10, BLOCK_EMPTY);
        setBlock(66, 9, BLOCK_EMPTY);
        setBlock(71, 10, BLOCK_EMPTY);
        setBlock(1, 5, BLOCK_EMPTY);
        setBlock(3, 5, BLOCK_EMPTY);
        setBlock(5, 6, BLOCK_EMPTY);
        setBlock(7, 6, BLOCK_EMPTY);
        setBlock(9, 7, BLOCK_EMPTY);
        setBlock(11, 7, BLOCK_EMPTY);
        setSpriteTemplate(11, 6, new SpriteTemplate(Enemy.ENEMY_GOOMBA, false));
        setBlock(13, 7, BLOCK_EMPTY);
        setBlock(15, 8, BLOCK_EMPTY);
        setBlock(17, 9, BLOCK_EMPTY);
        setBlock(19, 10, BLOCK_EMPTY);
        setBlock(21, 10, BLOCK_EMPTY);
        setBlock(23, 11, BLOCK_EMPTY);
        setBlock(25, 11, BLOCK_EMPTY);
        setBlock(27, 11, BLOCK_EMPTY);
        setSpriteTemplate(27, 10, new SpriteTemplate(Enemy.ENEMY_GREEN_KOOPA, false));
        setSpriteTemplate(38, 11, new SpriteTemplate(Enemy.ENEMY_GREEN_KOOPA, false));
        setSpriteTemplate(18, 8, new SpriteTemplate(Enemy.ENEMY_GOOMBA, false));
        setBlock(29, 12, BLOCK_EMPTY);
        setBlock(31, 12, BLOCK_EMPTY);
        setBlock(33, 12, BLOCK_EMPTY);
        setBlock(35, 12, BLOCK_EMPTY);
        setBlock(37, 12, BLOCK_EMPTY);
        setBlock(39, 12, BLOCK_EMPTY);
        setBlock(41, 12, BLOCK_EMPTY);
        setBlock(43, 12, BLOCK_EMPTY);
        setBlock(45, 12, BLOCK_EMPTY);
        setBlock(47, 12, BLOCK_EMPTY);
        setBlock(49, 12, BLOCK_EMPTY);
        setBlock(51, 11, BLOCK_EMPTY);
        setBlock(52, 11, BLOCK_EMPTY);
        setBlock(53, 10, BLOCK_EMPTY);
        setBlock(54, 10, BLOCK_EMPTY);
        setBlock(55, 10, BLOCK_EMPTY);
        setBlock(56, 10, BLOCK_EMPTY);
        setBlock(57, 10, BLOCK_EMPTY);
        setBlock(58, 10, BLOCK_EMPTY);
        setBlock(59, 10, BLOCK_EMPTY);
        setBlock(60, 10, BLOCK_EMPTY);
        setBlock(61, 10, BLOCK_EMPTY);
        setBlock(63, 10, BLOCK_EMPTY);
        setBlock(65, 9, BLOCK_EMPTY);
        setBlock(64, 9, BLOCK_EMPTY);
        setBlock(3, 6, BLOCK_EMPTY);
        setBlock(8, 7, BLOCK_EMPTY);
        setBlock(13, 8, BLOCK_EMPTY);
        setBlock(15, 9, BLOCK_EMPTY);
        setBlock(18, 10, BLOCK_EMPTY);
        setBlock(22, 11, BLOCK_EMPTY);
        setBlock(29, 11, BLOCK_EMPTY);
        setBlock(52, 10, BLOCK_EMPTY);
        setBlock(50, 11, BLOCK_EMPTY);
        setBlock(49, 11, BLOCK_EMPTY);
        setBlock(63, 9, BLOCK_EMPTY);
        setBlock(72, 10, BLOCK_EMPTY);
        setBlock(79, 11, BLOCK_EMPTY);
        setBlock(80, 11, BLOCK_EMPTY);
        setBlock(81, 11, BLOCK_EMPTY);
        setBlock(87, 11, BLOCK_EMPTY);
        setBlock(88, 11, BLOCK_EMPTY);
        setBlock(89, 11, BLOCK_EMPTY);
        setBlock(77, 11, BLOCK_EMPTY);
        setBlock(78, 11, BLOCK_EMPTY);
        setBlock(90, 11, BLOCK_EMPTY);
        setBlock(91, 11, BLOCK_EMPTY);
        setBlock(82, 11, BLOCK_EMPTY);
        setBlock(83, 11, BLOCK_EMPTY);
        setBlock(84, 11, BLOCK_EMPTY);
        setBlock(85, 11, BLOCK_EMPTY);
        setBlock(86, 11, BLOCK_EMPTY);
        setBlock(68, 6, COIN);
        setBlock(69, 6, COIN);
        setBlock(67, 7, COIN);
        setBlock(70, 6, COIN);
        setBlock(71, 7, COIN);
        setBlock(72, 8, COIN);
        setBlock(73, 7, COIN);
        setBlock(74, 7, COIN);
        setBlock(75, 7, COIN);
        setBlock(76, 8, COIN);
        setBlock(16, 3, BLOCK_EMPTY);
        setBlock(18, 3, BLOCK_EMPTY);
        setBlock(17, 3, BLOCK_EMPTY);
        setBlock(95, 14, GROUND);
        setBlock(96, 14, GROUND);
        setBlock(97, 14, GROUND);
        setBlock(98, 14, GROUND);
        setBlock(99, 14, GROUND);
        setBlock(16, 11, GROUND);
        setBlock(5, 7, BLOCK_EMPTY);
        setBlock(25, 7, BLOCK_EMPTY);
        setBlock(45, 7, BLOCK_EMPTY);
        setBlock(85, 7, BLOCK_EMPTY);
        setBlock(16, 11, HILL_TOP);
        setBlock(95, 14, HILL_TOP);
        setBlock(96, 14, HILL_TOP);
        setBlock(97, 14, HILL_TOP);
        setBlock(98, 14, HILL_TOP);
        setBlock(99, 14, HILL_TOP);

        xExit = 98;
        yExit = 14;
    }
}
