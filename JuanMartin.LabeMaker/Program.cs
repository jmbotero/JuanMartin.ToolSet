namespace JuanMartin.ToolSet.LabeMaker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var label = new DVDLabelMaker();

            label.Create("12 Monkeys");
        }
    }
}
