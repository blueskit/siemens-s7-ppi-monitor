using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S7PpiMonitor.Protocols;

namespace S7PpiMonitor;

internal static class ProgramTestPpi
{
    internal static void test_001()
    {
        var s01 = "68 1B 1B 68 02 00 7C 32 01 00 00 26 58 00 0E 00 00 04 01 12 0A 10 02 00 0C 00 01 84 00 09 60 6A 16";
        var s02 = "68 21 21 68 00 02 08 32 03 00 00 26 58 00 02 00 10 00 00 04 01 FF 04 00 60 43 6C 99 B2 43 15 80 0B 43 33 66 76 66 16";
        var s03 = "68 63 63 68 02 00 7C 32 01 00 00 26 59 00 56 00 00 04 07 12 0A 10 02 00 02 00 00 81 00 00 00 12 0A 10 02 00 01 00 00 82 00 00 00 12 0A 10 02 00 01 00 00 83 00 00 00 12 0A 10 02 00 01 00 00 83 00 00 50 12 0A 10 02 00 01 00 01 84 00 03 20 12 0A 10 02 00 01 00 01 84 00 03 70 12 0A 10 02 00 04 00 01 84 00 06 40 A2 16 E5 10 02 00 5C 5E 16";
        var s04 = "68 3D 3D 68 00 02 08 32 03 00 00 26 59 00 02 00 2C 00 00 04 07 FF 04 00 10 00 00 FF 04 00 08 09 00 FF 04 00 08 00 00 FF 04 00 08 00 00 FF 04 00 08 00 00 FF 04 00 08 00 00 FF 04 00 20 00 47 00 6E 22 16";
        var s10 = "68 E5 E5 68 00 04 08 32 03 01 02 88 1E 7F 80 02 00 A8 01 04 88 30 D0 11 06 08 B4 42 00 00 00 01 04 08 20 40 11 06 08 06 10 A0 40 00 01 00 00 01 F4 11 06 08 2C 40 40 00 32 02 D0 00 3C 05 C6 46 18 BB C0 96 75 30 05 63 06 90 A9 30 61 00 00 FE 11 06 08 2C 40 C0 00 32 01 68 00 3C 05 C6 46 18 BB C0 96 0B B8 2D C3 05 90 A9 30 C2 00 00 FE 11 06 08 2C 40 81 00 3C 01 84 0F 20 28 30 52 82 B0 01 2C 75 30 05 63 06 90 A9 30 C2 00 00 FE 11 06 08 2C 40 40 00 32 01 90 01 A2 28 18 52 C1 B0 01 2C 75 30 05 63 06 90 A9 30 C2 00 00 FE 11 06 08 34 40 00 00 00 01 04 08 20 40 11 06 08 31 40 FD 11 06 08 B1 60 82 FF 11 03 08 2C B0 A1 11 06 08 31 40 4E FF 11 0B 0C 59 60 78 FF D4 05 63 06 90 28 80 28 01 C2 FE D4 05 63 16 18 3F 80 00 01 C2 FE D4 05 A3 01 F4 01 90 05 A3 FE D4 05 A3 51 16 68 3F 7D 42 13 06 10 93 16 0C 20 40 E2 F9 00 64 00 00 08 04 12 29 82 4C 18 40 C2 00 01 84 01 12 04 95 21 41 C8 02 00 04 00 01 84 01 4A 06 49 21 41 88 02 00 0C 01 14 8C 28 40 D5 30 12 29 82 26 18 20 41 80 01 84 01 72 04 31 16 E5 10 02 00 B8 79 B3 FC 68 3B 3B 68 00 04 08 32 03 01 02 88 5E 5F 80 02 00 54 00 00 11 46 EC 11 06 08 34 40 00 00 00";

        var s20 = "68 4B 4B 68 02 00 7C 32 01 00 00 7E C8 00 3E 00 00 04 05 12 0A 10 02 00 08 00 00 83 00 00 00 12 0A 10 02 00 04 00 00 83 00 00 88 12 0A 10 02 00 20 00 01 84 00 17 90 12 0A 10 02 00 11 00 01 84 00 2E E0 12 0A 10 02 00 04 00 01 84 00 30 58 BF 16 E5 10 02 00 5C";

        var s90 = "68 1B 1B 68 02 00 7C 32 01 00 00 26 62 00 0E 00 00 04 01 12 0A 10 02 00 0C 00 01 84 00 09 60 74 16 E5 10 02 00 5C 5E 16 68 21 21 68 00 02 08 32 03 00 00 26 62 00 02 00 10 00 00 04 01 FF 04 00 60 43 6F 33 4C 43 15 80 0B 43 33 66 76 A7 16 68 63 63 68 02 00 7C 32 01 00 00 26 63 00 56 00 00 04 07 12 0A 10 02 00 02 00 00 81 00 00 00 12 0A 10 02 00 01 00 00 82 00 00 00 12 0A 10 02 00 01 00 00 83 00 00 00 12 0A 10 02 00 01 00 00 83 00 00 50 12 0A 10 02 00 01 00 01 84 00 03 20 12 0A 10 02 00 01 00 01 84 00 03 70 12 0A 10 02 00 04 00 01 84 00 06 40 AC 16 E5 10 02 00 5C 5E 16 68 3D 3D 68 00 02 08 32 03 00 00 26 63 00 02 00 2C 00 00 04 07 FF 04 00 10 00 00 FF 04 00 08 49 00 FF 04 00 08 00 00 FF 04 00 08 00 00 FF 04 00 08 00 00 FF 04 00 08 00 00 FF 04 00 20 00 49 00 6E 6E 16";
        var s91 = "68 23 23 68 02 00 7C 32 01 00 00 27 34 00 0E 00 08 05 01 12 0A 10 02 00 04 00 01 84 00 09 60 00 04 00 20 45 20 80 00 51 16 E5 10 02 00 5C 5E 16 68 12 12 68 00 02 08 32 03 00 00 27 34 00 02 00 01 00 00 05 01 FF A2 16";

        var buff01 = s01.Split(' ').Select(x => Convert.ToByte(x, 16)).ToArray();
        var buff02 = s02.Split(' ').Select(x => Convert.ToByte(x, 16)).ToArray();
        var buff03 = s03.Split(' ').Select(x => Convert.ToByte(x, 16)).ToArray();
        var buff04 = s04.Split(' ').Select(x => Convert.ToByte(x, 16)).ToArray();
        var buff10 = s10.Split(' ').Select(x => Convert.ToByte(x, 16)).ToArray();
        var buff20 = s20.Split(' ').Select(x => Convert.ToByte(x, 16)).ToArray();

        var buff90 = s90.Split(' ').Select(x => Convert.ToByte(x, 16)).ToArray();
        var buff91 = s91.Split(' ').Select(x => Convert.ToByte(x, 16)).ToArray();

        var bufHelper = new BufferManager();

        //bufHelper.WriteBytes(buff01);
        //bufHelper.WriteBytes(buff02);
        //bufHelper.WriteBytes(buff03);
        //bufHelper.WriteBytes(buff04);
        //bufHelper.WriteBytes(buff91);
        bufHelper.WriteBytes(buff20);

        var pduRefMap = PpiSD2PDU._pduRefMap;

        int frameNo = 0;
        while (bufHelper.Buffer.ReadableBytes > 0) {
            while (bufHelper.TryGetFrame(out var frame)) {
                frame.ParsePDU();
                Console.WriteLine("{0:000}|FRAME:{1}", ++frameNo, frame);
            }
        }


        Console.WriteLine("PRESS ANY KEY ...");
        Console.ReadLine();
    }
}
