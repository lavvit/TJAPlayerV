namespace Loader
{
    public class Bar
    {
        public int NoteCount = 1;
        public double Time = 0;
        public double BPM = 0;
        public double Measure = 1;
        public double Scroll = 1;

        public List<Chip> Chips = [];
        public List<Command> Commands = [];

        public override string ToString()
        {
            string note = "";
            foreach (Chip chip in Chips)
            {
                bool commanding = false;
                foreach (Command command in Commands)
                {
                    if (command.Position == chip.Position)
                    {
                        note += $"{(!commanding ? "\n" : "")}{command.Name}\n";
                        commanding = true;
                    }
                }
                note += ((int)chip.Type).ToString();
            }
            return note + ",";
        }
    }

    public class Command
    {
        public int Position;
        public string Name = "";
    }

}