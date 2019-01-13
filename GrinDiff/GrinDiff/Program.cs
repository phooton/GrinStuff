using System;
using System.Collections;
using System.Linq;
using System.Numerics;

namespace GrinDiff
{
    class Program
    {
        static string pre_pow = "00010000000000005229000000005c39e67e0004047aa170d439a5abe867adfdc9328f14972d593c4bc5b1a78882e98c5bb509610a49b488350fe33e01ca61ce72febc3e031d61e7b07fb5d84f50c0a923fbf5de0e27675b6a69d475ae6214d63edcfd9365c79bc36c218e658f584835f5770d711965ed3dab8975090c15565eaf93553104f480d966010469efcba16fac1b842ee85d2e2799d6e34399a4386e952cea2037190475cd7efacda38cddde52d0427982f23826d01bd18c168cb28b6262cce14d15ad5bfe61929eadace6ab32c3000000000001ff6400000000000142fc00000000f75c0868000002dc";
        static int height = 21033;
        static ulong[] nonces = new ulong[] { 4090213, 21134919, 27292199, 51997486, 52994723, 56051047, 77676868, 87653980, 101696818, 106637318, 109284737, 119482027, 124173812, 132521452, 134897158, 145071779, 155091585, 163405432, 171996149, 178943886, 200561165, 206691763, 210887794, 229836717, 258088099, 284983840, 303275384, 310678673, 311416850, 315133515, 335242291, 351258931, 358338647, 374445523, 375593195, 379240054, 423815706, 458113714, 467541171, 482864578, 494444189, 536357183 };
        static ulong nonce = 7366379558552056063UL;

        static int proof_size = 29;

        static void Main(string[] args)
        {
            ulong scale = GetScaleFromPrePow();

            //https://github.com/mimblewimble/grin/blob/ea38e15a6f7db024160f3a9534a4c17b09f0a8ac/core/src/pow/types.rs#L315

            BitArray packed = new BitArray(42 * proof_size);
            byte[] packedSolution = new byte[153]; // 42*proof_size/8 padded
            int position = 0;
            foreach (var n in nonces)
            {
                for (int i = 0; i < proof_size; i++)
                    packed.Set(position++, (n & (1UL << i)) != 0 );
            }
            packed.CopyTo(packedSolution, 0);

            var hash = new Crypto.Blake2B(256);

            BigInteger shift = (new BigInteger(scale)) << 64;                                        // unsigned , big endian
            BigInteger diff = shift / (new BigInteger(hash.ComputeHash(packedSolution).Take(8).ToArray(), true, true));

            ulong share_difficulty = Math.Min((UInt64)diff, UInt64.MaxValue);

            Console.WriteLine("MATCH: " + (share_difficulty == 554151));

            Console.ReadKey();
        }

        public static ulong GetScaleFromPrePow()
        {
            if (!string.IsNullOrEmpty(pre_pow))
            {
                byte[] header = GetHeader().Reverse().ToArray();

                if (header.Length > 20)
                {
                    return BitConverter.ToUInt32(header, 0);
                }
                else
                    return 1;
            }
            else
                return 1;
        }

        public static byte[] GetHeader()
        {
            return Enumerable.Range(0, pre_pow.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(pre_pow.Substring(x, 2), 16))
                     .ToArray();
        }
    }
}
