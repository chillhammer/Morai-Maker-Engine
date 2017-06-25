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

        setBlock(0, 11, BLOCK_EMPTY);
        setBlock(1, 11, BLOCK_EMPTY);
        setBlock(2, 11, BLOCK_EMPTY);
        setBlock(3, 11, BLOCK_EMPTY);
        setBlock(4, 11, BLOCK_EMPTY);
        setBlock(5, 11, BLOCK_EMPTY);
        setBlock(6, 11, BLOCK_EMPTY);
        setBlock(7, 11, BLOCK_EMPTY);
        setBlock(8, 11, BLOCK_EMPTY);
        setBlock(9, 11, BLOCK_EMPTY);
        setBlock(10, 11, BLOCK_EMPTY);
        setBlock(11, 10, BLOCK_EMPTY);
        setBlock(12, 10, BLOCK_EMPTY);
        setBlock(13, 10, BLOCK_EMPTY);
        setBlock(14, 9, BLOCK_EMPTY);
        setBlock(15, 9, BLOCK_EMPTY);
        setBlock(16, 9, BLOCK_EMPTY);
        setBlock(16, 8, BLOCK_EMPTY);
        setBlock(17, 8, BLOCK_EMPTY);
        setBlock(18, 8, BLOCK_EMPTY);
        setBlock(19, 8, BLOCK_EMPTY);
        setBlock(20, 8, BLOCK_EMPTY);
        setBlock(20, 7, BLOCK_EMPTY);
        setBlock(21, 7, BLOCK_EMPTY);
        setBlock(22, 7, BLOCK_EMPTY);
        setBlock(22, 8, BLOCK_EMPTY);
        setBlock(23, 8, BLOCK_EMPTY);
        setBlock(24, 8, BLOCK_EMPTY);
        setBlock(24, 7, BLOCK_EMPTY);
        setBlock(25, 7, BLOCK_EMPTY);
        setBlock(26, 7, BLOCK_EMPTY);
        setBlock(27, 7, BLOCK_EMPTY);
        setBlock(28, 8, BLOCK_EMPTY);
        setBlock(29, 8, BLOCK_EMPTY);
        setBlock(30, 8, BLOCK_EMPTY);
        setBlock(31, 8, BLOCK_EMPTY);
        setBlock(32, 8, BLOCK_EMPTY);
        setBlock(33, 8, BLOCK_EMPTY);
        setBlock(33, 9, BLOCK_EMPTY);
        setBlock(34, 9, BLOCK_EMPTY);
        setBlock(34, 10, BLOCK_EMPTY);
        setBlock(35, 10, BLOCK_EMPTY);
        setBlock(36, 10, BLOCK_EMPTY);
        setBlock(36, 11, BLOCK_EMPTY);
        setBlock(37, 11, BLOCK_EMPTY);
        setBlock(37, 10, BLOCK_EMPTY);
        setBlock(38, 10, BLOCK_EMPTY);
        setBlock(39, 10, BLOCK_EMPTY);
        setBlock(39, 9, BLOCK_EMPTY);
        setBlock(38, 9, BLOCK_EMPTY);
        setBlock(38, 8, BLOCK_EMPTY);
        setBlock(39, 8, BLOCK_EMPTY);
        setBlock(39, 7, BLOCK_EMPTY);
        setBlock(40, 7, BLOCK_EMPTY);
        setBlock(40, 6, BLOCK_EMPTY);
        setBlock(41, 6, BLOCK_EMPTY);
        setBlock(41, 5, BLOCK_EMPTY);
        setBlock(42, 5, BLOCK_EMPTY);
        setBlock(42, 4, BLOCK_EMPTY);
        setBlock(43, 4, BLOCK_EMPTY);
        setBlock(44, 4, BLOCK_EMPTY);
        setBlock(45, 4, BLOCK_EMPTY);
        setBlock(46, 4, BLOCK_EMPTY);
        setBlock(46, 5, BLOCK_EMPTY);
        setBlock(47, 5, BLOCK_EMPTY);
        setBlock(48, 5, BLOCK_EMPTY);
        setBlock(49, 6, BLOCK_EMPTY);
        setBlock(50, 6, BLOCK_EMPTY);
        setBlock(51, 6, BLOCK_EMPTY);
        setBlock(51, 7, BLOCK_EMPTY);
        setBlock(52, 7, BLOCK_EMPTY);
        setBlock(53, 8, BLOCK_EMPTY);
        setBlock(54, 8, BLOCK_EMPTY);
        setBlock(55, 8, BLOCK_EMPTY);
        setBlock(55, 9, BLOCK_EMPTY);
        setBlock(56, 9, BLOCK_EMPTY);
        setBlock(57, 9, BLOCK_EMPTY);
        setBlock(57, 8, BLOCK_EMPTY);
        setBlock(58, 8, BLOCK_EMPTY);
        setBlock(58, 7, BLOCK_EMPTY);
        setBlock(57, 7, BLOCK_EMPTY);
        setBlock(58, 6, BLOCK_EMPTY);
        setBlock(58, 5, BLOCK_EMPTY);
        setBlock(59, 5, BLOCK_EMPTY);
        setBlock(59, 4, BLOCK_EMPTY);
        setBlock(58, 4, BLOCK_EMPTY);
        setBlock(60, 4, BLOCK_EMPTY);
        setBlock(61, 4, BLOCK_EMPTY);
        setBlock(61, 5, BLOCK_EMPTY);
        setBlock(62, 5, BLOCK_EMPTY);
        setBlock(63, 6, BLOCK_EMPTY);
        setBlock(63, 7, BLOCK_EMPTY);
        setBlock(64, 7, BLOCK_EMPTY);
        setBlock(64, 8, BLOCK_EMPTY);
        setBlock(65, 9, BLOCK_EMPTY);
        setBlock(65, 10, BLOCK_EMPTY);
        setBlock(66, 10, BLOCK_EMPTY);
        setBlock(66, 11, BLOCK_EMPTY);
        setBlock(67, 11, BLOCK_EMPTY);
        setBlock(68, 11, BLOCK_EMPTY);
        setBlock(69, 11, BLOCK_EMPTY);
        setBlock(69, 10, BLOCK_EMPTY);
        setBlock(70, 10, BLOCK_EMPTY);
        setBlock(71, 10, BLOCK_EMPTY);
        setBlock(71, 9, BLOCK_EMPTY);
        setBlock(72, 9, BLOCK_EMPTY);
        setBlock(72, 8, BLOCK_EMPTY);
        setBlock(73, 8, BLOCK_EMPTY);
        setBlock(74, 8, BLOCK_EMPTY);
        setBlock(75, 8, BLOCK_EMPTY);
        setBlock(76, 8, BLOCK_EMPTY);
        setBlock(77, 8, BLOCK_EMPTY);
        setBlock(78, 8, BLOCK_EMPTY);
        setBlock(78, 9, BLOCK_EMPTY);
        setBlock(79, 9, BLOCK_EMPTY);
        setBlock(80, 9, BLOCK_EMPTY);
        setBlock(81, 9, BLOCK_EMPTY);
        setBlock(81, 10, BLOCK_EMPTY);
        setBlock(82, 10, BLOCK_EMPTY);
        setBlock(83, 11, BLOCK_EMPTY);
        setBlock(84, 11, BLOCK_EMPTY);
        setBlock(85, 11, BLOCK_EMPTY);
        setBlock(86, 11, BLOCK_EMPTY);
        setBlock(87, 11, BLOCK_EMPTY);
        setBlock(88, 11, BLOCK_EMPTY);
        setBlock(89, 11, BLOCK_EMPTY);
        setBlock(90, 11, BLOCK_EMPTY);
        setBlock(91, 11, BLOCK_EMPTY);
        setBlock(92, 10, BLOCK_EMPTY);
        setBlock(93, 10, BLOCK_EMPTY);
        setBlock(93, 9, BLOCK_EMPTY);
        setBlock(94, 9, BLOCK_EMPTY);
        setBlock(95, 9, BLOCK_EMPTY);
        setBlock(96, 9, GROUND);
        setBlock(97, 9, GROUND);
        setBlock(98, 9, GROUND);
        setBlock(99, 9, GROUND);
        setBlock(96, 9, HILL_TOP);
        setBlock(97, 9, HILL_TOP);
        setBlock(98, 9, HILL_TOP);
        setBlock(99, 9, HILL_TOP);

        xExit = 98;
        yExit = 9;
    }
}
