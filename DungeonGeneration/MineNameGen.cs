using System.Text;
using UnityEngine;

public static class MineNameGen
{
    public static string MakeName()
	{
        string RandAdj = Adjectives[Random.Range(0, Adjectives.Length)];
        string RandNoun = Nouns[Random.Range(0, Nouns.Length)];
        StringBuilder builder = new StringBuilder();
        builder.Append(RandAdj).Append(" ").Append(RandNoun);
		return builder.ToString();
	}

    private static readonly string[] Adjectives = {
        "Awful",
        "Crowded",
        "Damp",
        "Dank",
        "Dark",
        "Dirty",
        "Empty",
        "Evil",
        "Frozen",
        "Fungal",
        "Grungy",
        "Horrid",
        "Metallic",
        "Moldy",
        "Musty",
        "New",
        "Nice",
        "Nondescript",
        "Perfect",
        "Quaint",
        "Queer",
        "Salty",
        "Slimey",
        "Smelly",
        "Terror",
        "Twisty",
        "Ugly",
        "Weird",
        "Wet",
    };

    private static readonly string[] Nouns =
    {
        "Cave",
        "Cavern",
        "Cenote",
        "Dungeon",
        "Deposit",
        "Grotto",
        "Haven",
        "Hoard",
        "Hole",
        "Lode",
        "Mine",
        "Pit",
        "Quarry",
        "Shaft",
        "Trove",
        "Tunnel",
        "Vein"
    };
}
