using SFServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Configs
{
    public class VersionConfig
    {
        public string RequiredVersion { get; set; }
        public string LevelCompabilityVersion { get; set; }

        public DateTimeOffset TermsOfServiceAgreementUpdated { get; set; }
        public DateTimeOffset PrivacyPolicyUpdated { get; set; }

        public ClientVersion requiredVersion
        {
            get
            {
                if (_requiredVersion == null)
                    ClientVersion.Parse(RequiredVersion, out _requiredVersion);

                return _requiredVersion;
            }
            set
            {
                _requiredVersion = value;
            }
        }
        public ClientVersion levelCompabilityVersion
        {
            get
            {
                if (_levelCompabilityVersion == null)
                    ClientVersion.Parse(LevelCompabilityVersion, out _levelCompabilityVersion);

                return _levelCompabilityVersion;
            }
            set
            {
                _levelCompabilityVersion = value;
            }
        }

        private ClientVersion _requiredVersion;
        private ClientVersion _levelCompabilityVersion;
    }
}
