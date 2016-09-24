﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Vault.Models.Secret.Pki
{
    public class PkiIssueRequest
    {
        [JsonProperty("common_name")]
        public string CommonName { get; set; }

        [JsonProperty("alt_names", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _altNames;

        [JsonIgnore]
        public List<string> AltNames
        {
            get { return _altNames.Split(',').ToList(); }
            set { _altNames = string.Join(",", value); }
        }

        [JsonProperty("ip_sans", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _ipSans;

        [JsonIgnore]
        public List<string> IpSans
        {
            get { return _ipSans.Split(',').ToList(); }
            set { _ipSans = string.Join(",", value); }
        }

        [JsonProperty("ttl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Ttl { get; set; }

        [JsonProperty("format", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CertificateType Format { get; set; }

        [JsonProperty("exclude_cn_from_sans", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ExcludeCnFromSans { get; set; }
    }
}
