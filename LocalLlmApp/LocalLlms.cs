using LocalSemanticKernel;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class MainProgram
{
    static async Task<int> Main(string[] args)
    {
        var choice = AnsiConsole.Prompt(MenuOperations.MainMenu());
        while (choice.Id > -1)
        {
            var func = choice.Id switch
            {
                0 => LocalLlms.AskQuestions(),
                1 => LocalLlms.Amphibians(),
                2 => LocalLlms.Superhero(),
                3 => LocalLlms.TrainOnWebPage(true),
                4 => LocalLlms.TrainOnWebPage(false),
                _ => throw new NotImplementedException()
            };
            await func;
            choice = AnsiConsole.Prompt(MenuOperations.MainMenu());
        }

        Console.WriteLine("Program Finished");
        return await AsyncConsoleWork();
    }

    private static async Task<int> AsyncConsoleWork() => 0;
    public record MenuItem
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public override string ToString() => Text;

    }
    internal static class MenuOperations
    {
        public static SelectionPrompt<MenuItem> MainMenu()
        {
            var menuItems = new List<MenuItem>
            {
                new() { Id = 0, Text = "Ask random questions " },
                new() { Id = 1, Text = "What's an amphibian? - Text Embedding Cosine Similarity " },
                new() { Id = 2, Text = "Bruno's favourite super hero? - Memory Content" },
                new() { Id = 3, Text = "Train on webpage and add to an sqlite database" },
                new() { Id = 4, Text = "Train on webpage and add to vector database" },
                new() { Id = -1, Text = "Exit Program" }
            };
            SelectionPrompt<MenuItem> menu = new()
            {
                HighlightStyle = new Style(
                    Color.DodgerBlue1,
                    Color.Black,
                    Decoration.None)
            };

            menu.Title("What example do you want to run?")
                .PageSize(4)
                .MoreChoicesText("[grey](Move arrow up and down to select)[/]")
                .AddChoices(menuItems);

            return menu;
        }
    }
}
