// The whole game world: the player, the seed that describes it, and how
// long it's been dark. No Console calls in this file - Display.cs is the
// only place that prints anything.
//
// There's no array/list of biomes stored anywhere. Instead BiomeSeed is a
// string of one-letter biome codes, and BiomeCodeAt(location) reads the
// right character straight out of it - strings support indexing (code[i])
// and a length, which is all that's needed here without reaching for a
// real collection type.
//
// Biome codes:
//   a = camp              e = crystal peaks       h = radioactive fields (easter egg)
//   b = forest             f = desert              i = purple crystal ravines (easter egg)
//   c = mountain           g = crimson forest (easter egg)
//   d = sea (endless at both ends of the world)
//
// g, h and i are deliberately left out of the student assignment and of
// the required Biome.Gather cases - WorldArt.cs still renders them
// (crimson trees, a Geiger-glow glyph, sparkling ravine walls), so a
// curious student poking around the visuals can discover them and add
// them as a bonus.
public class World
{
    // How many steps wide one biome is.
    public const int CharsPerBiome = 20;

    // Odds (1 in this many) that Gathering in the right biome turns up
    // that biome's special item, rolled fresh every time until found.
    private const int ItemFindChance = 8;

    public string BiomeSeed { get; set; }
    public Player Player { get; set; }
    public int DaysWithoutSun { get; set; }

    // Each special item is hidden in one biome, picked once at world
    // generation. DogBiome/AngelTearBiome/DragonEyeBiome are always three
    // different letters, but which biome that is changes every game.
    public char DogBiome { get; set; }
    public char AngelTearBiome { get; set; }
    public char DragonEyeBiome { get; set; }

    private Random rnd;

    public World(string playerName)
    {
        rnd = new Random();
        BiomeSeed = GenerateBiomeSeed();
        Player = new Player(playerName);
        DaysWithoutSun = 0;

        string itemBiomes = PickDistinctLetters("bcefghi", 3);
        DogBiome = itemBiomes[0];
        AngelTearBiome = itemBiomes[1];
        DragonEyeBiome = itemBiomes[2];
    }

    // Camp ('a') sits in the middle, sea ('d') bookends both ends, and the
    // 7 other biomes are shuffled once each into the remaining slots (4
    // west of camp, 3 east) - every biome exists exactly once in the
    // world, so finding one means it's genuinely new.
    private string GenerateBiomeSeed()
    {
        string shuffled = PickDistinctLetters("bcefghi", 7);
        string west = shuffled.Substring(0, 4);
        string east = shuffled.Substring(4, 3);
        return "d" + west + "a" + east + "d";
    }

    // Picks `count` distinct letters out of `pool`, in random order and
    // without replacement. Used both to lay out the biomes themselves and
    // to decide which biomes hide the special items.
    private string PickDistinctLetters(string pool, int count)
    {
        string picked = "";

        for (int i = 0; i < count; i++)
        {
            int pick = rnd.Next(0, pool.Length);
            picked = picked + pool[pick];
            pool = pool.Substring(0, pick) + pool.Substring(pick + 1);
        }

        return picked;
    }

    // ===== GIVEN CODE: do not change =====
    // Every location maps onto one BiomeSeed character, CharsPerBiome
    // steps wide, with camp ('a') at location 0. The shift is added before
    // dividing so the math works the same for locations west of camp
    // (negative) as it does east. Anything beyond the string is endless sea.
    public char BiomeCodeAt(int location)
    {
        int campIndex = BiomeSeed.IndexOf('a');
        int shifted = location + CharsPerBiome / 2 + CharsPerBiome * campIndex;
        int index = shifted / CharsPerBiome;

        if (index < 0 || index >= BiomeSeed.Length)
        {
            return 'd';
        }

        return BiomeSeed[index];
    }
    // ===== END OF GIVEN CODE =====

    // Called once for every action the player takes - one action, one day.
    public void ProcessDay()
    {
        DaysWithoutSun += 1;
    }

    // Called after Gathering. Rolls to see whether the player's current
    // biome turns up its special item - only one of the three checks can
    // ever hit, since each item lives in its own biome. Returns null when
    // nothing is found, so callers only show a message when there's one
    // to show.
    public string TryFindSpecialItem()
    {
        char biomeCode = BiomeCodeAt(Player.Location);

        if (biomeCode == DogBiome && !Player.HasDog && rnd.Next(0, ItemFindChance) == 0)
        {
            Player.HasDog = true;
            return "A shaggy dog bounds out of the undergrowth and decides to follow " + Player.Name + " home!";
        }

        if (biomeCode == AngelTearBiome && !Player.HasAngelTear && rnd.Next(0, ItemFindChance) == 0)
        {
            Player.HasAngelTear = true;
            return Player.Name + " finds a single, perfectly clear tear, frozen solid among the rocks.";
        }

        if (biomeCode == DragonEyeBiome && !Player.HasDragonEye && rnd.Next(0, ItemFindChance) == 0)
        {
            Player.HasDragonEye = true;
            return Player.Name + " unearths a petrified dragon's eye, still faintly warm to the touch.";
        }

        return null;
    }
}
