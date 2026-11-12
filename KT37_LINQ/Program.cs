using System.Globalization;

namespace KT37_LINQ
{
    public class Order
    {
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public Order(string customerName, decimal amount, DateTime date)
        {
            CustomerName = customerName;
            Amount = amount;
            Date = date;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            List<Order> orders = new List<Order>();

            int count = 0;

            while (count < 10)
            {
                try
                {
                    Console.Write("Введите количество заказов (минимум 10): ");
                    count = int.Parse(Console.ReadLine());

                    if (count < 10)
                    {
                        Console.WriteLine("Необходимо добавить минимум 10 заказов.");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Ошибка! Введите целое число.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Введено слишком большое число.");
                }
            }

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine();
                Console.WriteLine($"Заказ номер: {i + 1}");

                string customerName = "";

                while (string.IsNullOrWhiteSpace(customerName))
                {
                    Console.Write("Введите имя клиента: ");
                    customerName = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(customerName))
                    {
                        Console.WriteLine("Имя клиента не может быть пустым.");
                    }
                }

                decimal amount = 0;

                while (amount <= 0)
                {
                    try
                    {
                        Console.Write("Введите сумму заказа: ");
                        amount = decimal.Parse(
                            Console.ReadLine().Replace(',', '.'),
                            CultureInfo.InvariantCulture);

                        if (amount <= 0)
                        {
                            Console.WriteLine("Сумма должна быть больше нуля.");
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Ошибка! Введите корректную сумму.");
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Введено слишком большое значение.");
                    }
                }

                DateTime date = DateTime.MinValue;

                while (date == DateTime.MinValue)
                {
                    try
                    {
                        Console.Write("Введите дату заказа (дд.мм.гггг): ");
                        date = DateTime.ParseExact(
                            Console.ReadLine(),
                            "dd.MM.yyyy",
                            CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Ошибка! Используйте формат дд.мм.гггг.");
                    }
                }

                orders.Add(new Order(customerName, amount, date));
            }

            Console.WriteLine();
            Console.WriteLine("Выбор периода анализа:");

            DateTime startDate = DateTime.MinValue;

            while (startDate == DateTime.MinValue)
            {
                try
                {
                    Console.Write("Введите начальную дату (дд.мм.гггг): ");
                    startDate = DateTime.ParseExact(
                        Console.ReadLine(),
                        "dd.MM.yyyy",
                        CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Ошибка! Используйте формат дд.мм.гггг.");
                }
            }

            DateTime endDate = DateTime.MinValue;

            while (endDate == DateTime.MinValue || endDate < startDate)
            {
                try
                {
                    Console.Write("Введите конечную дату (дд.мм.гггг): ");
                    endDate = DateTime.ParseExact(
                        Console.ReadLine(),
                        "dd.MM.yyyy",
                        CultureInfo.InvariantCulture);

                    if (endDate < startDate)
                    {
                        Console.WriteLine("Конечная дата не может быть раньше начальной.");
                        endDate = DateTime.MinValue;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Ошибка! Используйте формат дд.мм.гггг.");
                }
            }

            var report = orders
                .Where(o => o.Date >= startDate && o.Date <= endDate)
                .GroupBy(o => o.CustomerName)
                .Select(g => new
                {
                    Customer = g.Key,
                    Total = g.Sum(o => o.Amount)
                })
                .OrderByDescending(r => r.Total)
                .Take(3);

            Console.WriteLine();
            Console.WriteLine("ТОП-3 клиента по сумме заказов:");
            Console.WriteLine();

            if (!report.Any())
            {
                Console.WriteLine("За выбранный период заказов нет.");
            }
            else
            {
                foreach (var row in report)
                {
                    Console.WriteLine($"{row.Customer}: {row.Total} руб.");
                }
            }
        }
    }
}