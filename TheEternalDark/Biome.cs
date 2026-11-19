// One biome: its letter code, and what can be gathered there. Gather
// switches on the code, each case giving the player a small randomized
// amount of whatever that biome offers. No Console calls in this file.
//
// 'g' (crimson forest), 'h' (radioactive fields) and 'i' (purple crystal
// ravines) are deliberately left unhandled here - see World.cs. They
// fall through to the default case, even though WorldArt.cs still
// draws them.
public class Biome
{
    // How much the dog adds to every gather roll below, once found.
    private const int DogBonus = 2;

    public char Code { get; set; }

    private Random rnd;

    public Biome(char code)
    {
        Code = code;
        rnd = new Random();
    }

    // Between 3 and 6, plus the dog's bonus once it has been found.
    private int RollAmount(Player player)
    {
        int gained = rnd.Next(3, 7);

        if (player.HasDog)
        {
            gained += DogBonus;
        }

        return gained;
    }

    public string Gather(Player player)
    {
        switch (Code)
        {
            case 'a':
                return "There is nothing to gather at camp.";

            case 'b':
                int woodGained = RollAmount(player);
                player.Wood += woodGained;
                return player.Name + " chops wood in the forest and gathers " + woodGained + " wood.";

            case 'c':
                int stoneGained = RollAmount(player);
                int ironGained = RollAmount(player) / 2;
                player.Stone += stoneGained;
                player.Iron += ironGained;
                return player.Name + " mines the mountain and gathers " + stoneGained + " stone and " + ironGained + " iron.";

            case 'd':
                return "The sea has nothing you can carry back.";

            case 'e':
                int crystalGained = RollAmount(player);
                player.Crystal += crystalGained;
                return player.Name + " carefully gathers " + crystalGained + " crystal shards.";

            case 'f':
                int desertStoneGained = RollAmount(player);
                player.Stone += desertStoneGained;
                return player.Name + " digs sun-baked stone out of the desert and gathers " + desertStoneGained + " stone.";

            default:
                return "There is nothing to gather here.";
        }
    }
}
