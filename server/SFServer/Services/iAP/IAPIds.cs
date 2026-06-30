using SFServer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Services.iAP
{
    public class IAPIds
    {
        private static List<AnimalIdPair> animalIdPairs = new List<AnimalIdPair>(){
        new AnimalIdPair(Animal.Debra, "iap.character.zebra"),
        new AnimalIdPair(Animal.Honky, "iap.character.goose"),
        new AnimalIdPair(Animal.Jaws, "iap.character.shark"),
        new AnimalIdPair(Animal.Koko, "iap.character.monkey"),
        new AnimalIdPair(Animal.Leon, "iap.character.lion"),
        new AnimalIdPair(Animal.Nugget, "iap.character.chicken"),
        new AnimalIdPair(Animal.Olipher, "iap.character.elephant"),
        new AnimalIdPair(Animal.Patchy, "iap.character.giraffe"),
        new AnimalIdPair(Animal.Perry, "iap.character.platypus"),
        new AnimalIdPair(Animal.Rex, "iap.character.dinosaur"),
        new AnimalIdPair(Animal.Speedy, "iap.character.snail"),
    };


        public static string GetIAPIdByCharacter(Animal character)
        {
            AnimalIdPair pair = animalIdPairs.FirstOrDefault(x => x.animal == character);

            if (pair == null)
            {
                return "iAP Id for {character} not found!";
            }

            return pair.id;
        }

        public static Animal? GetCharacterByIAPId(string id)
        {
            AnimalIdPair pair = animalIdPairs.FirstOrDefault(x => x.id == id);

            if (pair == null)
            {
                return null;
            }

            return pair.animal;
        }

        private class AnimalIdPair
        {
            public Animal animal;
            public string id;

            public AnimalIdPair(Animal animal, string id)
            {
                this.animal = animal;
                this.id = id;
            }
        }
    }
}
