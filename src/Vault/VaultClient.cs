﻿using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace Vault
{
    public class VaultClient : IVaultClient
    {
        private readonly VaultHttpClient _httpClient;
        private readonly VaultClientConfiguration _config;

        private readonly object _lock = new object();

        public VaultClient() : this(VaultClientConfiguration.Default)
        {
        }

        public VaultClient(VaultClientConfiguration config)
        {
            _httpClient = new VaultHttpClient();
            _config = config;
        }

        internal Task<T> Get<T>(string path, string token, CancellationToken ct)
        {
            return _httpClient.Get<T>(BuildVaultUri(path), token, ct);
        }

        internal Task<T> Get<T>(string path, CancellationToken ct)
        {
            return _httpClient.Get<T>(BuildVaultUri(path), _config.Token, ct);
        }

        internal Task<T> List<T>(string path, CancellationToken ct)
        {
            return _httpClient.Get<T>(BuildVaultUri(path, new NameValueCollection { { "list", "true" } }),
                _config.Token, ct);
        }

        internal Task<TO> Post<TI, TO>(string path, TI content, CancellationToken ct)
        {
            return _httpClient.Post<TI, TO>(BuildVaultUri(path), content, _config.Token, ct);
        }

        internal Task PostVoid<T>(string path, T content, CancellationToken ct)
        {
            return _httpClient.PostVoid(BuildVaultUri(path), content, _config.Token, ct);
        }

        internal Task PutVoid(string path, CancellationToken ct)
        {
            return _httpClient.PutVoid(BuildVaultUri(path), _config.Token, ct);
        }

        internal Task PutVoid<T>(string path, T content, CancellationToken ct)
        {
            return _httpClient.PutVoid(BuildVaultUri(path), content, _config.Token, ct);
        }

        internal Task<TO> Put<TI, TO>(string path, TI content, CancellationToken ct)
        {
            return _httpClient.Put<TI, TO>(BuildVaultUri(path), content, _config.Token, ct);
        }

        internal Task DeleteVoid(string path, CancellationToken ct)
        {
            return _httpClient.DeleteVoid(BuildVaultUri(path), _config.Token, ct);
        }

        private Uri BuildVaultUri(string path, NameValueCollection parameters = null)
        {
            var uriBuilder = new UriBuilder(_config.Address)
            {
                Path = path
            };

            if (parameters == null) return uriBuilder.Uri;

            var parseParameters = QueryHelpers.ParseQuery(string.Empty);
            foreach (var k in parameters.AllKeys)
            {
                parseParameters.Add(k, parameters[k]);
            }
            uriBuilder.Query = parseParameters.ToString();

            return uriBuilder.Uri;
        }

        private Endpoints.Sys.ISysEndpoint _sys;
        public Endpoints.Sys.ISysEndpoint Sys
        {
            get
            {
                if (_sys == null)
                {
                    lock (_lock)
                    {
                        if (_sys == null)
                        {
                            _sys = new Endpoints.Sys.SysEndpoint(this);
                        }
                    }
                }
                return _sys;
            }
        }

        private Endpoints.ISecretEndpoint _secret;
        public Endpoints.ISecretEndpoint Secret
        {
            get
            {
                if (_secret == null)
                {
                    lock (_lock)
                    {
                        if (_secret == null)
                        {
                            _secret = new Endpoints.SecretEndpoint(this);
                        }
                    }
                }
                return _secret;
            }
        }

    }
}
