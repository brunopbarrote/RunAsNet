namespace RunAsNet
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiLogon.CreateProcessWithLogonWNetOnly();
        }
    }
}
