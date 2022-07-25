using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KerberoShutdown
{
    public class Enums
    {
        [Flags]
        public enum PrintColor
        {
            YELLOW = 0,
            GREEN = 1,
            RED = 2
        }

        public static Dictionary<string, int> DictFlags = new Dictionary<string, int>
        {
                { "NoAuthDataRequired", 33554432 },
                { "TrustedToAuthenticateForDelegation", 16777216 },
                { "TRUSTED_TO_AUTH_FOR_DELEGATION", 16777216 },
                { "PasswordExpired", 8388608 },
                { "DontRequirePreauth", 4194304 },
                { "UseDesKeyOnly", 2097152 },
                { "AccountNotDelegated", 1048576 },
                { "TrustedForDelegation", 524288 },
                { "SmartCardRequired", 262144 },
                { "MnsLogonAccount", 131072 },
                { "PasswordDoesNotExpire", 65536 },
                { "ServerTrustAccount", 8192 },
                { "WorkstationTrustAccount", 4096 },
                { "InterDomainTrustAccount", 2048 },
                { "NormalAccount", 512 },
                { "TempDuplicateAccount", 256 },
                { "EncryptedTextPasswordAllowed", 128 },
                { "PasswordCannotChange", 64 },
                { "PasswordNotRequired", 32 },
                { "AccountLockedOut", 16 },
                { "HomeDirectoryRequired", 8 },
                { "AccountDisabled", 2 },
                { "Script", 1 }
        };
    }
}
