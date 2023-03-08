namespace RemaSoftware.Domain.Constants;

public class OrderPriorityConstant
{
    public static readonly Dictionary<int, string> OrderPriority = new Dictionary<int, string>
    {
        { 0, "Minore" }, { 1, "Normale" }, { 2, "Maggiore" }, { 3, "Critica" }, { 4, "Bloccante" },
    };
}