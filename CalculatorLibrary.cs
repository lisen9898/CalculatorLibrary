using System.Diagnostics;
using System.Security.Principal;
using Newtonsoft.Json;

namespace CalculatorLibrary
{
    public class Calculator
    {
        JsonWriter writer;
        private int usageCount;
        private List<CalculationRecord> calculationHistory = new List<CalculationRecord>();
        public Calculator()
        {
            StreamWriter logFile = File.CreateText("calculator.json");
            logFile.AutoFlush = true;
            writer = new JsonTextWriter(logFile)
            {
                Formatting = Formatting.Indented
            };
            writer.WriteStartObject();
            writer.WritePropertyName("Operations");
            writer.WriteStartArray();
        }

        public double DoOperation(double num1, string? op)
        {
            double result = double.NaN; // Default value is "not-a-number" if an operation, such as division, could result in an error.
            writer.WriteStartObject();
            writer.WritePropertyName("Operand1");
            writer.WriteValue(num1);

            // Use a switch statement to do the math.
            double num2 = 0;
            switch (op)
            {
                case "a":
                    num2 = AskNum2();
                    result = num1 + num2;
                    writer.WritePropertyName("Operand2");
                    writer.WriteValue(num2);
                    writer.WritePropertyName("Operation");
                    writer.WriteValue("Add");
                    break;
                case "s":
                    num2 = AskNum2();
                    result = num1 - num2;
                    writer.WritePropertyName("Operand2");
                    writer.WriteValue(num2);
                    writer.WritePropertyName("Operation");
                    writer.WriteValue("Substract");
                    break;
                case "m":
                    num2 = AskNum2();
                    result = num1 * num2;
                    writer.WritePropertyName("Operand2");
                    writer.WriteValue(num2);
                    writer.WritePropertyName("Operation");
                    writer.WriteValue("Multiply");
                    break;
                case "d":
                    num2 = AskNum2();
                    // Ask the user to enter a non-zero divisor.
                    if (num2 != 0)
                    {
                        result = num1 / num2;
                        writer.WritePropertyName("Operand2");
                        writer.WriteValue(num2);
                        writer.WritePropertyName("Operation");
                        writer.WriteValue("Divide");
                    }
                    break;
                // Return text for an incorrect option entry.
                case "p":
                    num2 = AskNum2();
                    result = Math.Pow(num1, num2);
                    writer.WritePropertyName("Operand2");
                    writer.WriteValue(num2);
                    writer.WritePropertyName("Operation");
                    writer.WriteValue("Power of");
                    break;
                case "r":
                    result = Math.Sqrt(num1);
                    writer.WritePropertyName("Operation");
                    writer.WriteValue("Square  Root");
                    break;
                case "x":
                    result = num1 * 10;
                    writer.WritePropertyName("Operation");
                    writer.WriteValue("10x");
                    break;
                case "i":
                    result = Math.Sin(num1);
                    writer.WritePropertyName("Operation");
                    writer.WriteValue("Sin");
                    break;
                case "c":
                    result = Math.Cos(num1);
                    writer.WritePropertyName("Operation");
                    writer.WriteValue("Cos");
                    break;
                case "t":
                    result = Math.Tan(num1);
                    writer.WritePropertyName("Operation");
                    writer.WriteValue("Tan");
                    break;
                default:
                    break;
            }
            writer.WritePropertyName("Result");
            writer.WriteValue(result);
            IncrementUsageCount();
            writer.WritePropertyName("UsageCount");
            writer.WriteValue(GetUsagecount());
            writer.WriteEndObject();

            var calculationRecord = new CalculationRecord()
            {
                Operand1 = num1,
                Operand2 = num2,
                Operation = op == "a" ? "+" :
                            op == "s" ? "-" :
                            op == "m" ? "*" :
                            op == "d" ? "/" :
                            op == "p" ? "**" :
                            op == "x" ? "* 10" :
                            op == "r" ? "√" :
                            op == "c" ? "cos" :
                            op == "i" ? "sin" :
                            "tan",
                Result = result,
            };

            calculationHistory.Add(calculationRecord);
            return result;
        }

        public void Finish()
        {
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Close();

        }

        private void IncrementUsageCount()
        {
            usageCount++;
        }

        public int GetUsagecount()
        {
            return usageCount;
        }

        public void GetHistoryAndPropmt()
        {
            Console.WriteLine("Calculation History:");
            if (calculationHistory != null)
            {

                foreach (var item in calculationHistory)
                {
                    if (item.Operation == "+" || item.Operation == "-" || item.Operation == "*" || item.Operation == "/" || item.Operation == "**")
                    {
                        Console.WriteLine($"{item.Operand1} {item.Operation} {item.Operand2} = {item.Result}");
                    }
                    else if (item.Operation == "* 10")
                    {
                        Console.WriteLine($"{item.Operand1} {item.Operation} = {item.Result}");
                    }
                    else
                    {
                        Console.WriteLine($"{item.Operation}{item.Operand1} = {item.Result}");
                    }
                 
                }
                Console.WriteLine("\nPress 'd' and Enter to clear record, or press any other key and Enter to continue: ");
                if (Console.ReadLine() == "d")
                {
                    ClearCalculationHistory();
                }
            }
        }

        public void ClearCalculationHistory()
        {
            calculationHistory.Clear();
        }

        double AskNum2()
        {
            // Ask the user to type the second number.
            Console.Write("Type another number, and then press Enter: ");
            string? numInput2 = Console.ReadLine();
            double cleanNum2;

            while (!double.TryParse(numInput2, out cleanNum2))
            {
                Console.Write("This is not valid input. Please enter an integer value: ");
                numInput2 = Console.ReadLine();
            }
            return cleanNum2;
        }

    }

    public class CalculationRecord
    {
        public double Operand1 { get; set; }
        public double Operand2 { get; set; }
        public string Operation {  get; set; }
        public double Result { get; set; }
    }
}