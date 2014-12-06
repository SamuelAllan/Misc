namespace DeployDB
{
    public interface Feedback
    {
        void WriteLine();
        void WriteLine(string line);
        void WriteLine(string format, params object[] args);
    }
}
