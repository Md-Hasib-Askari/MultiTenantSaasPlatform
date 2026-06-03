// Adapter Interface
interface ITarget
{
    void Request();
}

// Adaptee
public class Adaptee
{
    public void SpecificRequest()
    {
        Console.WriteLine("Called SpecificRequest()");
    }
}

// Adapter
public class Adapter : ITarget
{
    private readonly Adaptee _adaptee;

    public Adapter(Adaptee adaptee)
    {
        _adaptee = adaptee;
    }

    public void Request()
    {
        // Translate the request to the specific request of the adaptee
        _adaptee.SpecificRequest();
    }
}

// Usage
class Program
{
    static void Main(string[] args)
    {
        Adaptee adaptee = new Adaptee();
        ITarget target = new Adapter(adaptee);
        target.Request(); // Output: Called SpecificRequest()
    }
}
