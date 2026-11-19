// The dwarf. Every action here just changes plain numbers on this object
// and returns a message describing what happened - no Console calls live
// in this file, so it stays readable without knowing anything about how
// the game is displayed.
public class Player
{
    private const int MoveStep = 5;

    public const int SunCostWood = 40;
    public const int SunCostStone = 40;
    public const int SunCostIron = 25;
    public const int SunCostCrystal = 20;

    // How much the angel's tear knocks off each of the four sun costs
    // above, once found.
    private const int AngelTearDiscount = 10;

    public string Name { get; set; }
    public int Wood { get; set; }
    public int Iron { get; set; }
    public int Crystal { get; set; }
    public int Stone { get; set; }

    // Negative = west of camp, positive = east of camp, 0 = at camp.
    public int Location { get; set; }

    public bool SunBuilt { get; set; }

    // The three special items, each hidden in one randomly chosen biome
    // (see World's *Biome properties) and found by Gathering there. None
    // of them is required to build the sun - each one just makes it easier.
    public bool HasDog { get; set; }
    public bool HasAngelTear { get; set; }
    public bool HasDragonEye { get; set; }

    public Player(string name)
    {
        Name = name;
        Wood = 0;
        Iron = 0;
        Crystal = 0;
        Stone = 0;
        Location = 0;
        SunBuilt = false;
        HasDog = false;
        HasAngelTear = false;
        HasDragonEye = false;
    }

    public string MoveLeft(World world)
    {
        int targetLocation = Location - MoveStep;

        if (world.BiomeCodeAt(targetLocation) == 'd')
        {
            return Name + " stops at the water's edge. The sea blocks the way west.";
        }

        Location = targetLocation;
        return Name + " walks west across the land.";
    }

    public string MoveRight(World world)
    {
        int targetLocation = Location + MoveStep;

        if (world.BiomeCodeAt(targetLocation) == 'd')
        {
            return Name + " stops at the water's edge. The sea blocks the way east.";
        }

        Location = targetLocation;
        return Name + " walks east across the land.";
    }

    // How many days the trip back to camp costs - the number of ordinary
    // moves it would take, or none at all with the dragon's eye.
    // Location only ever changes by MoveStep, so this always divides evenly.
    public int StepsToReturnToCamp()
    {
        if (HasDragonEye)
        {
            return 0;
        }

        return Math.Abs(Location) / MoveStep;
    }

    public string ReturnToCamp()
    {
        if (Location == 0)
        {
            return Name + " is already at camp.";
        }

        if (HasDragonEye)
        {
            Location = 0;
            return "The dragon's eye flares, and " + Name + " is back at camp in the blink of an eye.";
        }

        int steps = StepsToReturnToCamp();
        Location = 0;
        return Name + " treks all the way back to camp, taking " + steps + " days.";
    }

    public string BuildSun()
    {
        if (SunBuilt)
        {
            return "The new sun already blazes in the sky.";
        }

        if (Location != 0)
        {
            return "You need to be back at camp to build the sun.";
        }

        int woodCost = EffectiveSunCost(SunCostWood);
        int stoneCost = EffectiveSunCost(SunCostStone);
        int ironCost = EffectiveSunCost(SunCostIron);
        int crystalCost = EffectiveSunCost(SunCostCrystal);

        if (Wood >= woodCost && Stone >= stoneCost && Iron >= ironCost && Crystal >= crystalCost)
        {
            Wood -= woodCost;
            Stone -= stoneCost;
            Iron -= ironCost;
            Crystal -= crystalCost;
            SunBuilt = true;
            return "A new sun ignites and climbs into the sky!";
        }

        return "Not enough resources yet to build the sun. Need " + woodCost + " wood, " + stoneCost + " stone, " + ironCost + " iron, " + crystalCost + " crystal.";
    }

    // The angel's tear discounts every sun cost equally, never below zero.
    private int EffectiveSunCost(int baseCost)
    {
        if (HasAngelTear)
        {
            return Math.Max(0, baseCost - AngelTearDiscount);
        }

        return baseCost;
    }
}
