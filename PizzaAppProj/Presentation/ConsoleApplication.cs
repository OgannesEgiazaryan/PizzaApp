using PizzaAppProj.Application.Interfaces;
using PizzaAppProj.Application.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Presentation
{
    public sealed class ConsoleApplication(IPizzaService pizzaService, IOrderService orderService)
    {
        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            RenderHeader();

            var isRunning = true;
            while (isRunning)
            {
                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold orange1]Выберите действие[/]")
                        .PageSize(10)
                        .AddChoices(
                            "Сделать заказ",
                            "Меню",
                            "Информационное табло",
                            "История заказов",
                            "Админка",
                            "Выход"));

                switch (action)
                {
                    case "Сделать заказ":
                        await CreateOrderAsync(cancellationToken);
                        break;
                    case "Меню":
                        await ShowMenuAsync(cancellationToken);
                        break;
                    case "Информационное табло":
                        await ShowBoardAsync(cancellationToken);
                        break;
                    case "История заказов":
                        await ShowHistoryAsync(cancellationToken);
                        break;
                    case "Админка":
                        await ShowAdminMenuAsync(cancellationToken);
                        break;
                    case "Выход":
                        isRunning = false;
                        break;
                }
            }

            AnsiConsole.MarkupLine("[grey]Работа программы завершена.[/]");
        }

        private static void RenderHeader()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Pizza App")
                    .Color(Color.Orange1));
            AnsiConsole.Write(
                new Panel("[bold]Пиццерия с заказами, табло готовности, историей и админкой[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Grey));
            AnsiConsole.WriteLine();
        }

        private async Task ShowMenuAsync(CancellationToken cancellationToken)
        {
            var pizzas = await pizzaService.GetMenuAsync(cancellationToken);
            if (pizzas.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Меню пока пустое.[/]");
                return;
            }

            var table = new Table().Border(TableBorder.Rounded).Expand();
            table.AddColumn("Id");
            table.AddColumn("Название");
            table.AddColumn("Описание");
            table.AddColumn("Состав");
            table.AddColumn("Вес");
            table.AddColumn("Ккал/100г");
            table.AddColumn("Цена");

            foreach (var pizza in pizzas)
            {
                table.AddRow(
                    pizza.Id.ToString(),
                    Markup.Escape(pizza.Name),
                    Markup.Escape(pizza.Description),
                    Markup.Escape(pizza.Ingredients),
                    $"{pizza.WeightGrams} г",
                    pizza.CaloriesPer100Grams.ToString(),
                    pizza.Price.ToString("C"));
            }

            AnsiConsole.Write(table);
        }

        private async Task CreateOrderAsync(CancellationToken cancellationToken)
        {
            try
            {
                var pizzas = await pizzaService.GetMenuAsync(cancellationToken);
                if (pizzas.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Невозможно оформить заказ: меню пустое.[/]");
                    return;
                }

                var selectedPizzas = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<PizzaMenuItemDto>()
                        .Title("[bold]Выберите одну или несколько пицц[/]")
                        .InstructionsText("[grey](Пробел - выбрать, Enter - подтвердить)[/]")
                        .UseConverter(pizza =>
                            $"#{pizza.Id} {Markup.Escape(pizza.Name)} · {pizza.Price:C} · {pizza.WeightGrams} г")
                        .AddChoices(pizzas));

                var orderLines = new List<OrderLineRequest>();
                foreach (var pizza in selectedPizzas)
                {
                    var quantity = AnsiConsole.Prompt(
                        new TextPrompt<int>($"Сколько штук [orange1]{Markup.Escape(pizza.Name)}[/] добавить?")
                            .PromptStyle("orange1")
                            .Validate(value => value > 0
                                ? ValidationResult.Success()
                                : ValidationResult.Error("Количество должно быть больше нуля.")));

                    orderLines.Add(new OrderLineRequest(pizza.Id, quantity));
                }

                var customerName = AnsiConsole.Prompt(
                    new TextPrompt<string>("Введите имя клиента:")
                        .PromptStyle("green")
                        .Validate(value => !string.IsNullOrWhiteSpace(value)
                            ? ValidationResult.Success()
                            : ValidationResult.Error("Имя не может быть пустым.")));

                var summary = await orderService.CreateOrderAsync(
                    new CreateOrderRequest(customerName, orderLines),
                    cancellationToken);

                var orderInfo = string.Join(Environment.NewLine, summary.Items.Select(item =>
                    $"- {item.PizzaName} x{item.Quantity} = {item.LineTotal:C}"));

                AnsiConsole.Write(
                    new Panel($"[bold green]Заказ №{summary.OrderNumber} принят[/]\n" +
                              $"Клиент: {Markup.Escape(summary.CustomerName)}\n" +
                              $"Состав:\n{Markup.Escape(orderInfo)}\n" +
                              $"Итог: [bold]{summary.TotalCost:C}[/]\n" +
                              $"Ориентировочное время готовности: [bold]{ToLocalTime(summary.ReadyAt):HH:mm}[/]")
                        .Header("Новый заказ")
                        .Border(BoxBorder.Double)
                        .BorderColor(Color.Green));
            }
            catch (Exception exception)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(exception.Message)}[/]");
            }
        }

        private async Task ShowBoardAsync(CancellationToken cancellationToken)
        {
            var orders = await orderService.GetBoardAsync(cancellationToken);
            if (orders.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]Активных заказов нет.[/]");
                return;
            }

            var table = new Table().Border(TableBorder.Heavy).Expand();
            table.AddColumn("Номер");
            table.AddColumn("Клиент");
            table.AddColumn("Статус");
            table.AddColumn("Состав");
            table.AddColumn("Сумма");
            table.AddColumn("Готов к");

            foreach (var order in orders)
            {
                var items = string.Join(", ", order.Items.Select(item => $"{item.PizzaName} x{item.Quantity}"));
                table.AddRow(
                    order.OrderNumber.ToString(),
                    Markup.Escape(order.CustomerName),
                    $"[{order.Status.ToMarkupColor()}]{order.Status.ToDisplayName()}[/]",
                    Markup.Escape(items),
                    order.TotalCost.ToString("C"),
                    ToLocalTime(order.ReadyAt).ToString("HH:mm"));
            }

            AnsiConsole.Write(table);
        }

        private async Task ShowHistoryAsync(CancellationToken cancellationToken)
        {
            var history = await orderService.GetHistoryAsync(cancellationToken);
            if (history.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]История заказов пока пуста.[/]");
                return;
            }

            var table = new Table().Border(TableBorder.Rounded).Expand();
            table.AddColumn("Номер");
            table.AddColumn("Клиент");
            table.AddColumn("Статус");
            table.AddColumn("Состав");
            table.AddColumn("Сумма");
            table.AddColumn("Заказан");
            table.AddColumn("Выдан");

            foreach (var order in history)
            {
                var items = string.Join(", ", order.Items.Select(item => $"{item.PizzaName} x{item.Quantity}"));
                table.AddRow(
                    order.OrderNumber.ToString(),
                    Markup.Escape(order.CustomerName),
                    $"[{order.Status.ToMarkupColor()}]{order.Status.ToDisplayName()}[/]",
                    Markup.Escape(items),
                    order.TotalCost.ToString("C"),
                    ToLocalTime(order.OrderedAt).ToString("dd.MM HH:mm"),
                    order.IssuedAt is null ? "-" : ToLocalTime(order.IssuedAt.Value).ToString("dd.MM HH:mm")
                );
            }

            AnsiConsole.Write(table);
        }

        private static DateTimeOffset ToLocalTime(DateTimeOffset value) => value.ToLocalTime();

        private async Task ShowAdminMenuAsync(CancellationToken cancellationToken)
        {
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold orange1]Админка[/]")
                    .AddChoices("Выдать заказ", "Добавить новую пиццу", "Назад"));

            switch (action)
            {
                case "Выдать заказ":
                    await IssueOrderAsync(cancellationToken);
                    break;
                case "Добавить новую пиццу":
                    await AddPizzaAsync(cancellationToken);
                    break;
            }
        }

        private async Task IssueOrderAsync(CancellationToken cancellationToken)
        {
            var orderNumber = AnsiConsole.Prompt(
                new TextPrompt<long>("Введите номер заказа:")
                    .PromptStyle("orange1")
                    .Validate(value => value > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("Номер заказа должен быть положительным.")));

            var order = await orderService.MarkAsIssuedAsync(orderNumber, cancellationToken);
            if (order is null)
            {
                AnsiConsole.MarkupLine("[red]Заказ не найден.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[green]Заказ №{order.OrderNumber} выдан клиенту {Markup.Escape(order.CustomerName)}.[/]");
        }

        private async Task AddPizzaAsync(CancellationToken cancellationToken)
        {
            try
            {
                var name = AnsiConsole.Prompt(new TextPrompt<string>("Название пиццы:").PromptStyle("green"));
                var description = AnsiConsole.Prompt(new TextPrompt<string>("Краткое описание:").PromptStyle("green"));
                var ingredients = AnsiConsole.Prompt(new TextPrompt<string>("Состав:").PromptStyle("green"));
                var price = AnsiConsole.Prompt(new TextPrompt<decimal>("Цена:").PromptStyle("green"));
                var weight = AnsiConsole.Prompt(new TextPrompt<int>("Вес (г):").PromptStyle("green"));
                var calories = AnsiConsole.Prompt(new TextPrompt<int>("Ккал на 100 г:").PromptStyle("green"));

                var pizza = await pizzaService.AddPizzaAsync(
                    new CreatePizzaRequest(name, description, ingredients, price, weight, calories),
                    cancellationToken);

                AnsiConsole.MarkupLine($"[green]Пицца {Markup.Escape(pizza.Name)} добавлена в меню.[/]");
            }
            catch (Exception exception)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(exception.Message)}[/]");
            }
        }
    }
}
