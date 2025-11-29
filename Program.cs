using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using S7PpiMonitor.BackService;
using S7PpiMonitor.Forms;

namespace S7PpiMonitor;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

#if DEBUG
        test();
#endif

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddLogging(logbuilder => {
            logbuilder.ClearProviders();
            logbuilder.AddNLog();
        });

        //builder.Services.AddHostedService<S7PpiCommunicationAnalyzingService>();

        var app = builder.Build();
        app.Start();

        ApplicationConfiguration.Initialize();
        Application.Run(new S7PPIForm());

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }


    [Conditional("DEBUG")]
    private static void test()
    {
        var i1 = 0x838012c3;

        var f1 = Convert.ToSingle(0x838012c3);
        var f2 = Convert.ToSingle(0xc3128083);

        var i21 = Convert.ToInt32((float)100.1);
        var i22 = Convert.ToInt32((float)123.45);

        var f21 = 123.45f;
        var f21buff = BitConverter.GetBytes(f21);

        //ProgramTestPpi.test_001();

        //Console.WriteLine("PRESS ANY KEY... ...");
        //Console.ReadLine();
    }


}