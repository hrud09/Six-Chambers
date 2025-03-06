using System;
using System.Collections.Generic;
public static class Statics
{
    public static float goldenAspect = 0.5625f;
    public static DateTime resetDateTime = new DateTime(2000, 1, 1, 12, 0, 0);
    public static int freeSpinResetTime = 86400;
    public static int levelWinCoinReward = 20;
    public static List<int> hardLevelNums = new List<int>() { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
    public static List<int> rewardLevelNums = new List<int>() { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
    public static List<int> powerupUnlockLevelNums = new List<int>() { 3, 6, 10 }; //3, 6, 10
    public static List<int> isPowerupShownChecker = new List<int>();
    public static bool isLevelWinAnimationPending;

    public static string FormatNumber(long num)
    {
        if (num >= 100000000)
        {
            return (num / 1000000D).ToString("0.#M");
        }
        if (num >= 1000000)
        {
            return (num / 1000000D).ToString("0.##M");
        }
        if (num >= 100000)
        {
            return (num / 1000D).ToString("0.#k");
        }
        if (num >= 10000)
        {
            return (num / 1000D).ToString("0.##k");
        }

        return num.ToString("#,0");
    }
}

public enum SCENE_NUM
{
    LOADING_SCENE = 0,
    MAIN_MENU = 1,
    GAME_SCENE = 2
}

public enum UI_VIEW
{
    SHOP = 0,
    HOME,
    EQUIPMENT
}

public enum ECONOMY_TYPE
{
    COIN = 0,
    ENERGY
}

public enum FlyObjectType
{
    Coins = 0,
    ExtraPin = 3,
    ExtraCardSlot = 6,
    Hammer = 10
}
/*
public enum CardType
{
    EMPTY = 0,
    NORMAL,
    ELEMENT
}*/

public enum CardColorType
{
    EMPTY = 0,
    RED,
    GREEN,
    BLUE,
    YELLOW,
    PURPLE
}

public enum CardElementType
{
    EMPTY = 0,
    GRASS,
    WIND,
    FIRE,
    THUNDER,
    ICE
}


public enum PinState
{
    Attached = 0,
    Pulled
}

public enum PowerupType
{
    EMPTY = 0,
    EXTRA_PIN_SLOT,
    EXTRA_CARD_SLOT,
    HAMMER
}

