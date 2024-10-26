namespace PWPluginSamples.Shared
{
    public interface IBridge
    {
        string this[int index] { get; set; }

        string CreatePerson(string name, int age);
        void DoSomething(double radius = 10);
        string GetGreeting();
        string GetMessage(string message);
    }
}
