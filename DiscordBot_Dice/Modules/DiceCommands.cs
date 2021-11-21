using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot_Dice.Modules
{
    /// <summary>
    /// The general module containing commands like ping.
    /// </summary>
    public class DiceCommands : ModuleBase<SocketCommandContext>
    {
        //Roll a dice with a variable number of dices and sides and return all the results.
        [Command("roll")]
        [Summary("Roll a dice with a variable number of dices and sides")]
        public async Task Roll(int dices = 1, int sides = 6, int skill = 0, int difficulty = 0)
        {
        
            if (dices < 1 || sides < 1)
            {
                await ReplyAsync("You can't roll less than 1 dice or less than 1 side.");
                return;
            }
            if (dices > 4 || sides > 20)
            {
                await ReplyAsync("You can't roll more than 4 dice or more than 20 sides.");
                return;
            }

            //Create a List of the results.
            var results = new int[dices];

            for (int i = 0; i < dices; i++)
            {
                results[i] = (new Random().Next(1, sides + 1));
            }

            //Get the highest result. In case of several results, get the both highest results.
            var highest = GetTheHighestOfResults(results);

            //TODO: Handle if the highest and the second highest are 1 or 2.

            //Create Message String.
            var sumDicePlusSkill = highest.highest + highest.secondHighest + skill;

            var resultTextStringBuilder = new StringBuilder();
            if(dices == 1)
            {
                resultTextStringBuilder.Append("Wurf ");
            }
            else if(dices > 1)
            {
                resultTextStringBuilder.Append("Würfe ");
            }
            resultTextStringBuilder.Append($"+ Attribut ergeben: {sumDicePlusSkill}");

            var successText = "";

            if(sumDicePlusSkill >= difficulty && difficulty != 0)
            {
                int success = (sumDicePlusSkill - difficulty) / 3;
                var stringBuilder = new StringBuilder($"Bei einer Schwierigkeit von {difficulty} ");

                if(success == 1) stringBuilder.Append($"ist das {success} Erfolgsgrad");
                if(success > 1) stringBuilder.Append($"sind das {success} Erfolgsgrade");
                successText = $"{stringBuilder}";
            }

            //Bei einer Schwierigkeitsstufe von 0 wird kein Erfolgsgrad angezeigt.
            await Context.Channel.SendMessageAsync($"Gewürfelt: [{string.Join(", ", results)}]. \n{resultTextStringBuilder} \n{successText}");
        }

        private (int highest, int secondHighest) GetTheHighestOfResults(int[] results)
        {
            int highest = 0;
            int secondHighest = 0;
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] > highest)
                {
                    secondHighest = highest;
                    highest = results[i];
                }
                else if (results[i] > secondHighest)
                {
                    secondHighest = results[i];
                }
            }

            return (highest, secondHighest);
        }

        [Command("rollhelp")]
        [Summary("Explains how to use the dice bot")]
        public async Task RollHelp()
        {
            await ReplyAsync("Wirf 1 - 4 Würfel mit einer variablen Anzahl an Seiten.\n" +
                             "Beispiele: \n" + 
                             "#roll -> würfelt einen W6 \n" +
                             "#roll 2 10 -> wüfelt zwei W10 und gibt die Werte aufgelistet zurück\n" +
                             "#roll 4 10 5 -> würfelt vier W10 und gibt die Werte augelistet zurück und addiert die beiden höchsten Werte mit dem Attribut \n" +
                             "#roll 4 10 5 10 -> fügt noch die Schwierigkeit hinzu und gibt zusätzlich die Erfolgsgrade aus.");
        }

    }
}