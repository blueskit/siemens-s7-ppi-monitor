using S7PpiMonitor.Models;

namespace S7PpiMonitor.Utilities;

public class VariableFileReader
{
    private VarConfigFile _config;

    public VariableFileReader()
    {
        _config = new VarConfigFile();
    }

    public VariableFileReader(VarConfigFile config)
    {
        _config = config;
    }

    public void ReadFromFile(string filename)
    {
    }

    public void WriteToFile(string filename)
    {
    }

}
