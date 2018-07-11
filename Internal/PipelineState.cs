namespace Pipeline.Internal
{
    internal class PipelineState
    {
        public bool EndReached { get; private set; }

        private readonly bool[] _nextCalled;

        private bool _endReached;

        public PipelineState(int numItems)
        {
            _nextCalled = new bool[numItems];
            reset();
        }

        public void setEndReached()
        {
            EndReached = true;
        }

        public void setNextCalled(int index)
        {
            _nextCalled[index] = true;
        }

        public bool getNextCalled(int index)
        {
            return _nextCalled[index];
        }

        public void reset()
        {
            EndReached = false;

            for (var i = 0; i < _nextCalled.Length; i++)
                _nextCalled[i] = false;
        }
    }
}
