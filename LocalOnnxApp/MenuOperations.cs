using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalOnnxApp
{
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
                .PageSize(6)
                .MoreChoicesText("[grey](Move arrow up and down to select)[/]")
                .AddChoices(menuItems);

            return menu;
        }
    }
}
