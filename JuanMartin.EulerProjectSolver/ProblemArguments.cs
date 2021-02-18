namespace EulerExam
{
    class ProblemArguments
    {
        private int _id=1;
        private long _number=0;
        private long[] _numbers=null;
        private string _sequence = string.Empty;

        public long Number
        {
            get { return _number; }
        }

        public int ID
        {
            get { return _id; }
        }

        public string Sequence
        {
            get { return _sequence; }
        }

        public long[] Numbers
        {
            get { return _numbers; }
        }

        public ProblemArguments(int argument1)
        {
            _id = argument1;
        }

        public ProblemArguments(int argument1, long[] argument2)
            : this(argument1)
        {
            _numbers = argument2;
        }

        public ProblemArguments(int argument1, long argument2)
            : this(argument1)
        {
            _number = argument2;
        }

        public ProblemArguments(int argument1, string argument2)
            : this(argument1)
        {
            _sequence = argument2;
        }

        public ProblemArguments(int argument1, long argument2, string argument3)
            : this(argument1, argument2)
        {
            _sequence = argument3;
        }

        public ProblemArguments(int argument1, long argument2, string argument3, long[] argument4)
            : this(argument1, argument2,argument3)
        {
            _numbers = argument4;
        }
    }
}
